using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CurvePresetLibraryUtils
{
    public static readonly Type CurvePresetLibraryType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
    public static readonly MethodInfo AddMethod = CurvePresetLibraryType.GetMethod("Add");

    public static void CreateLibrary()
    {
        var library = ScriptableObject.CreateInstance(CurvePresetLibraryType);
    }

    public static void AddCurve(ScriptableObject library, AnimationCurve curve, string name)
    {
        AddMethod.Invoke(library, new object[] { curve, name });
    }
}
