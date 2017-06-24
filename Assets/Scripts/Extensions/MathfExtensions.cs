using System;
using UnityEngine;

public static class MathfExtensions
{
    public static int MostSignificantBit(this int x)
    {
        x |= (x >> 1);
        x |= (x >> 2);
        x |= (x >> 4);
        x |= (x >> 8);
        x |= (x >> 16);

        return x & ~(x >> 1);
    }

    public static int PowerOfTwoGreaterThanOrEqualTo(this int v)
    {
        if (Mathf.IsPowerOfTwo(v))
        {
            return v;
        }

        return Mathf.NextPowerOfTwo(v);
    }

    public static Int3 PowerOfTwoGreaterThanOrEqualTo(this Int3 v)
    {
        return new Int3(PowerOfTwoGreaterThanOrEqualTo(v.x),
                        PowerOfTwoGreaterThanOrEqualTo(v.y),
                        PowerOfTwoGreaterThanOrEqualTo(v.z));
    }

    public static float Remap01(this float x, float minIn, float maxIn)
    {
        return (x - minIn) / (maxIn - minIn);
    }

    public static float Remap(this float x, float minIn, float maxIn, float minOut, float maxOut)
    {
        return minOut + (maxOut - minOut) * Remap01(x, minIn, maxIn);
    }
}
