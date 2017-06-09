using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Min(this Vector2 v, Vector2 min)
    {
        return new Vector2(Mathf.Min(v.x, min.x),
                           Mathf.Min(v.y, min.y));
    }

    public static Vector2 Max(this Vector2 v, Vector2 max)
    {
        return new Vector2(Mathf.Max(v.x, max.x),
                           Mathf.Max(v.y, max.y));
    }

    public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(v.x, min.x, max.x),
                           Mathf.Clamp(v.y, min.y, max.y));
    }

    public static float PerpDot(this Vector2 l, Vector2 r)
    {
        return l.x * r.y - l.y * r.x;
    }
}
