using OpenTK.Mathematics;
using Striped.Engine.Core;

namespace Striped.Engine.BuildinComponents;

public class Transform : Component<Transform>
{
    public override bool IsExclusiveComponent { get; } = true;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}