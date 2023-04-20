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

    private Window* window;
    private Vector2i size;
    
    public EngineWindow(Renderer renderer, int width, int height, string title) : base(new NativeWindowSettings()
    {
        Size = new Vector2i(width,height),
        Title = title
    })
    {
        ActiveRenderer = renderer;
        size = new Vector2i(width, height);
        Initialize();
        Application.SetWindow(this);
    }

    private GLFWCallbacks.WindowSizeCallback resizeCallback;

    private void OnLoad()
    {
        ActiveRenderer.OnLoad();
        resizeCallback = (window1, width, height) => OnResize(window1,width, height); 
        GLFW.SetWindowSizeCallback(window,resizeCallback);
    }

    private double frameTimer = 0f;
    private double updateTimer = 0f;

    public void Run()
    {
        CenterWindow();
        ActiveRenderer.OnResize(this,size.X,size.Y);
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
        GameSession.ActiveSession?.Game?.Update();
        if (GameSession.ActiveSession?.LoadedEnvironments != null)
            foreach (var activeSessionLoadedEnvironment in GameSession.ActiveSession.LoadedEnvironments)
            {
                Span<Entity> entites = activeSessionLoadedEnvironment.GetEntitySpan();
                for (int i = 0; i < entites.Length;)
                {
                    Entity entity = entites[i];
                    entity.Update();
                    i = entity.nextEntityID;
                    if (i == -1) break;
                }
                foreach (var componentCollection in activeSessionLoadedEnvironment.allComponents.Values)
                {
                    foreach(ComponentBase component in componentCollection.Span)
                    {
                        component.Update();
                    }
                }
            }
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
        GameSession.ActiveSession?.UnloadAllEnvironments();
        GameSession.ActiveSession?.Game?.OnApplicationQuit();
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