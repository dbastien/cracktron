using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatIncrementalAttribute))]
public class FloatIncrementalDrawer : PropertyDrawer
{
    public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "|Increase");
    public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "|Decrease");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Float)
        {
            throw new ArgumentException() { };
        }

        var buttonLayout = new[] { GUILayout.Width(24f), GUILayout.Height(EditorGUIUtility.singleLineHeight) };

        const float buttonWidth = 24f;
        const float controlSpacing = 0f;

        float floatWidth = position.width - (buttonWidth * 2 + controlSpacing * 2);

        var floatRect = new Rect(position.x, position.y, floatWidth, position.height );
        var plusRect = new Rect(position.x + floatWidth + controlSpacing, position.y, buttonWidth, position.height );
        var minusRect = new Rect(position.x + floatWidth + controlSpacing*2 + buttonWidth, position.y, buttonWidth, position.height);

        position.x = EditorGUIUtility.currentViewWidth - position.width - 20f;

        property.floatValue = EditorGUI.FloatField(floatRect, label, property.floatValue);

        var inc = (this.attribute as FloatIncrementalAttribute).Incremeent;

        if (GUI.Button(plusRect, this.iconToolbarMinus))
        {
            property.floatValue -= inc;
        }
        if (GUI.Button(minusRect, this.iconToolbarPlus))
        {
            property.floatValue += inc;
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }
}


