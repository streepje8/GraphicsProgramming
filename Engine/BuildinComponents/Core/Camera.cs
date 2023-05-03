using OpenTK.Mathematics;
using Striped.Engine.Core;

namespace Striped.Engine.BuildinComponents;

public class Camera : Component<Camera>
{
    public static Camera Active { get; private set; }

    private int fov = 60;
    private float near = 0.1f;
    private float far = 1000f;

    public int FOV {get => fov; set {fov = value; UpdateProjectionMatrix();}}
    public float Near { get => near; set { near = value; UpdateProjectionMatrix(); } }
    public float Far { get => far; set { far = value; UpdateProjectionMatrix(); } }
    
    private Matrix4 projectionMatrix = Matrix4.Identity;
    public Matrix4 ProjectionMatrix
    {
        get
        {
            return projectionMatrix;
        }
    }

    public Matrix4 ViewMatrix
    {
        get
        {
            return (Matrix4.CreateScale(Vector3.One) * Matrix4.CreateFromQuaternion(transform.rotation) *
                   Matrix4.CreateTranslation(transform.position)).Inverted();
        }
    }

    public void UpdateProjectionMatrix() => projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), GameSession.ActiveSession!.Window.Size.X / (float)GameSession.ActiveSession!.Window.Size.X, near, far);

    public void SetActive(bool active = true)
    {
        if (active) Camera.Active = this;
        else Camera.Active = null;
    }

    public static void SetNoneActive() { Active = null; }
}