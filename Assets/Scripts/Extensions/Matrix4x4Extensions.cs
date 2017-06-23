using UnityEngine;

public static class Matrix4x4Extensions
{
    /// <summary>
    /// Creates a matrix for transforming an RGB color in HSV space
    /// </summary>
    /// <param name="H">hue shift in degrees</param>
    /// <param name="S">saturation multiplier</param>
    /// <param name="V">value multiplier</param>
    /// <returns></returns>
    public static Matrix4x4 CreateHSVTransform(float H, float S, float V)
    {
        Matrix4x4 m;

        float HR = -H * Mathf.PI * 0.5f;

        float VSU = V * S * Mathf.Cos(HR);
        float VSW = V * S * Mathf.Sin(HR);

        m.m00 = .299f * V + .701f * VSU + .168f * VSW;
        m.m10 = .587f * V - .587f * VSU + .330f * VSW;
        m.m20 = .114f * V - .114f * VSU - .497f * VSW;
        m.m30 = 0f;

        m.m01 = .299f * V - .299f * VSU - .328f * VSW;
        m.m11 = .587f * V + .413f * VSU + .035f * VSW;
        m.m21 = .114f * V - .114f * VSU + .292f * VSW;
        m.m31 = 0f;

        m.m02 = .299f * V - .300f * VSU + 1.25f * VSW;
        m.m12 = .587f * V - .588f * VSU - 1.05f * VSW;
        m.m22 = .114f * V + .886f * VSU - .203f * VSW;
        m.m32 = 0;

        m.m03 = 0;
        m.m13 = 0;
        m.m23 = 0;
        m.m33 = 1;

        return m;
    }
}
