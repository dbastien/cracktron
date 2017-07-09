using UnityEditor;
using UnityEngine;

public static class SerializedObjectExtensions
{
    public static void LogChildPropertyNames(this SerializedObject so)
    {
        var sp = so.GetIterator();
        sp.LogPathWithChildren();
    }
}