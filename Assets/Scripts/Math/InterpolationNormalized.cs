using System;
using UnityEngine;

/// <summary>
/// All functions return clamped progress [0,1] based on clamped time [0,1]
/// </summary>
public static class InterpolationNormalized
{    
    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dt+%7D,+%7Bt,0,1%7D+%5D
    public static float Linear(float t)
    {
        return t;
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dt*t*(3-2*t)+%7D,+%7Bt,0,1%7D+%5D
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

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dt+*+t+*+t+*+(t+*+(t+*+6+-+15)+%2B+10)+%7D,+%7Bt,0,1%7D+%5D
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


    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dt+*+t+%7D,+%7Bt,0,1%7D+%5D
    public static float QuadraticIn(float t)
    {
        return t * t;
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3D-(t+*+(t+-+2))+%7D,+%7Bt,0,1%7D+%5D
    public static float QuadraticOut(float t)
    {
        return -(t * (t - 2f));
    }

    public static float QuadraticInOut(float t)
    {
        return InterpolationNormalized.DoEaseInOut(t, InterpolationNormalized.QuadraticIn, InterpolationNormalized.QuadraticOut);
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dt+*+t+*+t+%7D,+%7Bt,0,1%7D+%5D
    public static float CubicIn(float t)
    {
        return t * t * t;
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3D(t-1)%5E3+%7D,+%7Bt,0,1%7D+%5D
    public static float CubicOut(float t)
    {
        t -= 1f;
        return t * t * t + 1f;
    }

    public static float CubicInOut(float t)
    {
        return InterpolationNormalized.DoEaseInOut(t, InterpolationNormalized.CubicIn, InterpolationNormalized.CubicOut);
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3D1+-+sqrt(1+-+t+*+t)+%7D,+%7Bt,0,1%7D+%5D
    public static float CircularIn(float t)
    { 
        return 1f - Mathf.Sqrt(1f - t * t);
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dsqrt((2+-+t)+*+t)+%7D,+%7Bt,0,1%7D+%5D
    public static float CircularOut(float t)
    {
        return Mathf.Sqrt((2f - t) * t);
    }
    public static float CircularInOut(float t)
    {
        return InterpolationNormalized.DoEaseInOut(t, InterpolationNormalized.CircularIn, InterpolationNormalized.CircularOut);
    }

    public static float BounceEaseIn(float t)
    {
        return 1f - InterpolationNormalized.BounceEaseOut(1f - t);
    }

    public static float BounceEaseOut(float t)
    {
        if (t < (4f / 11f))
        {
            return (121f * t * t) / 16f;
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

    public static float BounceEaseInOut(float t)
    {
        return InterpolationNormalized.DoEaseInOut(t, InterpolationNormalized.BounceEaseIn, InterpolationNormalized.BounceEaseOut);
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3Dsin(t*PI)+%7D,+%7Bt,0,1%7D+%5D
    public static float SinHalf(float t)
    {
        return Mathf.Sin(t * MathfConstants.TauDiv2);
    }
     
    public static float Square(float t)
    {
        return (t < 0.5f) ? 0f : 1f;
    }

    public static float Triangle(float t)
    {
        return Mathf.Abs((((t + .5f) * 2f) % 2) - 1f);
    }

    // https://www.wolframalpha.com/input/?i=Plot%5B+%7B+v%3D(t+*+2)+mod+1+%7D,+%7Bt,0,1%7D+%5D
    public static float Sawtooth(float t)
    {
        //tricky - need to generate last point 
        return (t * 2f) % 1; 
    }

    public static float DoEaseInOut(float t, Func<float, float> f1, Func<float, float> f2)
    {
        if (t < 0.5f)
        {
            return 0.5f * f1(t*2f);
        }

        return 0.5f * f2(t * 2f - 1f) + 0.5f;
    }
}