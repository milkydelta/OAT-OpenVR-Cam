namespace UnityEngine;

public struct Vector3
{
    float x,y,z;
    internal Vector3 (float X, float Y, float Z)
    {
        x=X;
        y=Y;
        z=Z;
    }

    public static implicit operator System.Numerics.Vector3(Vector3 v) => new System.Numerics.Vector3(v.x, v.y, v.z);
}

public struct Quaternion
{
    float x,y,z,w;
    internal Quaternion (float X, float Y, float Z, float W)
    {
        x=X;
        y=Y;
        z=Z;
        w=W;
    }

    static internal Quaternion identity => new Quaternion(0,0,0,1);
    public static implicit operator System.Numerics.Quaternion(Quaternion q) => new System.Numerics.Quaternion(q.x, q.y, q.z, q.w);
}

public struct Mathf
{
    public static float Max(float a, float b)
    {
        return System.Math.Max(a,b);
    }
    public static float Sqrt(float f)
    {
        return (float)System.Math.Sqrt(f);
    }
}