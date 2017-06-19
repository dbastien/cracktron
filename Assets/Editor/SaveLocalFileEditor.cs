using System;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SaveLocalFileAttribute))]
public class SaveLocalFileEditor : PropertyDrawer
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
            var path = EditorUtility.SaveFilePanel("Select a file", Application.dataPath, "", "");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (path.StartsWith(Application.dataPath))
            {
                path = path.Substring(Application.dataPath.Length);
            }

            property.stringValue = path;
        }
    }
}
