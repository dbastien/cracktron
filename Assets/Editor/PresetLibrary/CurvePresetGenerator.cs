using UnityEditor;
using UnityEngine;

public static class CurvePresetGenerator
{
    public static readonly int StepCount = 50;
    public static readonly float StepSize = CurvePresetGenerator.StepCount > 1 ? 1f / (CurvePresetGenerator.StepCount - 1) : 1f;

    public delegate float NormalizedCurveFunction(float t);

    [MenuItem("Cracktron/Preset Libraries/Generate curves")] 
    public static void GenerateCurvePresets()
    {
        var libraryNormalized = CurvePresetLibraryWrapper.CreateLibrary();
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.Linear), "linear");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmoothStep), "smoothstep");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmoothStepC1), "smoothstep c1");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmootherStep), "smootherstep");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmootherStepC1), "smootherstep c1");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SineHalf), "sine half");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.QuadraticIn), "quadratic in");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CubicIn), "cubic in");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CircularIn), "circular in");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.BounceEaseIn), "bounce ease in");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.QuadraticOut), "quadratic out");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CubicOut), "cubic out");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CircularOut), "circular out");
        CurvePresetLibraryWrapper.Add(libraryNormalized, CurvePresetGenerator.CreateCurve(InterpolationNormalized.BounceEaseOut), "bounce ease out");
        AssetDatabase.CreateAsset(libraryNormalized, "Assets" + Constants.NormalizedCurvesPath);

        var libraryUnnormalized = Object.Instantiate(libraryNormalized);
        AssetDatabase.CreateAsset(libraryUnnormalized, "Assets" + Constants.CurvesPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static AnimationCurve CreateCurve(NormalizedCurveFunction f)
    {
        var curve = new AnimationCurve()
        {
            preWrapMode = WrapMode.PingPong,
            postWrapMode = WrapMode.PingPong
        };

        float t = 0.0f;
        for (var i = 0; i < CurvePresetGenerator.StepCount; ++i)
        {
            float clamped = Mathf.Clamp01(t);
            float val = Mathf.Clamp01(f(clamped));
            curve.AddKey(new Keyframe(clamped, val));
            t += CurvePresetGenerator.StepSize;
        }

        AnimationCurveUtils.SetTangentMode(curve, AnimationUtility.TangentMode.Auto);
        AnimationCurveUtils.SetKeysBroken(curve, false);
        AnimationCurveUtils.SmoothAllTangents(curve);

        return curve;
    }
}
