namespace Striped.Engine.Serialization;

public class SerializeableObject
{
    public virtual string Serialize() => "";

    public virtual void Deserialize(string data) { }
}