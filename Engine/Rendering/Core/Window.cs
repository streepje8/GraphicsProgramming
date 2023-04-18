using GraphicsProgramming.Engine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GraphicsProgramming;

public class Window : GameWindow
{
    public Renderer ActiveRenderer { get; private set; }

    private Game activeGame;
    public Game ActiveGame
    {
        get => activeGame;
        set
        {
            if (activeGame == null) activeGame = value;
            else throw new Exception("You are not allowed to overwrite the active game!");
        }
    }

    public Window(Renderer renderer, int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings() { Size = (width, height), Title = title })
    {
        ActiveRenderer = renderer;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(1.0f,0.0f,0.0f,0.0f);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Time.Tick();
        base.OnUpdateFrame(args);
        activeGame?.Update();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        
        
        Context.SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0,0,e.Width,e.Height);
    }

    public void Clean()
    {
        activeGame?.OnApplicationQuit();
    }

    public void Initialize()
    {
        activeGame?.Init();
    }
}