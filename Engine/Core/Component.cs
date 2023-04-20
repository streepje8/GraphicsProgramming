using Newtonsoft.Json;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Core;

namespace Striped.Engine.Core;

public class Component<T> : ComponentBase
{
    public static Memory<T> instances = new Memory<T>();
    public static Stack<int> availbleInstanceIndexes = new Stack<int>();
    public int EntityID { get; private set; }
    public int ComponentID { get; private set; }
    public int nextComponentID = -1;
    public int previousComponentID = -1;

    public Transform? transform;
    public Entity? entity;
    public void SetID(int id) => ComponentID = id;

    [JsonIgnore] public virtual bool IsExclusiveComponent { get; } = false;
    public void SetEntity(Entity? e)
    {
        entity = e;
        transform = e.transform;
        EntityID = e.ID;
    }
}