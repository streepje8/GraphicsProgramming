using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL4;
using Striped.Engine.Serialization;
using Striped.Engine.Util;

namespace Striped.Engine.Rendering.TemplateRenderers.Shaders;

public struct StringBlock
{
    public string name;
    public string content;
}

public class OpenGLShader : SerializeableObject
{
    public string name;
    private int handle;

    private int vertexShader;
    private int fragmentShader;

    private StringBlock fragmetShaderSource;
    private StringBlock vertexShaderSource;

    private string path = "";
    public OpenGLShader(string filePath)
    {
        LoadShader(filePath);
    }

    private void LoadShader(string filePath)
    {
        path = filePath;
        string shaderSource = File.ReadAllText(filePath);
        if (shaderSource.StartsWith("Shader"))
        {
            shaderSource = shaderSource.Replace("\r", "");
            shaderSource = shaderSource.Replace("\t", "");
            string token = "";
            List<StringBlock> resultingBlocks = new List<StringBlock>();
            Stack<StringBlock> blocks = new Stack<StringBlock>();
            foreach (var character in shaderSource.ToCharArray())
            {
                
                if (character == '[')
                {
                    StringBlock block = new StringBlock();
                    block.name = new Regex("[\\s*]").Replace(token,"");
                    blocks.Push(block);
                    token = "";
                }
                else
                {
                    if (character == ']')
                    {
                        StringBlock block = blocks.Pop();
                        block.content = token;
                        resultingBlocks.Add(block);
                        token = "";
                    }
                    else
                    {
                        token += character;
                    }
                }
            }

            foreach (var resultingBlock in resultingBlocks)
            {
                if (resultingBlock.name.Equals("Vertex", StringComparison.OrdinalIgnoreCase)) vertexShaderSource = resultingBlock;
                if (resultingBlock.name.Equals("Fragment", StringComparison.OrdinalIgnoreCase)) fragmetShaderSource = resultingBlock;
                if (resultingBlock.name.StartsWith("Shader", StringComparison.OrdinalIgnoreCase))
                {
                    name = new Regex("Shader").Replace(resultingBlock.name,"",1);
                    Logger.Info("Loaded Shader: " + name);
                }
            }
        }
        else
        {
            Logger.Except(new Exception("Invalid shader!"));
        }
    }

    public void BindSource()
    {
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource.content);
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmetShaderSource.content);
    }

    public void CompileAndLoad()
    {
        //Compile
        GL.CompileShader(vertexShader);

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexCompilationExitCode);
        if (vertexCompilationExitCode == 0)
        {
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            Logger.Err(infoLog);
        }

        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragmentCompilationExitCode);
        if (fragmentCompilationExitCode == 0)
        {
            string infoLog = GL.GetShaderInfoLog(fragmentShader);
            Logger.Err(infoLog);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
            LoadShader(Application.AssetsFolder + "/Shaders/Standard/errorShader.shader");
            BindSource();
            CompileAndLoad();
        }
        else
        {

            //Load
            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(handle);
                Logger.Err(infoLog);
            }
            Enable();
            for (int i = 0; i < 31; i++)
            {
                GL.Uniform1(GL.GetUniformLocation(handle,"texture" + i),i);
            }
            

            //Cleanup
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
        }
    }
    
    public void Enable()
    {
        GL.UseProgram(handle);
    }

    public void CleanUp()
    {
        GL.DeleteProgram(handle);
    }

    public int GetUniformLocation(string name) => GL.GetUniformLocation(handle, name);
    public int GetAttribLocation(string name) => GL.GetAttribLocation(handle, name);

    public void BindAttribLocation(int index, string name)
    {
        GL.BindAttribLocation(handle, index, name);
    }

    public override string Serialize()
    {
        return path;
    }

    public override void Deserialize(string data)
    {
        LoadShader(data);
        BindSource();
        CompileAndLoad();
    }

    public int Handle()
    {
        return handle;
    }
}