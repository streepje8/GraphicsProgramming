namespace Striped.Engine.Core;

public class ComponentBase
{
    public virtual void OnCreate() { }
    public virtual void Update() { }
    public virtual void OnDestroy() { }
}