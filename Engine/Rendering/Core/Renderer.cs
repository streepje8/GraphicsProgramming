namespace Striped.Engine.Rendering.Core;

public abstract class Renderer
{
    public abstract void OnRenderFrame();
    public virtual void OnLoad() { }
    public virtual void OnUnLoad() { }
    public virtual void OnUpdate() { }
    public virtual void OnResize(EngineWindow engineWindow, int width, int height) { }
}