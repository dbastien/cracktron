using UnityEngine;

public static class IntExtensions
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
}