using System;
using System.Reflection;
using UnityEngine;

public static class CurvePresetLibraryWrapper
{
    private static readonly Type CurvePresetLibraryType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
    //PresetLibrary presetLibrary = ScriptableSingleton<PresetLibraryManager>.instance.GetLibrary<T>(this.m_SaveLoadHelper, libraryPath) as PresetLibrary;

    private static readonly MethodInfo AddMethod = CurvePresetLibraryType.GetMethod("Add");
    private static readonly MethodInfo CountMethod = CurvePresetLibraryType.GetMethod("Count");
    private static readonly MethodInfo GetPresetMethod = CurvePresetLibraryType.GetMethod("GetPreset");
    private static readonly MethodInfo RemoveMethod = CurvePresetLibraryType.GetMethod("Remove");
    private static readonly MethodInfo ReplaceMethod = CurvePresetLibraryType.GetMethod("Replace");

    public static ScriptableObject CreateLibrary()
    {
        return ScriptableObject.CreateInstance(CurvePresetLibraryType);
    }

    public static void Add(ScriptableObject library, AnimationCurve curve, string name)
    {
        AddMethod.Invoke(library, new object[] { curve, name });
    }

    public static int Count(ScriptableObject library)
    {
        return (int)CountMethod.Invoke(library, new object[] { });
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
