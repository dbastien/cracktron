using UnityEngine;

public static class Matrix4x4Extensions
{
    /// <summary>
    /// Creates a matrix for transforming an RGB color in HSV space
    /// </summary>
    /// <param name="h">Hue shift in degrees</param>
    /// <param name="s">Saturation multiplier</param>
    /// <param name="v">Value multiplier</param>
    /// <returns>Matrix for transforming an RGB color in HSV space</returns>
    public static Matrix4x4 CreateHSVTransform(float h, float s, float v)
    {
        Matrix4x4 m;

        float hr = -h * Mathf.PI * 0.5f;

        float vsu = v * s * Mathf.Cos(hr);
        float vsw = v * s * Mathf.Sin(hr);

        m.m00 = (.299f * v) + (.701f * vsu) + (.168f * vsw);
        m.m10 = (.587f * v) - (.587f * vsu) + (.330f * vsw);
        m.m20 = (.114f * v) - (.114f * vsu) - (.497f * vsw);
        m.m30 = 0f;

        m.m01 = (.299f * v) - (.299f * vsu) - (.328f * vsw);
        m.m11 = (.587f * v) + (.413f * vsu) + (.035f * vsw);
        m.m21 = (.114f * v) - (.114f * vsu) + (.292f * vsw);
        m.m31 = 0f;

        m.m02 = (.299f * v) - (.300f * vsu) + (1.25f * vsw);
        m.m12 = (.587f * v) - (.588f * vsu) - (1.05f * vsw);
        m.m22 = (.114f * v) + (.886f * vsu) - (.203f * vsw);
        m.m32 = 0;

        m.m03 = 0;
        m.m13 = 0;
        m.m23 = 0;
        m.m33 = 1;

        return m;
    }
}
