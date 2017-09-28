using UnityEditor;
using UnityEngine;

/// <summary>
/// Property drawer for display of Int2
/// </summary>
[CustomPropertyDrawer(typeof(Int2))]
public class Int2Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        var contentPosition = EditorGUI.PrefixLabel(position, label);

        var subLabels = new GUIContent[] { new GUIContent("x"), new GUIContent("y") };

        EditorGUI.MultiPropertyField(contentPosition, subLabels, property.FindPropertyRelative("x"));

        EditorGUI.EndProperty();
    }
}
