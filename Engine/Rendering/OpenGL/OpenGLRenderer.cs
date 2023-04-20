using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Rendering.Core;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Util;

namespace Striped.Engine.Rendering.TemplateRenderers;

public class OpenGLRenderer : Renderer
{
    int VertexBufferObject;
    float[] triangle = {
        -0.5f, -0.5f, 0.0f, 1f, 0.0f, 0.0f, //Bottom-left vertex 
        0.5f, -0.5f, 0.0f, 0.0f, 1f, 0.0f, //Bottom-right vertex
        0.0f,  0.5f, 0.0f, 0.0f, 0f, 1.0f, //Top vertex
    };
    
    float[] quad = {
        -0.5f, -0.5f, 0.0f, 1f, 0.0f, 0.0f, //Bottom-left vertex 
        0.5f, -0.5f, 0.0f, 0.0f, 1f, 0.0f, //Bottom-right vertex
        -0.5f, 0.5f, 0.0f, 0f, 0.0f, 1.0f, //Bottom-left vertex 
        0.5f, 0.5f, 0.0f, 1.0f, 1f, 0.0f, //Bottom-right vertex
    };
    int VertexArrayObject;


    private static Vector4 clearColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
    public static Vector4 ClearColor
    {
        get => clearColor;
        set
        {
            clearColor = value;
            GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
        }
    }

    private Dictionary<string, OpenGLShader> shaders = new Dictionary<string, OpenGLShader>();

    public override void OnResize(EngineWindow engineWindow, int width, int height)
    {
        GL.Viewport(0,0,width,height);
    }
    
    public OpenGLShader CreateShader(string filePath)
    {
        OpenGLShader shader = new OpenGLShader(filePath);
        shaders.Add(shader.name,shader);
        return shader;
    }

    private Transform transform;
    
    public override void OnLoad()
    {
        GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
        
        
        VertexBufferObject = GL.GenBuffer();
        VertexArrayObject = GL.GenVertexArray();
        
        GL.BindVertexArray(VertexArrayObject);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, quad.Length * sizeof(float), quad, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        
        transform = new Transform();
    }

    private Matrix4 mat;

    public override void OnRenderFrame()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        transform.rotation *= Quaternion.FromEulerAngles(new Vector3(0, 0, (float)(10000f * Time.deltaTime)));
        if (shaders.Values.Count > 0)
        {
            shaders.Values.First().Enable();

            mat = transform.TRS();
            //Logger.Info(mat);
            GL.UniformMatrix4(shaders.Values.First().GetUniformLocation("transform"), true, ref mat);
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }
    }

    public override void OnUnLoad()
    {
        GL.DeleteBuffer(VertexBufferObject);
        foreach (var openGLShader in shaders) openGLShader.Value.CleanUp();
    }
}