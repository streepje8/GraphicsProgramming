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
    public Entity camera;
    public float cameraSpeed = 5f;
    public float cameraRotationSpeed = 2f;

    public float camRotX = 0;
    public float camRotY = 0;

    public override void Init()
    {
        InteractiveEnvironment? scene = GameSession.ActiveSession?.CreateEnvironment();
        
        Entity cam = scene.CreateEntity();
        cam.AddComponent<Camera>();

        camera = cam;
        
        Entity cube = scene.CreateEntity();
        MeshRenderer mr = cube.AddComponent<MeshRenderer>();
        mr.SetMesh(ModelLoader.LoadModel(Application.AssetsFolder + "/Meshes/Body.obj"));
        mr.SetMaterial(AssetImporter.ImportAsset<GLMaterial>(Application.AssetsFolder + "/Material/defaultDiffuse.glmaterial")); //new GLMaterial("Default/Diffuse")

        Texture2D cat = new Texture2D("L:\\MyProjects\\GraphicsProgramming\\Assets\\Meshes\\BodyTexture.png");
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
            camera.transform.position += camera.transform.rotation * new Vector3(-1, 0, 0) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GeyKeyDown(Keys.W))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, 0, -1) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GeyKeyDown(Keys.S))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, 0, 1) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GeyKeyDown(Keys.D))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(1, 0, 0) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GeyKeyDown(Keys.Space))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, 1, 0) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GeyKeyDown(Keys.LeftShift))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, -1, 0) * cameraSpeed * Time.deltaTime;
        }

        if (Input.GeyKey(Keys.Left))
        {
            camRotY += cameraRotationSpeed * Time.deltaTime;
        }
        if (Input.GeyKey(Keys.Right))
        {
            camRotY -= cameraRotationSpeed * Time.deltaTime;
        }
        
        if (Input.GeyKey(Keys.Up))
        {
            camRotX += cameraRotationSpeed * Time.deltaTime;
        }
        if (Input.GeyKey(Keys.Down))
        {
            camRotX -= cameraRotationSpeed * Time.deltaTime;
        }
        
        camera.transform.rotation = Quaternion.FromEulerAngles(new Vector3(0, camRotY, 0)) * Quaternion.FromEulerAngles(new Vector3(camRotX,0,0));

        cube.transform.rotation *= Quaternion.FromEulerAngles(0, (float)(2f * Time.deltaTime), 0);
    }

    public override void OnApplicationQuit()
    {
        Logger.Info("Game shut down!");
    }
}