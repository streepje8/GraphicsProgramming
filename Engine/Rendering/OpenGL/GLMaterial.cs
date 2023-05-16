using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Serialization;
using Striped.Engine.Util;

namespace Striped.Engine.Rendering.TemplateRenderers;

[Serializable]
public struct SerializedMaterial
{
    public string shaderName;
    public string shader;
    public string[] textures;
}

public class GLMaterial : SerializeableObject
{
    public OpenGLShader shader;
    public List<GLTexture2D> textures {get; private set;} = new List<GLTexture2D>();

    public GLMaterial(string shaderName)
    {
        OpenGLShader glShader = OpenGLRenderer.GetShader(shaderName);
        if (glShader == null) Logger.Err("A shader with the name " + shaderName + " could not be found!");
        else shader = glShader;
    }
    

    public void Enable()
    {
        shader.Enable();
    }

    public override string Serialize()
    {
        SerializedMaterial material = new SerializedMaterial();
        material.shader = shader.Serialize();
        material.shaderName = shader.name;
        List<string> serializedTextures = new List<string>();
        foreach (var texture2D in textures)
        {
            serializedTextures.Add(texture2D.Serialize());
        }
        material.textures = serializedTextures.ToArray();
        return JsonConvert.SerializeObject(material, Formatting.Indented);
    }

    public override void Deserialize(string data)
    {
        SerializedMaterial material = JsonConvert.DeserializeObject<SerializedMaterial>(data);
        shader = OpenGLRenderer.GetShader(material.shaderName);
        if (shader == null)
        {
            shader = Deserializer.Deserialize<OpenGLShader>(material.shader);
            OpenGLRenderer.AddShaderInternal(shader);
        }
        textures = new List<GLTexture2D>();
        foreach (var materialTexture in material.textures)
        {
            textures.Add(Deserializer.Deserialize<GLTexture2D>(materialTexture));
        }
    }

    private readonly Dictionary<string, int> uniformCache = new Dictionary<string, int>();

    public void SetMatrix4(string name, Matrix4 value)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        GL.UniformMatrix4(endLocation, true, ref value);
    }
    
    public void SetVector3(string name, Vector3 value)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        GL.Uniform3(endLocation, ref value);
    }
    
    public void SetVector2(string name, Vector2 value)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        GL.Uniform2(endLocation, ref value);
    }
    
    public void SetFloat(string name, float value)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        GL.Uniform1(endLocation, value);
    }
    
    public void SetInt(string name, int value)
    {
        Enable();
        int endLocation = -1;
        if (!uniformCache.TryGetValue(name, out int location))
        {
            endLocation = shader.GetUniformLocation(name);
            uniformCache.Add(name, endLocation);
        }
        else endLocation = location;
        GL.Uniform1(endLocation, value);
    }
}