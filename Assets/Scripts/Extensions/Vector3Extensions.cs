using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 UnclampedLerp(this Vector3 l, Vector3 r, float t)
    {
        return l + t * (r - l);
    }

    public static Vector3 Min(this Vector3 v, Vector3 min)
    {
        return new Vector3(Mathf.Min(v.x, min.x),
                           Mathf.Min(v.y, min.y),
                           Mathf.Min(v.z, min.z));
    }

    public static Vector3 Max(this Vector3 v, Vector3 max)
    {
        return new Vector3(Mathf.Max(v.x, max.x),
                           Mathf.Max(v.y, max.y),
                           Mathf.Max(v.z, max.z));
    }

    public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(v.x, min.x, max.x),
                           Mathf.Clamp(v.y, min.y, max.y),
                           Mathf.Clamp(v.z, min.z, max.z));
    }
}
