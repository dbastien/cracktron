using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentMemberReferenceCurve))]
public class ComponentMemberReferenceCurveEditor : Editor
{
    public static readonly List<Type> TargetTypes = new List<Type>
    {
        typeof(float),
        typeof(Vector2), typeof(Vector3), typeof(Vector4),
        typeof(Color)
    };

    public override void OnInspectorGUI()
    {
//        var targetCurve = this.target as ComponentMemberReferenceCurve;

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
                EditorGUIUtility.labelWidth = 90f;
//                targetCurve.Start = EditorGUILayout.Vector3Field("start", targetCurve.Start);
//                targetCurve.End = EditorGUILayout.Vector3Field("end", targetCurve.End);
//                targetCurve.LengthScale = EditorGUILayout.FloatField("length scale", targetCurve.LengthScale);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();        
    }
}