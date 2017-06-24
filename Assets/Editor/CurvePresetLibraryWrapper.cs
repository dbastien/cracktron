using System;
using System.Reflection;
using UnityEngine;

public static class CurvePresetLibraryWrapper
{
    public static readonly Type CurvePresetLibraryType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
    //PresetLibrary presetLibrary = ScriptableSingleton<PresetLibraryManager>.instance.GetLibrary<T>(this.m_SaveLoadHelper, libraryPath) as PresetLibrary;

    public static ScriptableObject CreateLibrary()
    {
        return ScriptableObject.CreateInstance(CurvePresetLibraryType);
    }

    #region wrapped methods
    public static readonly MethodInfo AddMethod = CurvePresetLibraryType.GetMethod("Add");
    public static void Add(ScriptableObject library, AnimationCurve curve, string name)
    {
        AddMethod.Invoke(library, new object[] { curve, name });
    }

    public static readonly MethodInfo CountMethod = CurvePresetLibraryType.GetMethod("Count");
    public static int Count(ScriptableObject library)
    {
        return (int)CountMethod.Invoke(library, new object[] { });
    }

    public static readonly MethodInfo GetPresetMethod = CurvePresetLibraryType.GetMethod("GetPreset");
    public static AnimationCurve GetPreset(ScriptableObject library, int index)
    {
        return (AnimationCurve)GetPresetMethod.Invoke(library, new object[] { index });
    }

    public static readonly MethodInfo RemoveMethod = CurvePresetLibraryType.GetMethod("Remove");
    public static void Remove(ScriptableObject library, int index)
    {
        RemoveMethod.Invoke(library, new object[] { index });
    }

    public static readonly MethodInfo ReplaceMethod = CurvePresetLibraryType.GetMethod("Replace");
    public static void Replace(ScriptableObject library, int index, AnimationCurve newObject)
    {
        ReplaceMethod.Invoke(library, new object[] { index, newObject });
    }
    #endregion
}
