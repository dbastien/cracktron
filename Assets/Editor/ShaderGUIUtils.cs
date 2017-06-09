using UnityEngine;
using UnityEditor;

public static class ShaderGUIUtils
{
    public static readonly GUILayoutOption[] GUILayoutEmptyArray = new GUILayoutOption[0];

    //re-implementation of MaterialEditor internal
    public static Rect GetControlRectForSingleLine()
    {
        return EditorGUILayout.GetControlRect(true, 18f, EditorStyles.layerMaskField, GUILayoutEmptyArray);
    }

    //re-implementation of EditorGUI internal
    public static void GetRectsForMiniThumbnailField(Rect position, out Rect thumbRect, out Rect labelRect)
    {
        thumbRect = EditorGUI.IndentedRect(position);
        thumbRect.y -= 1f;
        thumbRect.height = 18f;
        thumbRect.width = 32f;

        float xPos = thumbRect.x + 30f;

        labelRect = new Rect(xPos, position.y, thumbRect.x + EditorGUIUtility.labelWidth - xPos, position.height);
    }

    public static void SetKeyword(Material mat, string keyword, bool state)
    {
        if (state)
        {
            mat.EnableKeyword(keyword);
        }
        else
        {
            mat.DisableKeyword(keyword);
        }
    }

    public static bool TryGetToggle(Material material, string property, bool defaultVal)
    {
        if (material.HasProperty(property))
        {
            return material.GetFloat(property) == 1.0f;
        }
        return defaultVal;
    }
}