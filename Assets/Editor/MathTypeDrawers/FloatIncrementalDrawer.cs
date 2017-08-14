using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatIncrementalAttribute))]
public class FloatIncrementalDrawer : PropertyDrawer
{
    public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "|Increase");
    public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "|Decrease");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        const float buttonWidth = 24f;

        EditorGUI.BeginProperty(position, label, property);

        var floatWidth = position.width - (buttonWidth * 2);

        var propRect = new Rect(position.x, position.y, floatWidth, position.height);
        var plusRect = new Rect(propRect.x + propRect.width, position.y, buttonWidth, position.height);
        var minusRect = new Rect(plusRect.x + plusRect.width, position.y, buttonWidth, position.height);

        property.floatValue = EditorGUI.FloatField(propRect, label, property.floatValue);

        var inc = (this.attribute as FloatIncrementalAttribute).Increment;

        if (GUI.Button(plusRect, this.iconToolbarMinus))
        {
            property.floatValue -= inc;
        }
        if (GUI.Button(minusRect, this.iconToolbarPlus))
        {
            property.floatValue += inc;
        }

        EditorGUI.EndProperty();
    }
}


