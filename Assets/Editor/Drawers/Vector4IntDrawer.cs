using UnityEditor;
using UnityEngine;

/// <summary>
/// Property drawer for display of Int4
/// </summary>
[CustomPropertyDrawer(typeof(Vector4Int))]
public class Vector4IntDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        var contentPosition = EditorGUI.PrefixLabel(position, label);

        var subLabels = new GUIContent[] { new GUIContent("x"), new GUIContent("y"), new GUIContent("z"), new GUIContent("w") };

        EditorGUI.MultiPropertyField(contentPosition, subLabels, property.FindPropertyRelative("x"));

        EditorGUI.EndProperty();
    }
}
