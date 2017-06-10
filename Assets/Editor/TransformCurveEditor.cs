using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

//TODO: validate length scale != 0

[CustomEditor(typeof(TransformCurve))]
public class TransformCurveEditor : Editor
{
    private static List<string> propNames = new List<string>();

   // private static Dictionary<Component, string> = new List<Component, string>;

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
        {
            reflectionCurve.Curve = EditorGUILayout.CurveField(reflectionCurve.Curve, GUILayout.Height(100f), GUILayout.Width(100f));
            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginChangeCheck();
                {
                    index = EditorGUILayout.Popup(index, propNames.ToArray());
                }
                if (EditorGUI.EndChangeCheck())
                {
                    reflectionCurve.CurveTargetName = propNames[index];
                    reflectionCurve.ResetStartEnd();
                }
                EditorGUIUtility.labelWidth = 90f;
                reflectionCurve.Start = EditorGUILayout.Vector3Field("start", reflectionCurve.Start);
                reflectionCurve.End = EditorGUILayout.Vector3Field("end", reflectionCurve.End);
                reflectionCurve.LengthScale = EditorGUILayout.FloatField("length scale", reflectionCurve.LengthScale);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        //reset to default
        EditorGUIUtility.labelWidth = 0;
    }
}
