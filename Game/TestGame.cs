using OpenTK.Windowing.GraphicsLibraryFramework;
using Striped.Engine.Core;
using Striped.Engine.InputSystem;
using Striped.Engine.Util;

public class TestGame : Game
{
    public override string Title { get; } = "My Epic Game";
    public override int Width { get; } = 800;
    public override int Height { get; } = 400;

    public override void Init()
    {
        Logger.Info("Simple log");
        Logger.Info("Advanced log", Logger.GatherContext());
    }

    public override void Update()
    {
        if (Input.GeyKeyDown(Keys.Escape))
        {
            Application.Quit();
        }
    }

    public override void OnApplicationQuit()
    {
        
    }
}