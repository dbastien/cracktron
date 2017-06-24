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

        //for (int i = 0; i < currentLib.Count(); ++i)
        //{
        //    Rect rect = new Rect(45f + 45f * (float)i, num, 40f, 25f);
        //    this.m_GUIContent.tooltip = currentLib.GetName(i);
        //    if (GUI.Button(rect, this.m_GUIContent, CurveEditorWindow.ms_Styles.curveSwatch))
        //    {
        //        var animationCurve = currentLib.GetPreset(i) as AnimationCurve;
        //        this.m_Curve.keys = this.GetDenormalizedKeys(animationCurve.keys);
        //        this.m_Curve.postWrapMode = animationCurve.postWrapMode;
        //        this.m_Curve.preWrapMode = animationCurve.preWrapMode;
        //        this.m_CurveEditor.SelectNone();
        //        this.SendEvent("CurveChanged", true);
        //    }
        //}

        //reset to default
        EditorGUIUtility.labelWidth = 0;
    }
}
