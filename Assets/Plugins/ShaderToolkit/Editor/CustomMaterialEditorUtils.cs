﻿using UnityEditor;
using UnityEngine;

/// <summary>
/// Helper class for custom material editors
/// </summary>
public static class CustomMaterialEditorUtils
{
    public static Rect TextureWithToggleableColorSingleLine
    (
        MaterialEditor matEditor,
        GUIContent label,
        MaterialProperty textureProp,
        MaterialProperty colorToggleProp,
        MaterialProperty colorProp
    )
    {
        var lineRect = CustomMaterialEditorUtils.GetControlRectForSingleLine();
        var controlRect = lineRect;

        //TexturePropertyMiniThumbnail handles begin and end animation checks
        matEditor.TexturePropertyMiniThumbnail(lineRect, textureProp, label.text, label.tooltip);

        controlRect.x += EditorGUIUtility.labelWidth;
        controlRect.width = EditorGUIUtility.fieldWidth;

        GUIContent toggleTooltip = new GUIContent()
        {
            text = string.Empty,
            tooltip = "Enable/Disable color"
        };

        //label indent of -1 is the secret sauce to make it aligned with right aligned toggles that come after labels
        //ShaderProperty handles begin and end animation checks
        matEditor.ShaderProperty(controlRect, colorToggleProp, toggleTooltip, -1);

        if (colorToggleProp.floatValue != 0.0f)
        {
            controlRect.x += EditorStyles.toggle.fixedWidth;
            controlRect.x += EditorStyles.toggle.padding.right;

            //size it to take up the remainder of the space
            controlRect.width = lineRect.width - controlRect.x;

            GUIContent tooltipOnly = new GUIContent()
            {
                text = string.Empty,
                tooltip = label.tooltip
            };
            EditorGUI.showMixedValue = colorProp.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            var color = EditorGUI.ColorField(controlRect, tooltipOnly, colorProp.colorValue);
            if (EditorGUI.EndChangeCheck())
            {
                colorProp.colorValue = color;
            }
            EditorGUI.showMixedValue = false;
        }

        return lineRect;
    }

    public static void HandleToggleableColor
    (
        MaterialEditor matEditor,
        GUIContent label,
        Rect controlRect,
        Rect lineRect, 
        MaterialProperty colorToggleProp,
        MaterialProperty colorProp
    )
    {
        GUIContent toggleTooltip = new GUIContent()
        {
            text = string.Empty,
            tooltip = "Enable/Disable color"
        };

        //label indent of -1 is the secret sauce to make it aligned with right aligned toggles that come after labels
        //ShaderProperty handles begin and end animation checks
        matEditor.ShaderProperty(controlRect, colorToggleProp, toggleTooltip, -1);

        if (colorToggleProp.floatValue != 0.0f)
        {
            controlRect.x += EditorStyles.toggle.fixedWidth;
            controlRect.x += EditorStyles.toggle.padding.right;

            //size it to take up the remainder of the space
            controlRect.width = lineRect.width - controlRect.x;

            GUIContent tooltipOnly = new GUIContent()
            {
                text = string.Empty,
                tooltip = label.tooltip
            };
            EditorGUI.showMixedValue = colorProp.hasMixedValue;
            EditorGUI.BeginChangeCheck();
            var color = EditorGUI.ColorField(controlRect, tooltipOnly, colorProp.colorValue);
            if (EditorGUI.EndChangeCheck())
            {
                colorProp.colorValue = color;
            }
            EditorGUI.showMixedValue = false;
        }
    }

    public static void SetScaleOffsetKeywords
    (
        MaterialEditor matEditor,
        MaterialProperty textureProp,
        MaterialProperty scaleOffsetProp
    )
    {
        var texScaleOffset = scaleOffsetProp.vectorValue;
        bool usesScale = texScaleOffset.x != 1.0f || texScaleOffset.y != 1.0f;
        bool usesOffset = texScaleOffset.z != 0.0f || texScaleOffset.w != 0.0f;

        var mat = matEditor.target as Material;

        var scaleKeyword = textureProp.name + "_SCALE_ON";
        var offsetKeyword = textureProp.name + "_OFFSET_ON";

        mat.SetKeyword(scaleKeyword, usesScale);
        mat.SetKeyword(offsetKeyword, usesOffset);
    }

    public static Rect TextureWithAutoScaleOffsetSingleLine
    (
        MaterialEditor matEditor,
        GUIContent label,
        MaterialProperty textureProp,
        MaterialProperty scaleOffsetProp
    )
    {
        var lineRect = CustomMaterialEditorUtils.GetControlRectForSingleLine();

        //TexturePropertyMiniThumbnail handles begin and end animation checks
        matEditor.TexturePropertyMiniThumbnail(lineRect, textureProp, label.text, label.tooltip);

        CustomMaterialEditorUtils.SetScaleOffsetKeywords(matEditor, textureProp, scaleOffsetProp);

        return lineRect;
    }

    public static Rect TextureWithToggleableColorAutoScaleOffsetSingleLine
    (
        MaterialEditor matEditor,
        GUIContent label,
        MaterialProperty textureProp,
        MaterialProperty colorToggleProp, MaterialProperty colorProp,
        MaterialProperty scaleOffsetProp
    )
    {
        var rect = CustomMaterialEditorUtils.TextureWithToggleableColorSingleLine(matEditor, label, textureProp, colorToggleProp, colorProp);

        CustomMaterialEditorUtils.SetScaleOffsetKeywords(matEditor, textureProp, scaleOffsetProp);

        return rect;
    }

    public static void TextureScaleOffsetVector4Property(MaterialEditor matEditor, GUIContent label, MaterialProperty scaleOffsetProp)
    {
        EditorGUI.showMixedValue = scaleOffsetProp.hasMixedValue;
        EditorGUI.BeginChangeCheck();

        Vector4 scaleOffsetVector = scaleOffsetProp.vectorValue;

        var textureScale = new Vector2(scaleOffsetVector.x, scaleOffsetVector.y);
        textureScale = EditorGUILayout.Vector2Field(Styles.scale, textureScale, new GUILayoutOption[0]);

        var textureOffset = new Vector2(scaleOffsetVector.z, scaleOffsetVector.w);
        textureOffset = EditorGUILayout.Vector2Field(Styles.offset, textureOffset, new GUILayoutOption[0]);

        if (EditorGUI.EndChangeCheck())
        {
            scaleOffsetProp.vectorValue = new Vector4(textureScale.x, textureScale.y, textureOffset.x, textureOffset.y);
        }
        EditorGUI.showMixedValue = false;
    }

    public static Rect GetControlRectForSingleLine()
    {
        return EditorGUILayout.GetControlRect(true, 18f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
    }

    private static class Styles
    {
        public static GUIContent scale = new GUIContent("Tiling", "Scale of texture - multiplied by texture coordinates from vertices");
        public static GUIContent offset = new GUIContent("Offset", "Offset of texture - added to texture coordinates from vertices");
    }
}