using UnityEngine;

/// <summary>
/// All functions return unclamped progress [0,1] based on unclamped time [0,1]
/// </summary>
public static class InterpolationNormalized
{
    //https://www.wolframalpha.com/input/?i=plot+v%3Dt,+0%3C%3Dt%3C%3D1
    public static float Linear(float t)
    {
        return t;
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3Dt+*+t+*+(3+-+2+*+t),+0%3C%3Dt%3C%3D1
    public static float SmoothStep(float t)
    {
        //3rd order
        return t * t * (3f - 2f * t);
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3Dt+*+t+*+t+*+(t+*+(t+*+6+-+15)+%2B+10),+0%3C%3Dt%3C%3D1
    public static float SmootherStep(float t)
    {
        //5th order
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3Dt*t,+0%3C%3Dt%3C%3D1
    public static float QuadraticIn(float t)
    {
        return t * t;
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3D-(t+*+(t+-+2)),+0%3C%3Dt%3C%3D1
    public static float QuadraticOut(float t)
    {
        return -(t * (t - 2f));
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3Dt*t*t,+0%3C%3Dt%3C%3D1
    public static float CubicIn(float t)
    {
        return t * t * t;
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3D(t-1)%5E3%2B1,+0%3C%3Dt%3C%3D1
    public static float CubicOut(float t)
    {
        t -= 1f;
        return t * t * t + 1f;
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3Dsin(t*PI),+0%3C%3Dt%3C%3D1
    public static float SineHalf(float t)
    {
        return Mathf.Sin(t * MathfConstants.TauDiv2);
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3D1+-+sqrt(1+-+t+*+t),+0%3C%3Dt%3C%3D1
    public static float CircularIn(float t)
    {
        return 1f - Mathf.Sqrt(1f - t * t);
    }

    //https://www.wolframalpha.com/input/?i=plot+v%3Dsqrt((2+-+t)+*+t),+0%3C%3Dt%3C%3D1
    public static float CircularOut(float t)
    {
        return Mathf.Sqrt((2f - t) * t);
    }
}