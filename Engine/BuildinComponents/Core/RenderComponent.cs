using Striped.Engine.Core;

namespace Striped.Engine.BuildinComponents;

public abstract class RenderComponent : Component<RenderComponent>
{
    public abstract void OnRender(Camera cam);
}