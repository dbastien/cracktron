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
        var lib = CurvePresetLibraryWrapper.CreateLibrary();

        //in & out non-piecewise
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.Linear), "linear");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmoothStep), "smoothstep");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmoothStepC1), "smoothstep c1");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmootherStep), "smootherstep");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SmootherStepC1), "smootherstep c1");

        //in & out piecewise
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.QuadraticInOut), "quadratic in out");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CubicInOut), "cubic in out");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CircularInOut), "circular in out");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.BounceEaseInOut), "bounce in out");

        //in
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.QuadraticIn), "quadratic in");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CubicIn), "cubic in");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CircularIn), "circular in");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.BounceEaseIn), "bounce in");
        
        //out
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.QuadraticOut), "quadratic out");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CubicOut), "cubic out");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.CircularOut), "circular out");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.BounceEaseOut), "bounce out");

        //centered (t(.5)=1) waves
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.SinHalf), "sine half");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.Square), "square");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.Triangle), "triangle");
        CurvePresetLibraryWrapper.Add(lib, CurvePresetGenerator.CreateCurve(InterpolationNormalized.Sawtooth), "sawtooth");
         
        AssetDatabase.CreateAsset(lib, "Assets" + CurveConstants.NormalizedCurvesPath);

        var libUnnormalized = Object.Instantiate(lib);
        AssetDatabase.CreateAsset(libUnnormalized, "Assets" + CurveConstants.CurvesPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static AnimationCurve CreateCurve(NormalizedCurveFunction f)
    {
        return CurvePresetGenerator.CreateCurve(f, CurvePresetGenerator.StepCount);
    }

    public static AnimationCurve CreateCurve(NormalizedCurveFunction f, int stepCount)
    {
        var curve = new AnimationCurve()
        {
            preWrapMode = WrapMode.PingPong,
            postWrapMode = WrapMode.PingPong
        };

        float t = 0f;
        for (var i = 0; i < stepCount; ++i)
        {
            //clamp input to [0,1]
            var clamped = Mathf.Clamp01(t);

            //execute curve function and clamp output to [0,1]
            var val = Mathf.Clamp01(f(clamped));

            var keyframe = new Keyframe(clamped, val);
            //AnimationUtility.SetKeyLeftTangentMode
            curve.AddKey(keyframe);

            t += CurvePresetGenerator.StepSize;
        }

        AnimationCurveUtils.SetTangentMode(curve, AnimationUtility.TangentMode.Auto);
        AnimationCurveUtils.SetKeysBroken(curve, false);
        AnimationCurveUtils.SmoothAllTangents(curve);

        return curve;
    }
}
