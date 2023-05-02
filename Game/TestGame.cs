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

    private Entity quado;
    
    public override void Init()
    {
        InteractiveEnvironment? scene = GameSession.ActiveSession?.CreateEnvironment();
        OpenGLShader? shader = ((OpenGLRenderer)GameSession.ActiveSession.Window.ActiveRenderer).CreateShader(Application.AssetsFolder + "/Shaders/Standard/defaultDiffuse.shader");
        
        
        Entity cam = scene.CreateEntity();
        cam.AddComponent<Camera>();
        Entity quad = scene.CreateEntity();
        QuadRenderer qr = quad.AddComponent<QuadRenderer>();
        qr.SetMaterial(new GLMaterial("Default/Diffuse"));
        Texture2D cat = new Texture2D("C:\\Users\\streep\\Desktop\\TestImage.jpg");
        qr.materal.textures.Add(cat);

        quado = quad;
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
            OpenGLRenderer.ClearColor = new Vector4(1, 0, 0, 1);
        }
        if (Input.GeyKeyDown(Keys.S))
        {
            OpenGLRenderer.ClearColor = new Vector4(0, 1, 0, 1);
        }
        if (Input.GeyKeyDown(Keys.D))
        {
            OpenGLRenderer.ClearColor = new Vector4(0, 0, 1, 1);
        }

        quado.transform.rotation *= Quaternion.FromEulerAngles(0, 0, (float)(5f * Time.deltaTime));
    }

    public override void OnApplicationQuit()
    {
        Logger.Info("Game shut down!");
    }
}