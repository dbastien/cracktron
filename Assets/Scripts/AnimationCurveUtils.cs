using UnityEngine;

public static class AnimationCurveUtils
{
    public static void SmoothAllTangents(AnimationCurve curve)
    {
        for (var k = 0; k < curve.length; ++k)
        {
            curve.SmoothTangents(k, 0.0f);
        }
    }
}