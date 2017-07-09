using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEditor.Callbacks;

//TODO: validate length scale != 0

[CustomEditor(typeof(TransformCurve))]
public class TransformCurveEditor : Editor
{
    private static ScriptableObject presets;
    private static List<string> propNames = new List<string>();

    public WrapMode CurveWrapMode = WrapMode.Loop;

    Vector2 scrollPos;
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

    [DidReloadScripts]
    private static void LoadPresets()
    {
        var path = Application.dataPath + "/Editor/NormalizedCurves.curvesNormalized";
        var objs = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(path);
        presets = objs[0] as ScriptableObject;

        //var libs = AssetDatabase.FindAssets("t:UnityEditor.CurvePresetLibrary");

        //for (int i = 0; i < libs.Length; ++i)  
        //{
        //    Debug.Log(libs[i]);
        //}

        //Debug.Log("whee");
    }

    public override void OnInspectorGUI()
    {
        if (presets == null)
        {
            LoadPresets();
        }

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
