using Striped.Engine.BuildinComponents;

namespace Striped.Engine.Core;

public class InteractiveEnvironment
{
    public Memory<Entity> entities;
    public Dictionary<Type,Memory<ComponentBase>> allComponents = new Dictionary<Type, Memory<ComponentBase>>();
    private Stack<int> availbleEntityIndexes = new Stack<int>();
    
    public InteractiveEnvironment()
    {
        entities = new Memory<Entity>();
    }

    
    public Entity CreateEntity(bool addTransform = true)
    {
        Entity e;
        if (availbleEntityIndexes.Count > 0)
        {
            int index = availbleEntityIndexes.Pop();
            e = new Entity(index,this);
            e.previousEntityID = index - 1;
            e.nextEntityID = index + 1;
            if(e.nextEntityID > entities.Span.Length) e.nextEntityID = -1;
            entities.Span[index] = e;
        }
        else
        {
            Entity[] currentArr = entities.Span.ToArray();
            e = new Entity(currentArr.Length,this);
            e.previousEntityID = currentArr.Length - 1;
            e.nextEntityID = -1;
            if(e.previousEntityID >= 0)currentArr[e.previousEntityID].nextEntityID = e.ID;
            entities = new Memory<Entity>(currentArr.Concat(new[] { e }).ToArray());
        }
        e.SetTransform(e.AddComponent<Transform>());
        return e;
    }
    
    public Entity GetEntity(int id) => entities.Span[id];
    public Span<Entity> GetEntitySpan() => entities.Span;
    public Span<Entity> GetEntitySpan(int start, int end) => entities.Span.Slice(start, end);

    public Span<Entity> GetEntitySpanNoEmpty()
    {
        List<Entity> allEntities = entities.Span.ToArray().ToList();
        List<int> toDelete = availbleEntityIndexes.ToList().OrderByDescending(x => x).ToList();
        foreach (var i in toDelete)
        {
            allEntities.RemoveAt(i);
        }
        return allEntities.ToArray().AsSpan();
    }

    public void DestroyEntity(Entity e)
    {
        e.Destroy();
        availbleEntityIndexes.Push(e.ID);
        if(e.nextEntityID >= 0)entities.Span[e.nextEntityID].previousEntityID = e.previousEntityID;
        if(e.previousEntityID >= 0)entities.Span[e.previousEntityID].nextEntityID = e.nextEntityID;
        entities.Span[e.ID] = null;
    }
    
    public T AddComponent<T>(Entity? e) where T : ComponentBase, new()
    {
        T c = new T();
        if (Component<T>.AvailbleInstanceIndexes.Count > 0)
        {
            int index = Component<T>.AvailbleInstanceIndexes.Pop();
            c.SetID(index);
            c.nextComponentID = index + 1;
            c.previousComponentID = index - 1;
            if(c.nextComponentID > Component<T>.instances.Span.Length) c.nextComponentID = -1;
            Component<T>.instances.Span[index] = c;
        }
        else
        {
            T[] currentArr = Component<T>.instances.Span.ToArray();
            c.SetID(currentArr.Length);
            c.nextComponentID = -1;
            c.previousComponentID = currentArr.Length - 1;
            if(c.previousComponentID >= 0)currentArr[c.previousComponentID].nextComponentID = c.ComponentID;
            Component<T>.instances = new Memory<T>(currentArr.Concat<T>(new[] { c }).ToArray());
        }
        e.AddComponentInternal(c);
        c.SetEntity(e);
        c.OnCreate();
        if(allComponents.ContainsKey(typeof(T))) allComponents.Remove(typeof(T));
        allComponents.Add(typeof(T), new Memory<ComponentBase>(Component<T>.instances.ToArray().Cast<ComponentBase>().ToArray()));
        AddInheritanceReferencesRecursive(c);
        return c;
    }

    private void AddInheritanceReferencesRecursive<T>(T component) where T : ComponentBase, new()
    {
        if (typeof(T).BaseType != typeof(Component<T>))
        {
            if (typeof(T).BaseType != null)
            {
                Type baseType = typeof(T).BaseType;
                Memory<ComponentBase> currentComps = new Memory<ComponentBase>();
                if (allComponents.TryGetValue(baseType, out Memory<ComponentBase> foundComps)) currentComps = foundComps;
                ComponentBase[] newComps = currentComps.Span.ToArray().Append(component).ToArray();
                if (allComponents.ContainsKey(baseType)) allComponents.Remove(baseType);
                allComponents.Add(baseType, new Memory<ComponentBase>(newComps));
                AddInheritanceReferencesRecursive(component,baseType);
            }
        }
    }
    
    private void AddInheritanceReferencesRecursive(object component, Type baseType)
    {
        if (baseType.BaseType != typeof(Component<>).MakeGenericType(baseType))
        {
            if (baseType.BaseType != null)
            {
                Type baseBaseType = baseType.BaseType;
                Memory<ComponentBase> currentComps = new Memory<ComponentBase>();
                if (allComponents.TryGetValue(baseBaseType, out Memory<ComponentBase> foundComps)) currentComps = foundComps;
                ComponentBase[] newComps = currentComps.Span.ToArray().Append((ComponentBase)component).ToArray();
                if (allComponents.ContainsKey(baseType)) allComponents.Remove(baseType);
                allComponents.Add(baseType, new Memory<ComponentBase>(newComps));
                AddInheritanceReferencesRecursive(component,baseType);
            }
        }
    }

    public void DestroyComponent<T>(int componentID, bool alertEntity = true) where T : Component<T>, new()
    {
        Component<T>.AvailbleInstanceIndexes.Push(componentID);
        Component<T>.instances.Span[componentID].OnDestroy();
        if (Component<T>.instances.Span[componentID].nextComponentID >= 0)
            Component<T>.instances.Span[Component<T>.instances.Span[componentID].nextComponentID]
                .previousComponentID = Component<T>.instances.Span[componentID].previousComponentID;
        if (Component<T>.instances.Span[componentID].previousComponentID >= 0)
            Component<T>.instances.Span[Component<T>.instances.Span[componentID].previousComponentID].nextComponentID = Component<T>.instances.Span[componentID].nextComponentID;
        Component<T>.instances.Span[componentID] = null;
        if (alertEntity)
        {
            Entity e = entities.Span[Component<T>.instances.Span[componentID].EntityID];
            e.RemoveComponentInternal(Component<T>.instances.Span[componentID]);
        }
        if(allComponents.ContainsKey(typeof(T))) allComponents.Remove(typeof(T));
        allComponents.Add(typeof(T), new Memory<ComponentBase>(Component<T>.instances.ToArray().Cast<ComponentBase>().ToArray()));
    }

    public void Destroy()
    {
        foreach (var e in entities.Span)
        {
            e.Destroy();
        }
    }

    public T GetComponent<T>(Entity entity) where T : Component<T>, new()
    {
        if (entity.components.TryGetValue(typeof(T), out int[] results))
        {
            if (results.Length > 0)
            {
                return Component<T>.instances.Span[results[0]];
            }
            return null;
        }
        return null;
    }
    
    public List<T> GetComponents<T>(Entity entity) where T : Component<T>, new()
    {
        if (entity.components.TryGetValue(typeof(T), out int[] results))
        {
            if (results.Length > 0)
            {
                List<T> foundComponents = new List<T>();
                foreach (var component in Component<T>.instances.Span)
                {
                    if(results.Contains(component.ComponentID)) foundComponents.Add(component);
                }
                return foundComponents;
            }
            return null;
        }
        return null;
    }

    public static Span<T> GetAllComponents<T>() where T : Component<T>, new()
    {
        return Component<T>.instances.Span;
    }
}