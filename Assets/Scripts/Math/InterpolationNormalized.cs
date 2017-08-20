using UnityEngine;

/// <summary>
/// All functions return unclamped progress ~[0,1] based on unclamped time [0,1]
/// </summary>
public static class InterpolationNormalized
{
    // https://www.wolframalpha.com/input/?i=plot+v%3Dt,+0%3C%3Dt%3C%3D1
    public static float Linear(float t)
    {
        return t;
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3Dt+*+t+*+(3+-+2+*+t),+0%3C%3Dt%3C%3D1
    public static float SmoothStep(float t)
    {
        //3rd order
        return t * t * (3f - 2f * t);
    }

    public static float SmoothStepC1(float t)
    {
        //SmoothStep chained once
        return InterpolationNormalized.SmoothStep(InterpolationNormalized.SmoothStep(t));
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3Dt+*+t+*+t+*+(t+*+(t+*+6+-+15)+%2B+10),+0%3C%3Dt%3C%3D1
    public static float SmootherStep(float t)
    {
        //5th order
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    public static float SmootherStepC1(float t)
    {
        //SmootherStep chained once
        return InterpolationNormalized.SmootherStep(InterpolationNormalized.SmootherStep(t));
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3Dt*t,+0%3C%3Dt%3C%3D1
    public static float QuadraticIn(float t)
    {
        return t * t;
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3D-(t+*+(t+-+2)),+0%3C%3Dt%3C%3D1
    public static float QuadraticOut(float t)
    {
        return -(t * (t - 2f));
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3Dt*t*t,+0%3C%3Dt%3C%3D1
    public static float CubicIn(float t)
    {
        return t * t * t;
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3D(t-1)%5E3%2B1,+0%3C%3Dt%3C%3D1
    public static float CubicOut(float t)
    {
        t -= 1f;
        return t * t * t + 1f;
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3Dsin(t*PI),+0%3C%3Dt%3C%3D1
    public static float SineHalf(float t)
    {
        return Mathf.Sin(t * MathfConstants.TauDiv2);
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3D1+-+sqrt(1+-+t+*+t),+0%3C%3Dt%3C%3D1
    public static float CircularIn(float t)
    {
        return 1f - Mathf.Sqrt(1f - t * t);
    }

    // https://www.wolframalpha.com/input/?i=plot+v%3Dsqrt((2+-+t)+*+t),+0%3C%3Dt%3C%3D1
    public static float CircularOut(float t)
    {
        return Mathf.Sqrt((2f - t) * t);
    }

    public static float BounceEaseIn(float t)
    {
        return 1 - InterpolationNormalized.BounceEaseOut(1 - t);
    }

    public static float BounceEaseOut(float t)
    {
        if (t < (4f / 11f))
        {
            return (121 * t * t) / 16f;
        }

        if (t < (8 / 11f))
        {
            return ((363f / 40f) * t * t) - ((99f / 10f) * t) + (17f / 5f);
        }
        
        if (t < (9f / 10f))
        {
            return ((4356f / 361f) * t * t) - ((35442f / 1805f) * t) + (16061f / 1805f);
        }

        return ((54f / 5f) * t * t) - ((513f / 25f) * t) + (268f / 25f);
    }
}