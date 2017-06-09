using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(TransformCurve))]
public class TransformCurveEditor : Editor
{
    private static List<string> propNames = new List<string>();

    static TransformCurveEditor()
    {
        var members = typeof(Transform).GetMembers(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

        foreach (var member in members)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                var info = member as PropertyInfo;

                if (info.PropertyType == typeof(Vector3))
                {
                    propNames.Add(info.Name);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        var reflectionCurve = target as TransformCurve;

        int index = propNames.IndexOf(reflectionCurve.CurveTargetName);
        index = Mathf.Max(0, index);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Target Property", new GUILayoutOption[0]);
        index = EditorGUILayout.Popup(index, propNames.ToArray(), new GUILayoutOption[0]);
        EditorGUILayout.EndHorizontal();

        reflectionCurve.CurveTargetName = propNames[index];

        base.OnInspectorGUI();
    }
}
