using OpenTK.Mathematics;
using Striped.Engine.Core;

namespace Striped.Engine.BuildinComponents;

public class Camera : Component<Camera>
{
    public static Camera Active { get; private set; }

    private int fov = 60;
    private float near = 0.1f;
    private float far = 1000f;
    private float aspect = 1f;

    public int FOV {get => fov; set {fov = value; UpdateProjectionMatrix(GameSession.ActiveSession.Window.Size);}}
    public float Near { get => near; set { near = value; UpdateProjectionMatrix(GameSession.ActiveSession.Window.Size); } }
    public float Far { get => far; set { far = value; UpdateProjectionMatrix(GameSession.ActiveSession.Window.Size); } }
    public float Aspect { get => aspect; }
    
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
            return (Matrix4.CreateFromQuaternion(transform.rotation) *
                   Matrix4.CreateTranslation(transform.position)).Inverted(); //Matrix4.CreateScale(Vector3.One)
        }
    }

    public void UpdateProjectionMatrix(Vector2i size)
    {
        aspect = size.X / (float)size.Y;
        projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), size.X / (float)size.Y, near, far);
    }

    public void SetActive(bool active = true)
    {
        if (active) Camera.Active = this;
        else Camera.Active = null;
    }

    public static void SetNoneActive() { Active = null; }
}