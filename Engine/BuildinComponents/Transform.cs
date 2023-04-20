using OpenTK.Mathematics;
using Striped.Engine.Core;
using Striped.Engine.Util;

namespace Striped.Engine.BuildinComponents;

public class Transform : Component<Transform>
{
    public override bool IsExclusiveComponent { get; } = true;
    public Vector3 position = new Vector3(0,0,0);
    public Quaternion rotation = Quaternion.Identity;
    public Vector3 scale = new Vector3(1,1,1);

    public Matrix4 TRS()
    {
        return  Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(rotation) *
                Matrix4.CreateTranslation(position);
    }
}