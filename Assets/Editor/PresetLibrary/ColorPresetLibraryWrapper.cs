using System;
using System.Reflection;
using UnityEngine;

public static class ColorPresetLibraryWrapper
{
    private static readonly Type type = Type.GetType("UnityEditor.ColorPresetLibrary, UnityEditor");

    private static readonly MethodInfo AddMethod = ColorPresetLibraryWrapper.type.GetMethod("Add");
    private static readonly MethodInfo CountMethod = ColorPresetLibraryWrapper.type.GetMethod("Count");
    private static readonly MethodInfo DrawMethod = ColorPresetLibraryWrapper.type.GetMethod("Draw", new Type[] { typeof(Rect), typeof(int) });
    private static readonly MethodInfo GetPresetMethod = ColorPresetLibraryWrapper.type.GetMethod("GetPreset");
    private static readonly MethodInfo RemoveMethod = ColorPresetLibraryWrapper.type.GetMethod("Remove");
    private static readonly MethodInfo ReplaceMethod = ColorPresetLibraryWrapper.type.GetMethod("Replace");

    public static ScriptableObject CreateLibrary()
    {
        return ScriptableObject.CreateInstance(ColorPresetLibraryWrapper.type);
    }

    public static void Add(ScriptableObject library, Color color, string name)
    {
        ColorPresetLibraryWrapper.AddMethod.Invoke(library, new object[] { color, name });
    }

    public static int Count(ScriptableObject library)
    {
        return (int)ColorPresetLibraryWrapper.CountMethod.Invoke(library, new object[] { });
    }

    public static void Draw(ScriptableObject library, Rect rect, int index)
    {
        ColorPresetLibraryWrapper.DrawMethod.Invoke(library, new object[] { rect, index });
    }

    public static AnimationCurve GetPreset(ScriptableObject library, int index)
    {
        return (AnimationCurve)ColorPresetLibraryWrapper.GetPresetMethod.Invoke(library, new object[] { index });
    }

    public static void Remove(ScriptableObject library, int index)
    {
        ColorPresetLibraryWrapper.RemoveMethod.Invoke(library, new object[] { index });
    }

    public static void Replace(ScriptableObject library, int index, Color newObject)
    {
        ColorPresetLibraryWrapper.ReplaceMethod.Invoke(library, new object[] { index, newObject });
    }
}
