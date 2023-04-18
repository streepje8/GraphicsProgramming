using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Striped.Engine.Core;
using Striped.Engine.InputSystem;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Util;

public class TestGame : Game
{
    public override string Title { get; } = "My Epic Game";
    public override int Width { get; } = 800;
    public override int Height { get; } = 400;

    public override void Init()
    {
        Logger.Info("Game ready!");
    }

    public override void Update()
    {
        if (Input.GeyKeyDown(Keys.Escape))
        {
            Application.Quit();
        }

        if (Input.GeyKeyDown(Keys.A))
        {
            OpenGLRenderer.ClearColor = new Vector4(1, 0, 0, (float)Time.deltaTime);
        }
        if (Input.GeyKeyDown(Keys.S))
        {
            OpenGLRenderer.ClearColor = new Vector4(0, 1, 0, (float)Time.deltaTime);
        }
        if (Input.GeyKeyDown(Keys.D))
        {
            OpenGLRenderer.ClearColor = new Vector4(0, 0, 1, (float)Time.deltaTime);
        }
    }

    public override void OnApplicationQuit()
    {
        Logger.Info("Game shut down!");
    }
}