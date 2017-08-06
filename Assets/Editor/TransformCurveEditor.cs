using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

//TODO: validate length scale != 0

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
        var targetCurve = target as TransformCurve;

        int index = propNames.IndexOf(targetCurve.CurveTargetName);
        index = Mathf.Max(0, index);

        var curveProperty = serializedObject.FindProperty("Curve");

        EditorGUILayout.PropertyField(curveProperty);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginChangeCheck();
                {
                    index = EditorGUILayout.Popup(index, propNames.ToArray());
                }
                if (EditorGUI.EndChangeCheck())
                {
                    targetCurve.CurveTargetName = propNames[index];
                    targetCurve.ResetStartEnd();
                }
                EditorGUIUtility.labelWidth = 90f;
                targetCurve.Start = EditorGUILayout.Vector3Field("start", targetCurve.Start);
                targetCurve.End = EditorGUILayout.Vector3Field("end", targetCurve.End);
                targetCurve.LengthScale = EditorGUILayout.FloatField("length scale", targetCurve.LengthScale);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        
        //reset to default
        EditorGUIUtility.labelWidth = 0;
    }
}
