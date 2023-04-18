using OpenTK.Graphics.OpenGL4;
using Striped.Engine.Rendering.Core;

namespace Striped.Engine.Rendering.TemplateRenderers;

public class OpenGLRenderer : Renderer
{
    public override void OnLoad()
    {
        GL.ClearColor(1.0f,0.0f,0.0f,0.0f);
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