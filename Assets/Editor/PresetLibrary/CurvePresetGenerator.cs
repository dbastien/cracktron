using UnityEngine;
using UnityEditor;

public static class CurvePresetGenerator
{
    public static readonly int StepCount = 100;
    public static readonly float StepSize = StepCount > 1 ? 1f / (StepCount - 1) : 1f;

    public delegate float NormalizedCurveFunction(float t);

    [MenuItem("Cracktron/Preset Libraries/Generate curves")] 
    public static void GenerateCurvePresets()
    {
        var libraryNormalized = CurvePresetLibraryWrapper.CreateLibrary();
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.Linear), "linear");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.SmoothStep), "smooth step");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.SmootherStep), "smoother step");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.QuadraticIn), "quadratic in");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.CubicIn), "cubic in");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.CircularIn), "circular in");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.SineHalf), "sine half");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.QuadraticOut), "quadratic out");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.CubicOut), "cubic out");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CreateCurve(InterpolationNormalized.CircularOut), "circular out");
        AssetDatabase.CreateAsset(libraryNormalized, "Assets" + Constants.NormalizedCurvesPath);

        var libraryUnnormalized = Object.Instantiate(libraryNormalized);

        AssetDatabase.CreateAsset(libraryUnnormalized, "Assets" + Constants.CurvesPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static AnimationCurve CreateCurve(NormalizedCurveFunction f)
    {
        var curve = new AnimationCurve();
        float t = 0.0f;
        for (var i = 0; i < StepCount; ++i)
        {
            float tClamped = Mathf.Clamp01(t);
            float val = Mathf.Clamp01(f(tClamped));
            curve.AddKey(new Keyframe(tClamped, val));
            t += StepSize;
        }

        AnimationCurveUtils.SetTangentMode(curve, AnimationUtility.TangentMode.Auto);
        AnimationCurveUtils.SetKeysBroken(curve, false);
        AnimationCurveUtils.SmoothAllTangents(curve);

        return curve;
    }
}
