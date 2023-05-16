using OpenTK.Mathematics;
using Striped.Engine.Core;

namespace Striped.Engine.BuildinComponents;

public class Transform : Component<Transform>
{
    public override bool IsExclusiveComponent { get; } = true;
    public Vector3 position = new Vector3(0,0,0);
    public Quaternion rotation = Quaternion.Identity;
    public Vector3 scale = new Vector3(1,1,1);
    public Transform parent { get; private set; } = null;

    public void SetParent(Transform parent)
    {
        if (parent.parent != this) this.parent = parent;
    }

    public Matrix4 TRS()
    {
        return  parent == null ?  LocalTRS()
            : parent.TRS() * LocalTRS();
    }

    public Matrix4 LocalTRS() => Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(rotation) *
                                 Matrix4.CreateTranslation(position);
}