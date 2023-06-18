using Striped.Engine.Rendering.TemplateRenderers;

namespace Striped.Engine.Core;

public class Game
{
    public virtual string Title { get; } = "Striped Window";
    public virtual int Width { get; } = 800;
    public virtual int Height { get; } = 400;
    
    public virtual Type Renderer { get; } = typeof(OpenGLRenderer);
    public virtual void Init() { }
    public virtual void Update() { }
    public virtual void OnApplicationQuit() { }
}