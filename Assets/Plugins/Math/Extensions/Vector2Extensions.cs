using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 LerpUnclamped(this Vector2 l, Vector2 r, float t)
    {
        return l + t * (r - l);
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

    //TODO: write this
    public static Vector2 RotateTowards(Vector2 current, Vector2 target, float maxRadiansDelta, float maxMagnitudeDelta)
    {
        //get angle between

        //clamp to max delta

        //rotate

        return Vector2.zero;
    }
}