using GraphicsProgramming.Engine.BuildinComponents.Renderers.Abstract;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Core;
using Striped.Engine.InputSystem;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Rendering.TemplateRenderers.Shaders;
using Striped.Engine.Serialization;
using Striped.Engine.Util;

public class TestGame : Game
{
    public override string Title { get; } = "My Epic Game";
    public override int Width { get; } = 800;
    public override int Height { get; } = 800;

    private Entity cube;

    public override void Init()
    {
        InteractiveEnvironment? scene = GameSession.ActiveSession?.CreateEnvironment();
        OpenGLShader? shader = ((OpenGLRenderer)GameSession.ActiveSession.Window.ActiveRenderer).CreateShader(Application.AssetsFolder + "/Shaders/Standard/defaultDiffuse.shader");

        Entity cam = scene.CreateEntity();
        cam.AddComponent<Camera>();
        
        // Entity quad = scene.CreateEntity();
        // quad.transform.position = new Vector3(0, 0, -3);
        // QuadRenderer qr = quad.AddComponent<QuadRenderer>();
        // string material = @"{  
        // ""shaderName"": ""Default/Diffuse"",
        // ""shader"": ""../../../Assets/Shaders/Standard/defaultDiffuse.shader"",
        // ""textures"": [
        // ""C:\\Users\\streep\\Desktop\\TestImage.jpg""
        //     ]    }    ";
        // qr.SetMaterial(Deserializer.Deserialize<GLMaterial>(material));

        Entity cube = scene.CreateEntity();
        MeshRenderer mr = cube.AddComponent<MeshRenderer>();
        mr.SetMesh(ObjLoader.LoadDefaultMesh(DefaultMesh.Cube));
        mr.SetMaterial(new GLMaterial("Default/Diffuse"));

        Texture2D cat = new Texture2D("C:\\Users\\streep\\Desktop\\TestImage.jpg");
        mr.materal.textures.Add(cat);
        
        cube.transform.position = new Vector3(0, 0, -5);
        this.cube = cube;
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

        cube.transform.rotation *= Quaternion.FromEulerAngles(0, (float)(2f * Time.deltaTime), (float)(2f * Time.deltaTime));
    }

    public override void OnApplicationQuit()
    {
        Logger.Info("Game shut down!");
    }
}