using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 UnclampedLerp(this Vector3 l, Vector3 r, float t)
    {
        return l + t * (r - l);
    }

    public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(v.x, min.x, max.x),
                           Mathf.Clamp(v.y, min.y, max.y),
                           Mathf.Clamp(v.z, min.z, max.z));
    }
}
