using System;
using UnityEngine;

public static class MathfExtensions
{
    /// <summary>
    /// Returns the power of 2 >= val
    /// </summary>
    /// <param name="v">positive integer (result is always 1 for negative integers or zero)</param>
    /// <returns>power of 2 >= val</returns>
    public static int RoundUpPowerOfTwo(this int v)
    {
        if (v <= 1)
        {
            return 1;
        }

        --v;

        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;

        return v + 1;
    }

    public static Int3 RoundUpPowerOfTwo(this Int3 v)
    {
        return new Int3(RoundUpPowerOfTwo(v.x),
                        RoundUpPowerOfTwo(v.y),
                        RoundUpPowerOfTwo(v.z));
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
