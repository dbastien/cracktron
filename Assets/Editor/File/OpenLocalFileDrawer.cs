using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OpenLocalFileAttribute))]
public class OpenLocalFileDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            throw new ArgumentException() { };
        }

        position.width -= 30;
        EditorGUI.PropertyField(position, property, label);

        position.x += position.width;
        position.width = 30.0f;

        if (GUI.Button(position, "..."))
        {
            var path = EditorUtility.OpenFilePanel("Select a file", Application.dataPath, string.Empty);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (path.StartsWith(Application.dataPath))
            {
                path = path.Substring(Application.dataPath.Length);
                path.Replace("/", "\\");
            }

            property.stringValue = path;
        }
    }

    public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
