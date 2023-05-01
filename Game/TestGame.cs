using GraphicsProgramming.Engine.BuildinComponents.Renderers.Abstract;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Core;
using Striped.Engine.InputSystem;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Util;

public class TestGame : Game
{
    public override string Title { get; } = "My Epic Game";
    public override int Width { get; } = 800;
    public override int Height { get; } = 800;

    public static string AssetsFolder = "../../../Assets";

    private Entity quado;
    public override void Init()
    {
        Logger.Info("Game ready!");
        InteractiveEnvironment? scene = GameSession.ActiveSession?.CreateEnvironment();
        OpenGLShader? shader = ((OpenGLRenderer)GameSession.ActiveSession.Window.ActiveRenderer).CreateShader(AssetsFolder + "/Shaders/Standard/defaultDiffuse.shader");
        Entity cam = scene.CreateEntity();
        cam.AddComponent<Camera>();
        Entity quad = scene.CreateEntity();
        QuadRenderer qr = quad.AddComponent<QuadRenderer>();
        qr.materal = new GLMaterial("Default/Diffuse");

        quado = quad;
    }

    public override void Update()
    {
        if (Input.GeyKeyDown(Keys.Escape))
        {
            Application.Quit();
        }

        quado.transform.rotation *= Quaternion.FromEulerAngles(0,0, (float)Time.deltaTime * 1000);
        
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