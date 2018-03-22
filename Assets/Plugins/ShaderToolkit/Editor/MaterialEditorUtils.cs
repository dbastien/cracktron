using UnityEditor;
using UnityEngine;

public static class MaterialEditorUtils
{
    public static Rect TextureColorToggleInline
    (
        MaterialEditor matEditor, GUIContent label,
        MaterialProperty textureProp, MaterialProperty colorToggleProp, MaterialProperty colorProp
    )
    {
        return matEditor.TexturePropertySingleLine(label, 
                                                   textureProp, 
                                                   colorToggleProp.floatValue != 0f ? colorProp : null,
                                                   colorToggleProp);
    }

    public static Rect TextureAutoSTInline(MaterialEditor matEditor, GUIContent label, MaterialProperty texProp, MaterialProperty scaleOffsetProp)
    {
        var lineRect = ShaderGUIUtils.GetControlRectForSingleLine();
        matEditor.TexturePropertyMiniThumbnail(lineRect, texProp, label.text, label.tooltip);
        MaterialEditorUtils.SetSTKeywords(matEditor, texProp, scaleOffsetProp);
        return lineRect;
    }

    public static Rect TextureColorToggleAutoSTInline
    (
        MaterialEditor matEditor, GUIContent label,
        MaterialProperty texProp, MaterialProperty colorToggleProp, MaterialProperty colorProp, MaterialProperty scaleOffsetProp
    )
    {
        var rect = MaterialEditorUtils.TextureColorToggleInline(matEditor, label, texProp, colorToggleProp, colorProp);
        MaterialEditorUtils.SetSTKeywords(matEditor, texProp, scaleOffsetProp);
        return rect;
    }

    public static void STVector4Prop(MaterialEditor matEditor, GUIContent label, MaterialProperty scaleOffsetProp)
    {               
        EditorGUI.showMixedValue = scaleOffsetProp.hasMixedValue;
        //EditorGUI.BeginChangeCheck();

        var scaleOffsetVector = scaleOffsetProp.vectorValue;

        var textureScale = new Vector2(scaleOffsetVector.x, scaleOffsetVector.y);
        textureScale = EditorGUILayout.Vector2Field(Styles.scale, textureScale, new GUILayoutOption[0]);

        var textureOffset = new Vector2(scaleOffsetVector.z, scaleOffsetVector.w);
        textureOffset = EditorGUILayout.Vector2Field(Styles.offset, textureOffset, new GUILayoutOption[0]);

        //if (EditorGUI.EndChangeCheck())
        {
            scaleOffsetProp.vectorValue = new Vector4(textureScale.x, textureScale.y, textureOffset.x, textureOffset.y);
        }
        EditorGUI.showMixedValue = false;
    }

    public static void SetSTKeywords(MaterialEditor matEditor, MaterialProperty texProp, MaterialProperty scaleOffsetProp)
    {
        var texScaleOffset = scaleOffsetProp.vectorValue;
        var mat = matEditor.target as Material;
        mat.SetKeyword(texProp.name + "_SCALE_ON" , texScaleOffset.x != 1.0f || texScaleOffset.y != 1.0f);
        mat.SetKeyword(texProp.name + "_OFFSET_ON", texScaleOffset.z != 0.0f || texScaleOffset.w != 0.0f);
    }

    private static class Styles
    {
        public static GUIContent scale = new GUIContent("Tiling", "Scale of texture - multiplied by texture coordinates from vertices");
        public static GUIContent offset = new GUIContent("Offset", "Offset of texture - added to texture coordinates from vertices");
    }
}