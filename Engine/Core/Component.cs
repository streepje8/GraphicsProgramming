// ReSharper disable InconsistentNaming
// ReSharper disable StaticMemberInGenericType

namespace Striped.Engine.Core;

public class Component<T> : ComponentBase
{
    public static Memory<T> instances = new Memory<T>();
    public static readonly Stack<int> AvailbleInstanceIndexes = new Stack<int>();
}