using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Striped.Engine.Rendering.Core;

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

    public override void OnLoad()
    {
        GL.ClearColor(clearColor.X,clearColor.Y,clearColor.Z,clearColor.W);
    }
    
    public override void OnRenderFrame()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    public override void OnResize(EngineWindow engineWindow, int width, int height)
    {
        GL.Viewport(0,0,width,height);
    }
}