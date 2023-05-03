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
        foreach (var camera in Component<Camera>.instances.Span)
        {
            camera.UpdateProjectionMatrix(new Vector2i(width,height));
        }
    }
    
    public OpenGLShader? CreateShader(string filePath)
    {
        OpenGLShader? shader = new OpenGLShader(filePath);
        shaders.Add(shader.name,shader);
        shader.BindSource();
        shader.CompileAndLoad();
        return shader;
    }
    
    public static void AddShaderInternal(OpenGLShader shader)
    {
        shaders.Add(shader.name,shader);
    }

    public static OpenGLShader? GetShader(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        if (shaders.TryGetValue(name, out OpenGLShader? shader)) return shader;
        return null;
    }
    
    public override void OnLoad()
    {
        OpenGLShader? shader = CreateShader(Application.AssetsFolder + "/Shaders/Standard/errorShader.shader");
        GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
        //GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
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

        Camera.SetNoneActive();
    }

    private void RenderCamera(Camera cam)
    {
        cam.SetActive();
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