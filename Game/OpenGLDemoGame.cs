
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Striped.Engine.BuildinComponents.Renderers.Abstract;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Core;
using Striped.Engine.InputSystem;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Serialization;
using Striped.Engine.Util;

public class OpenGLDemoGame : Game
{
    public override string Title { get; } = "OpenGL Demo";
    public override int Width { get; } = 800;
    public override int Height { get; } = 800;

    public override Type Renderer { get; } = typeof(OpenGLRenderer);

    private Entity bot;
    public Entity camera;
    public float cameraSpeed = 5f;
    public float cameraRotationSpeed = 2f;

    public float camRotX = 0;
    public float camRotY = 0;

    public override void Init()
    {
        InteractiveEnvironment? scene = GameSession.ActiveSession?.CreateEnvironment();
        
        Entity? cam = scene?.CreateEntity();
        cam?.AddComponent<Camera>();

        camera = cam!;
        
        
        //Terrain
        Entity terrain = scene.CreateEntity();
        GLMeshRenderer? terrainRenderer = terrain.AddComponent<GLMeshRenderer>();
        terrainRenderer?.SetMesh(GLModelLoader.LoadModel(Application.AssetsFolder + "/Meshes/TerrainFlat.fbx"));
        
        //Terrain Shader
        ((OpenGLRenderer)(GameSession.ActiveSession.Window.ActiveRenderer)).CreateShader(Application.AssetsFolder + "/Shaders/terrain.shader");
        GLMaterial terrainMaterial = new GLMaterial("Custom/Terrain");
        terrainRenderer?.SetMaterial(terrainMaterial);

        //Textures        
        GLTexture2D height = new GLTexture2D(Application.AssetsFolder + "/Textures/TerrainHeightmap.png");
        GLTexture2D normal = new GLTexture2D(Application.AssetsFolder + "/Textures/TerrainNormalmap.png");
        normal.SetFilterMode(TextureMinFilter.Nearest);
        height.SetFilterMode(TextureMinFilter.Linear);
        terrainMaterial.textures.Add(height);
        terrainMaterial.textures.Add(normal);
        
        terrain.transform.position = Vector3.Zero;
        terrain.transform.scale = Vector3.One * 0.6f;

        
        //Genshin bot
        Entity genshinBot = scene.CreateEntity();
        GLMeshRenderer? botRenderer = genshinBot.AddComponent<GLMeshRenderer>();
        botRenderer?.SetMesh(GLModelLoader.LoadModel(Application.AssetsFolder + "/Meshes/Body.obj"));
        botRenderer?.SetMaterial(AssetImporter.ImportAsset<GLMaterial>(Application.AssetsFolder + "/Material/defaultLit.glmaterial"));
        GLTexture2D botGLTexture = new GLTexture2D(Application.AssetsFolder + "\\Textures\\BodyTexture.png");
        botRenderer?.materal.textures.Add(botGLTexture);
        genshinBot.transform!.position = new Vector3(0, 0, -5);
        bot = genshinBot;
        
        Logger.Info("Demo ready!");
        Logger.Info("Use WASD, space, shift and the arrow keys to move around!");
    }

    public override void Update()
    {
        if (Input.GetKey(Keys.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKey(Keys.A))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(-1, 0, 0) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(Keys.W))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, 0, -1) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(Keys.S))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, 0, 1) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(Keys.D))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(1, 0, 0) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(Keys.Space))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, 1, 0) * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(Keys.LeftShift))
        {
            camera.transform.position += camera.transform.rotation * new Vector3(0, -1, 0) * cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(Keys.Left))
        {
            camRotY += cameraRotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(Keys.Right))
        {
            camRotY -= cameraRotationSpeed * Time.deltaTime;
        }
        
        if (Input.GetKey(Keys.Up))
        {
            camRotX += cameraRotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(Keys.Down))
        {
            camRotX -= cameraRotationSpeed * Time.deltaTime;
        }
        
        camera.transform.rotation = Quaternion.FromEulerAngles(new Vector3(0, camRotY, 0)) * Quaternion.FromEulerAngles(new Vector3(camRotX,0,0));

        bot.transform.rotation *= Quaternion.FromEulerAngles(0, 2f * Time.deltaTime, 0);
    }

    public override void OnApplicationQuit()
    {
        Logger.Info("Demo shut down!");
    }
}
