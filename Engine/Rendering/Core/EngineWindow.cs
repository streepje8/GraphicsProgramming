using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Striped.Engine.Core;
using Striped.Engine.InputSystem;
using Striped.Engine.Util;

namespace Striped.Engine.Rendering.Core;

public unsafe class EngineWindow : NativeWindow
{
    public Renderer ActiveRenderer { get; private set; }

    private float timePerFrame = 0.01f;
    public int FPS
    {
        set
        {
            timePerFrame = 1 / (float)value;
        }
    }

    private float timePerUpdate = 0.001666666666666f;
    public int UPS
    {
        set
        {
            timePerUpdate = 1 / (float)value;
        }
    }

    private Game? activeGame;
    public Game? ActiveGame
    {
        get => activeGame;
        set
        {
            if (activeGame == null) activeGame = value;
            else throw new Exception("You are not allowed to overwrite the active game!");
        }
    }

    private Window* window;

    public EngineWindow(Renderer renderer, int width, int height, string title) : base(new NativeWindowSettings()
    {
        Size = new Vector2i(width,height),
        Title = title
    })
    {
        ActiveRenderer = renderer;
        Initialize();
        Application.SetWindow(this);
    }

    private void OnLoad()
    {
        ActiveRenderer.OnLoad();
        GLFW.SetWindowSizeCallback(window,OnResize);
    }

    private double frameTimer = 0f;
    private double updateTimer = 0f;

    public void Run()
    {
        CenterWindow();
        while (!GLFW.WindowShouldClose(window))
        {
            Time.Tick();
            frameTimer += Time.deltaTime;
            updateTimer += Time.deltaTime;
            if (updateTimer > timePerUpdate)
            {
                Input.UpdateKeyboardState(KeyboardState);
                OnUpdateFrame();
                updateTimer = 0d;
            }
            if (frameTimer > timePerFrame)
            {
                OnRenderFrame();
                GLFW.SwapBuffers(window);
                frameTimer = 0d;
            }
            double timeUntilNextFrame = Math.Min(timePerUpdate - updateTimer, timePerFrame - frameTimer);
            if (timeUntilNextFrame > 0.0)Thread.Sleep((int) Math.Floor(timeUntilNextFrame * 1000.0));
            GLFW.PollEvents();
        }
    }

    private void OnUpdateFrame()
    {
        Time.Tick();
        activeGame?.Update();
    }

    private void OnRenderFrame()
    {
        ActiveRenderer.OnRenderFrame();
    }

    private void OnResize(Window* window1, int width, int height)
    {
        ActiveRenderer.OnResize(this, width, height);
    }

    public void Clean()
    {
        activeGame?.OnApplicationQuit();
        ActiveRenderer.OnUnLoad();
    }

    public void Initialize()
    {
        GLFW.Init();
        window = WindowPtr;
        Context?.MakeCurrent();
        OnLoad();
    }
}