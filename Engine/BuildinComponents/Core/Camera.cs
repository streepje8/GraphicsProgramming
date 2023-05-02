using Striped.Engine.Core;

namespace Striped.Engine.BuildinComponents;

public class Camera : Component<Camera>
{
    public static Camera active { get; private set; }
    public void SetActive(bool active = true)
    {
        if (active) Camera.active = this;
        else Camera.active = null;
    }

    public static void SetNoneActive() { active = null; }
}