using System;
using UnityEngine;

public static class MathfConstants
{
    public const float Tau = Mathf.PI * 2.0f;
    public const float TauDiv2 = Mathf.PI;
    public const float TauDiv4 = Mathf.PI * 0.5f;
    public const float PIDiv2 = MathfConstants.TauDiv4;
    public const float TauInv = 1.0f / MathfConstants.Tau;
    public const float TauSqrt = 2.506628f;
    public const float E = (float)Math.E;
}