using System.Runtime.CompilerServices;

namespace Striped.Engine.Serialization;

public class Deserializer
{
    public static T Deserialize<T>(string data) where T : SerializeableObject
    {
        T obj = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        obj.Deserialize(data);
        return obj;
    }
}