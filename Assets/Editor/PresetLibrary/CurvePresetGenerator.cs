using UnityEngine;
using UnityEditor;

public static class CurvePresetGenerator
{
    public static readonly int StepCount = 50;
    public static readonly float StepSize = StepCount >= 2 ? 1f / ((float)StepCount -  1) : 1f;

    public delegate float NormalizedCurveFunction(float t); 

    [MenuItem("Cracktron/Preset Libraries/Generate curves")] 
    public static void GenerateCurvePresets()
    {
        var library = CurvePresetLibraryWrapper.CreateLibrary();
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.Linear), "linear");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.SmoothStep), "smooth step");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.SmootherStep), "smoother step");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.QuadraticIn), "quadratic in");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.CubicIn), "cubic in");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.CircularIn), "circular in");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.SineHalf), "sine half");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.QuadraticOut), "quadratic out");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.CubicOut), "cubic out");
        CurvePresetLibraryWrapper.Add(library, CreateCurve(InterpolationNormalized.CircularOut), "circular out");
         
        AssetDatabase.CreateAsset(library, "Assets/Editor/CracktronCurves.curvesNormalized");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static AnimationCurve CreateCurve(NormalizedCurveFunction f) 
    {
        var curve = new AnimationCurve();
        float t = 0.0f;
        for (var i = 0; i < StepCount; ++i)
        { 
            float val = f(t);          
            curve.AddKey(new Keyframe(t, val)); 
            t += StepSize;
        }

        AnimationCurveUtils.SetTangentMode(curve, AnimationUtility.TangentMode.Auto);
        AnimationCurveUtils.SetKeysBroken(curve, false);
        AnimationCurveUtils.SmoothAllTangents(curve);

        return curve;
    }
}
