using UnityEngine;
using UnityEditor;

public static class CurvePresetGenerator
{
    public static readonly int StepCount = 100;
    public static readonly float StepSize = 1 / (float)StepCount;

    delegate float NormalizedCurveFunction(float t);

    [MenuItem("Cracktron/Generate curve presets")]
    public static void GenerateCurvePresets()
    {
        var library = CurvePresetLibraryWrapper.CreateLibrary();
        CurvePresetLibraryWrapper.Add(library, CreateCurve(), "testCurve");

        AssetDatabase.CreateAsset(library, "Assets/Editor/CracktronCurves.curvesNormalized");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static AnimationCurve CreateCurve()
    {
        var curve = new AnimationCurve();
        float t = 0.0f;
        for (var i = 0; i < StepCount; ++i)
        {
            float val = Mathf.SmoothStep(0.0f, 1.0f, t);
            curve.AddKey(new Keyframe(t, val));
            t += StepSize;
        }

        AnimationCurveUtils.SmoothAllTangents(curve);

        return curve;
    }
}
