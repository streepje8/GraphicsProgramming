using Newtonsoft.Json;
using Striped.Engine.BuildinComponents;

namespace Striped.Engine.Core;

public class ComponentBase
{
    public int EntityID { get; private set; }
    public int ComponentID { get; private set; }
    public int nextComponentID = -1;
    public int previousComponentID = -1;

    

    public Transform? transform;
    public Entity? entity;
    public void SetID(int id) => ComponentID = id;

    
    public bool isActive = true;
    [JsonIgnore] public virtual bool IsExclusiveComponent { get; } = false;
    public void SetEntity(Entity? e)
    {
        entity = e;
        transform = e?.transform;
        if (e != null) EntityID = e.ID;
    }
    
    
    public virtual void OnCreate() { }
    public virtual void Update() { }
    public virtual void OnDestroy() { }
}