using UnityEngine;

public static class RandomExtensions
{
    public static bool GetBool()
    {
        return Random.value > 0.5f;
    }

    public static Vector2 GetVector2()
    {
        return new Vector2(Random.value, Random.value);
    }

    public static Vector3 GetVector3()
    {
        return new Vector3(Random.value, Random.value, Random.value);
    }

    public static Vector4 GetVector4()
    {
        return new Vector4(Random.value, Random.value, Random.value, Random.value);
    }

    public static Vector2 GetVector2InRange(Vector2 min, Vector2 max)
    {
        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }

    public static Vector3 GetVector3InRange(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x),
                           Random.Range(min.y, max.y),
                           Random.Range(min.z, max.z));
    }

    public static Vector4 GetVector4InRange(Vector4 min, Vector4 max)
    {
        return new Vector4(Random.Range(min.x, max.x),
                           Random.Range(min.y, max.y),
                           Random.Range(min.z, max.z),
                           Random.Range(min.w, max.w));
    }
}