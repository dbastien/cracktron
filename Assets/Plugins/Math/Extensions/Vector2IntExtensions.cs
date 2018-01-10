using UnityEngine;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

public static class Vector2IntExtensions
{
    public static int Dot(Vector2Int l, Vector2Int r)
    {
        return l.x * r.x + l.y * r.y;
    }

    public static int DotOne(this Vector2Int v)
    {
        return v.x * v.y;
    }

    public static int PerpDot(Vector2Int l, Vector2Int r)
    {
        return l.x * r.y - l.y * r.x;
    }

    public static Vector2Int Min(Vector2Int l, Vector2Int r)
    {
        return new Vector2Int(Mathf.Min(l.x, r.x), Mathf.Min(l.y, r.y));
    }

    public static Vector2Int Max(Vector2Int l, Vector2Int r)
    {
        return new Vector2Int(Mathf.Max(l.x, r.x), Mathf.Max(l.y, r.y));
    }

    public static Vector2Int Clamp(Vector2Int v, Vector2Int min, Vector2Int max)
    {
        return new Vector2Int(Mathf.Clamp(v.x, min.x, max.x),
                              Mathf.Clamp(v.y, min.y, max.y));
    }

    public static Vector2Int ClosestPowerOfTwo(Vector2Int v)
    {
        return new Vector2Int(Mathf.ClosestPowerOfTwo(v.x), Mathf.ClosestPowerOfTwo(v.y));
    }
}