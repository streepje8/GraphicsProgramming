using GraphicsProgramming.Engine.Rendering.RayTracing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Striped.Engine.BuildinComponents;
using Striped.Engine.InputSystem;
using Striped.Engine.Util;

namespace Striped.Engine.Core;

public class RaytracingDemo : Game
{
    public override string Title { get; } = "Raytracing Demo";
    public override int Width { get; } = 800;
    public override int Height { get; } = 800;

    public override Type Renderer { get; } = typeof(RTRenderer);
    
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
    }

    public override void OnApplicationQuit()
    {
        Logger.Info("Demo shut down!");
    }
}
