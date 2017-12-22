using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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
                    TransformCurveEditor.propNames.Add(info.Name);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        var targetCurve = this.target as TransformCurve;

        int index = TransformCurveEditor.propNames.IndexOf(targetCurve.CurveTargetName);
        index = Mathf.Max(0, index);

        var curveProperty = this.serializedObject.FindProperty("Curve");

        EditorGUILayout.PropertyField(curveProperty);
        this.serializedObject.ApplyModifiedProperties();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginChangeCheck();
                {
                    index = EditorGUILayout.Popup(index, TransformCurveEditor.propNames.ToArray());
                }
                if (EditorGUI.EndChangeCheck())
                {
                    targetCurve.CurveTargetName = TransformCurveEditor.propNames[index];
                    targetCurve.ResetStartEnd();
                }
                EditorGUIUtility.labelWidth = 90f;

                targetCurve.RelativeMode = EditorGUILayout.Toggle("Relative Mode", targetCurve.RelativeMode);

                if (targetCurve.RelativeMode)
                {
                    targetCurve.CurveOffset = EditorGUILayout.Vector3Field("offset", targetCurve.CurveOffset);
                }
                else
                {
                    targetCurve.CurveStart = EditorGUILayout.Vector3Field("start", targetCurve.CurveStart);
                    targetCurve.CurveEnd = EditorGUILayout.Vector3Field("end", targetCurve.CurveEnd);
                }

                targetCurve.LengthScale = EditorGUILayout.FloatField("length scale", targetCurve.LengthScale);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        
        //reset to default
        EditorGUIUtility.labelWidth = 0;
    }
}
