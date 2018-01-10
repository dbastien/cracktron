using UnityEngine;

public static class Vector3IntExtensions
{
    public static readonly Vector3Int forward = new Vector3Int(0, 0, 1);
    public static readonly Vector3Int back = new Vector3Int(0, 0, -1);

    public static int Dot(Vector3Int l, Vector3Int r)
    {
        return l.x * r.x + l.y * r.y + l.z * r.z;
    }

    public static int DotOne(this Vector3Int v)
    {
        return v.x * v.y * v.z;
    }

    public static Vector3Int Cross(Vector3Int l, Vector3Int r)
    {
        return new Vector3Int((l.y * r.z) - (l.z * r.y), 
                              (l.z * r.x) - (l.x * r.z),
                              (l.x * r.y) - (l.y * r.x));
    }

    public static Vector3Int Min(Vector3Int l, Vector3Int r)
    {
        return new Vector3Int(Mathf.Min(l.x, r.x), Mathf.Min(l.y, r.y), Mathf.Min(l.z, r.z));
    }

    public static Vector3Int Max(Vector3Int l, Vector3Int r)
    {
        return new Vector3Int(Mathf.Max(l.x, r.x), Mathf.Max(l.y, r.y), Mathf.Max(l.z, r.z));
    }

    public static Vector3Int Clamp(Vector3Int v, Vector3Int min, Vector3Int max)
    {
        return new Vector3Int(Mathf.Clamp(v.x, min.x, max.x),
                              Mathf.Clamp(v.y, min.y, max.y),
                              Mathf.Clamp(v.z, min.z, max.z));
    }

    public static Vector3Int ClosestPowerOfTwo(Vector3Int v)
    {
        return new Vector3Int(Mathf.ClosestPowerOfTwo(v.x),
                              Mathf.ClosestPowerOfTwo(v.y),
                              Mathf.ClosestPowerOfTwo(v.z));
    }

    public static Vector3Int PowerOfTwoGreaterThanOrEqualTo(Vector3Int v)
    {
        return new Vector3Int(v.x.PowerOfTwoGreaterThanOrEqualTo(),
                              v.y.PowerOfTwoGreaterThanOrEqualTo(),
                              v.z.PowerOfTwoGreaterThanOrEqualTo());
    }

    public static int CubicToLinearIndex(Vector3Int v, Vector3Int size)
    {
        return (v.x) +
               (v.y * size.x) +
               (v.z * size.x * size.y);
    }

    public static Vector3Int LinearToCubicIndex(int v, Vector3Int size)
    {
        return new Vector3Int(v % size.x,
                             (v / size.x) % size.y,
                             (v / (size.x * size.y)) % size.z);
    }
}
