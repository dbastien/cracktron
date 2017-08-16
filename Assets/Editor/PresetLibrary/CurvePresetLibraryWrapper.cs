using System;
using System.Reflection;
using UnityEngine;

public static class CurvePresetLibraryWrapper
{
    private static readonly Type type = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");

    private static readonly MethodInfo AddMethod = CurvePresetLibraryWrapper.type.GetMethod("Add");
    private static readonly MethodInfo CountMethod = CurvePresetLibraryWrapper.type.GetMethod("Count");
    private static readonly MethodInfo DrawMethod = CurvePresetLibraryWrapper.type.GetMethod("Draw", new Type[] { typeof(Rect), typeof(int) });
    private static readonly MethodInfo GetPresetMethod = CurvePresetLibraryWrapper.type.GetMethod("GetPreset");
    private static readonly MethodInfo RemoveMethod = CurvePresetLibraryWrapper.type.GetMethod("Remove");
    private static readonly MethodInfo ReplaceMethod = CurvePresetLibraryWrapper.type.GetMethod("Replace");

    public static ScriptableObject CreateLibrary()
    {
        return ScriptableObject.CreateInstance(CurvePresetLibraryWrapper.type);
    }

    public static void Add(ScriptableObject library, AnimationCurve curve, string name)
    {
        CurvePresetLibraryWrapper.AddMethod.Invoke(library, new object[] { curve, name });
    }

    public static int Count(ScriptableObject library)
    {
        return (int)CurvePresetLibraryWrapper.CountMethod.Invoke(library, new object[] { });
    }

    public static void Draw(ScriptableObject library, Rect rect, int index)
    {
        CurvePresetLibraryWrapper.DrawMethod.Invoke(library, new object[] { rect, index });
    }

    public static AnimationCurve GetPreset(ScriptableObject library, int index)
    {
        return (AnimationCurve)CurvePresetLibraryWrapper.GetPresetMethod.Invoke(library, new object[] { index });
    }

    public static void Remove(ScriptableObject library, int index)
    {
        CurvePresetLibraryWrapper.RemoveMethod.Invoke(library, new object[] { index });
    }

    public static void Replace(ScriptableObject library, int index, AnimationCurve newObject)
    {
        CurvePresetLibraryWrapper.ReplaceMethod.Invoke(library, new object[] { index, newObject });
    }
}
