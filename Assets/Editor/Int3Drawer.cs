using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Int3))]
public class Int3Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        var contentPosition = EditorGUI.PrefixLabel(position, label);

        var subLabels = new GUIContent[] { new GUIContent("x"), new GUIContent("y"), new GUIContent("z") };

        EditorGUI.MultiPropertyField(contentPosition, subLabels, property.FindPropertyRelative("x"));

        EditorGUI.EndProperty();
    }
}
