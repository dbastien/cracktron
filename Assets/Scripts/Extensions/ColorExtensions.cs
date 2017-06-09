using UnityEngine;

public static class ColorExtensions
{
    public static Color LerpUnclamped(this Color l, Color r, float t)
    {
        return l + t * (r - l);
    }

    public static Color PremultiplyAlpha(this Color v)
    {
        return new Color(v.r * v.a, v.g * v.a, v.b * v.a, v.a);
    }
}
