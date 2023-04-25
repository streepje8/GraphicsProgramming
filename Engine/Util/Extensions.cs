using OpenTK.Mathematics;

namespace Striped.Engine.Util;

public static class Extensions
{
    public static Quaternion UnityMultiply(this Quaternion lhs, Quaternion rhs)
    {
        return new Quaternion(
            lhs.W * rhs.X + lhs.X * rhs.W + lhs.Y * rhs.Z - lhs.Z * rhs.Y,
            lhs.W * rhs.Y + lhs.Y * rhs.W + lhs.Z * rhs.X - lhs.X * rhs.Z,
            lhs.W * rhs.Z + lhs.Z * rhs.W + lhs.X * rhs.Y - lhs.Y * rhs.X,
            lhs.W * rhs.W - lhs.X * rhs.X - lhs.Y * rhs.Y - lhs.Z * rhs.Z);
    }
}