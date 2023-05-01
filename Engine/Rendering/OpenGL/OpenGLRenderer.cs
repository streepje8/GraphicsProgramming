using System.Net.Mime;
using GraphicsProgramming.Engine.BuildinComponents.Renderers.Abstract;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Core;
using Striped.Engine.Rendering.Core;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Util;

namespace Striped.Engine.Rendering.TemplateRenderers;

public class OpenGLRenderer : Renderer
{
    // need 24 vertices for normal/uv-mapped Cube
    // float[] vertices = new float[]{
    //     // positions            //colors            // tex coords   // normals
    //     0.5f, -0.5f, -0.5f,     1.0f, 1.0f, 1.0f,   1.0f, 0.0f,       00.0f, -10.0f, 00.0f,
    //     0.5f, -0.5f, 0.5f,      1.0f, 1.0f, 1.0f,   10.0f, 10.0f,       00.0f, -10.0f, 00.0f,
    //     -0.5f, -0.5f, 0.5f,     1.0f, 1.0f, 1.0f,   00.0f, 10.0f,       00.0f, -10.0f, 00.0f,
    //     -0.5f, -0.5f, -.5f,     1.0f, 1.0f, 1.0f,   00.0f, 00.0f,       00.0f, -10.0f, 00.0f,
    //
    //     0.5f, 0.5f, -0.5f,      1.0f, 1.0f, 1.0f,   20.0f, 00.0f,       10.0f, 00.0f, 00.0f,
    //     0.5f, 0.5f, 0.5f,       1.0f, 1.0f, 1.0f,   20.0f, 10.0f,       10.0f, 00.0f, 00.0f,
    //
    //     0.5f, 0.5f, 0.5f,       1.0f, 1.0f, 1.0f,   10.0f, 20.0f,       00.0f, 00.0f, 10.0f,
    //     -0.5f, 0.5f, 0.5f,      1.0f, 1.0f, 1.0f,   00.0f, 20.0f,       00.0f, 00.0f, 10.0f,
    //
    //     -0.5f, 0.5f, 0.5f,      1.0f, 1.0f, 1.0f,   -10.0f, 10.0f,      -10.0f, 00.0f, 00.0f,
    //     -0.5f, 0.5f, -.5f,      1.0f, 1.0f, 1.0f,   -10.0f, 00.0f,      -10.0f, 00.0f, 00.0f,
    //
    //     -0.5f, 0.5f, -.5f,      1.0f, 1.0f, 1.0f,   00.0f, -10.0f,      00.0f, 00.0f, -10.0f,
    //     0.5f, 0.5f, -0.5f,      1.0f, 1.0f, 1.0f,   10.0f, -10.0f,      00.0f, 00.0f, -10.0f,
    //
    //     -0.5f, 0.5f, -.5f,      1.0f, 1.0f, 1.0f,   30.0f, 00.0f,       00.0f, 10.0f, 00.0f,
    //     -0.5f, 0.5f, 0.5f,      1.0f, 1.0f, 1.0f,   30.0f, 10.0f,       00.0f, 10.0f, 00.0f,
    //
    //     0.5f, -0.5f, 0.5f,      1.0f, 1.0f, 1.0f,   10.0f, 10.0f,       00.0f, 00.0f, 10.0f,
    //     -0.5f, -0.5f, 0.5f,     1.0f, 1.0f, 1.0f,   00.0f, 10.0f,       00.0f, 00.0f, 10.0f,
    //
    //     -0.5f, -0.5f, 0.5f,     1.0f, 1.0f, 1.0f,   00.0f, 10.0f,       -10.0f, 00.0f, 00.0f,
    //     -0.5f, -0.5f, -.5f,     1.0f, 1.0f, 1.0f,   00.0f, 00.0f,       -10.0f, 00.0f, 00.0f,
    //
    //     -0.5f, -0.5f, -.5f,     1.0f, 1.0f, 1.0f,   00.0f, 00.0f,       00.0f, 00.0f, -10.0f,
    //     0.5f, -0.5f, -0.5f,     1.0f, 1.0f, 1.0f,   10.0f, 00.0f,       00.0f, 00.0f, -10.0f,
    //
    //     0.5f, -0.5f, -0.5f,     1.0f, 1.0f, 1.0f,   10.0f, 00.0f,       10.0f, 00.0f, 00.0f,
    //     0.5f, -0.5f, 0.5f,      1.0f, 1.0f, 1.0f,   10.0f, 10.0f,       10.0f, 00.0f, 00.0f,
    //
    //     0.5f, 0.5f, -0.5f,      1.0f, 1.0f, 1.0f,   20.0f, 00.0f,       00.0f, 10.0f, 00.0f,
    //     0.5f, 0.5f, 0.5f,       1.0f, 1.0f, 1.0f,   20.0f, 10.0f,       00.0f, 10.0f, 00.0f
    // };
    //
    // private int stride = (3 + 3 + 2 + 3) * sizeof(float);
    //
    //  int[] cubeIndices = new int[]{  // note that we start from 0!
    //     // DOWN
    //     0, 1, 2,   // first triangle
    //     0, 2, 3,    // second triangle
    //     // BACK
    //     14, 6, 7,   // first triangle
    //     14, 7, 15,    // second triangle
    //     // RIGHT
    //     20, 4, 5,   // first triangle
    //     20, 5, 21,    // second triangle
    //     // LEFT
    //     16, 8, 9,   // first triangle
    //     16, 9, 17,    // second triangle
    //     // FRONT
    //     18, 10, 11,   // first triangle
    //     18, 11, 19,    // second triangle
    //     // UP
    //     22, 12, 13,   // first triangle
    //     22, 13, 23,    // second triangle
    // };
    
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

    private static Dictionary<string, OpenGLShader?> shaders = new Dictionary<string, OpenGLShader?>();

    public override void OnResize(EngineWindow engineWindow, int width, int height)
    {
        GL.Viewport(0,0,width,height);
    }
    
    public OpenGLShader? CreateShader(string filePath)
    {
        OpenGLShader? shader = new OpenGLShader(filePath);
        shaders.Add(shader.name,shader);
        shader.BindSource();
        shader.CompileAndLoad();
        return shader;
    }

    public static OpenGLShader? GetShader(string name)
    {
        if (shaders.TryGetValue(name, out OpenGLShader? shader)) return shader;
        return null;
    }
    
    public override void OnLoad()
    {
        OpenGLShader? shader = CreateShader(Application.AssetsFolder + "/Shaders/Standard/errorShader.shader");
        GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
    }

    private Matrix4 mat;

    public override void OnRenderFrame()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        foreach (Camera cam in Camera.instances.Span)
        {
            if (cam.isActive)
            {
                RenderCamera(cam);
            }
        }
    }

    private void RenderCamera(Camera cam)
    {
        foreach (RenderComponent rend in cam.entity.GetEnvironment().GetAllComponentsInScene<RenderComponent>())
        {
            rend.OnRender(cam);
        }
    }

    public override void OnUnLoad()
    {
        foreach (var openGLShader in shaders) openGLShader.Value.CleanUp();
    }
}