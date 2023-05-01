using Newtonsoft.Json;
using Striped.Engine.BuildinComponents;
using Striped.Engine.Util;

// ReSharper disable InconsistentNaming

namespace Striped.Engine.Core;

public class Entity : RuntimeObject
{
    public int ID { get; }
    public int nextEntityID = -1;
    public int previousEntityID = -1;
    
    [JsonIgnore]public Transform? transform;
    [JsonIgnore]private InteractiveEnvironment environment;

    public Dictionary<Type,int[]> components = new Dictionary<Type, int[]>();

    
    public Entity(int id, InteractiveEnvironment environment)
    {
        ID = id;
        this.environment = environment;
    }
    
    public InteractiveEnvironment GetEnvironment() => environment;
    
    public void Destroy()
    {
        var destroyMethod = typeof(InteractiveEnvironment).GetMethod("DestroyComponent");
        foreach (var i in components.ToArray().AsSpan())
        {
            var genericDestroyMethod = destroyMethod?.MakeGenericMethod(i.Key);
            for (int j = 0; j < i.Value.Length; j++)
            {
                genericDestroyMethod?.Invoke(environment, new object?[] { i.Value[j], false });
            }
        }
    }
    
    
    public T? AddComponent<T>() where T : ComponentBase, new()
    {
        if(new T().IsExclusiveComponent) if(components.TryGetValue(typeof(T), out int[] results)) if (results.Length > 0) return null;
        return environment.AddComponent<T>(this);
    }

    public T GetComponent<T>() where T : Component<T>, new() => environment.GetComponent<T>(this);
    public Span<T> GetComponents<T>() where T : Component<T>, new() => environment.GetComponents<T>(this).ToArray().AsSpan();

    public void RemoveComponent<T>(T component) where T : ComponentBase, new() => environment.DestroyComponent<T>(component.ComponentID,true);
    

    internal void AddComponentInternal<T>(T component) where T : ComponentBase, new()
    {
        int[] componentIDs = components.ContainsKey(typeof(T)) ? components[typeof(T)] : new int[0];
        componentIDs = componentIDs.Append(component.ComponentID).ToArray();
        components[typeof(T)] = componentIDs;
    }
    
    internal void RemoveComponentInternal<T>(T component) where T : ComponentBase, new()
    {
        int[] componentIDs = components.ContainsKey(typeof(T)) ? components[typeof(T)] : new int[0];
        componentIDs = componentIDs.Where(x => x != component.ComponentID).ToArray();
        components[typeof(T)] = componentIDs;
    }

    public void Update()
    {
        //Optional game logic
    }

    public void SetTransform(Transform? transform)
    {
        this.transform = transform;
    }

    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public override void Deserialize(string data)
    {
        Entity? e = JsonConvert.DeserializeObject<Entity>(data);
        e.SetTransform(e.GetComponent<Transform>());
    }
}