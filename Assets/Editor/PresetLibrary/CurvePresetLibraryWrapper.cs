using System;
using System.Reflection;
using UnityEngine;

public static class CurvePresetLibraryWrapper
{
    private static readonly Type type = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");

    private static readonly MethodInfo AddMethod = type.GetMethod("Add");
    private static readonly MethodInfo CountMethod = type.GetMethod("Count");
    private static readonly MethodInfo DrawMethod = type.GetMethod("Draw", new Type[] { typeof(Rect), typeof(int) });
    private static readonly MethodInfo GetPresetMethod = type.GetMethod("GetPreset");
    private static readonly MethodInfo RemoveMethod = type.GetMethod("Remove");
    private static readonly MethodInfo ReplaceMethod = type.GetMethod("Replace");

    public static ScriptableObject CreateLibrary()
    {
        return ScriptableObject.CreateInstance(type);
    }

    public static void Add(ScriptableObject library, AnimationCurve curve, string name)
    {
        AddMethod.Invoke(library, new object[] { curve, name });
    }

    public static int Count(ScriptableObject library)
    {
        return (int)CountMethod.Invoke(library, new object[] { });
    }

    public static void Draw(ScriptableObject library, Rect rect, int index)
    {
        DrawMethod.Invoke(library, new object[] { rect, index });
    }

    public static AnimationCurve GetPreset(ScriptableObject library, int index)
    {
        return (AnimationCurve)GetPresetMethod.Invoke(library, new object[] { index });
    }

    public static void Remove(ScriptableObject library, int index)
    {
        RemoveMethod.Invoke(library, new object[] { index });
    }

    public static void Replace(ScriptableObject library, int index, AnimationCurve newObject)
    {
        ReplaceMethod.Invoke(library, new object[] { index, newObject });
    }
}
