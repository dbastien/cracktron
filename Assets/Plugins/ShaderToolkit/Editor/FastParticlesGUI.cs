using System;
using UnityEditor;
using UnityEngine;

public class FastParticlesGUI : AdvancedShaderGUI
{
    protected MaterialProperty blendMode;

    protected override void CacheProperties(MaterialProperty[] props)
    {
        // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        this.blendMode = ShaderGUI.FindProperty("_Mode", props);
        propsByName = PropertiesToDictionary(props);
    }

    protected override void CheckAndHandleFirstTimeApply(MaterialEditor matEditor)
    {
        if (!this.firstTimeApply) { return; }

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching from an existing material.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        var matTarget = matEditor.target as Material;
        this.SetMaterialBlendPreset(matTarget, (BlendPreset)this.blendMode.floatValue);
        this.SetMaterialAutoPropertiesAndKeywords(matTarget);
        this.firstTimeApply = false;
    }

    protected override void UpdateAllTargets()
    {
        foreach (var obj in this.blendMode.targets)
        {
            var mat = obj as Material;
            this.SetMaterialBlendPreset(mat, (BlendPreset)this.blendMode.floatValue);
            this.SetMaterialAutoPropertiesAndKeywords(mat);
        }
    }

    protected override void ShowMainGUI(MaterialEditor matEditor)
    {
        this.ShowBlendPresetGUI(matEditor);

        var mode = (BlendPreset)this.blendMode.floatValue;
        var mat = matEditor.target as Material;

        //ShaderProp(matEditor, "_UseVertexColor");

        MaterialEditorUtils.TextureColorToggleInline(matEditor, Styles.main, this.propsByName["_MainTex"], this.propsByName["_UseMainColor"], this.propsByName["_Color"]);

        ShaderGUIUtils.HeaderSection("Global UV", ()=>
        {
            MaterialEditorUtils.STVector4Prop(matEditor, Styles.textureScaleAndOffset, this.propsByName["_TextureScaleOffset"]);
        });
        
        ShowBlendPresetGUI(matEditor, mat, mode);

        EditorGUILayout.Separator();        
        ShaderGUIUtils.HeaderAutoSection(matEditor, "Advanced Settings", this.propsByName["_ShowAdvanced"], ()=>
        {
            HeaderSectionWithProps("Output Configuration", matEditor, "_Cull", "_ZTest", "_ZWrite", "_ColorWriteMask");
        });
        matEditor.RenderQueueField();
    }

    protected virtual void ShowBlendPresetGUI(MaterialEditor matEditor, Material mat, BlendPreset mode)
    {
        if (mode != BlendPreset.Cutout && mode != BlendPreset.Custom) { return; }

        ShaderGUIUtils.HeaderSection("Alpha Blending", ()=>       
        {
            if (mode == BlendPreset.Custom) { ShaderProps(matEditor, "_AlphaTest", "_AlphaPremultiply"); }

            if ( (mode == BlendPreset.Cutout) ||
                 ((mode == BlendPreset.Custom) && (this.propsByName["_AlphaTest"].floatValue >= 0f)))
            {
                ShaderProp(matEditor, "_Cutoff");
            }

            if (mode == BlendPreset.Custom) { ShaderProps(matEditor, "_SrcBlend", "_DstBlend", "_BlendOp"); }
        });
    }

    public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(mat, oldShader, newShader);

        //base.AssignNewShaderToMaterial gets weird as I use a global _TextureScaleOffset, so do this here
        if (mat.HasProperty("_MainTex"))
        {
            var s = mat.GetTextureScale("_MainTex");
            var o = mat.GetTextureOffset("_MainTex");
            mat.SetVector("_TextureScaleOffset", new Vector4(s.x, s.y, o.x, o.y));
        }       

        var blendMode = BlendPreset.Opaque;

        if (oldShader != null)
        {
            bool legacy = oldShader.name.Contains("Legacy Shaders/");
            bool mobile = oldShader.name.Contains("Mobile/");

            if (mobile || legacy)
            {
                bool transparent = oldShader.name.Contains("Transparent/");
                bool cutout = oldShader.name.Contains("Transparent/Cutout/");

                if      (cutout)      { blendMode = BlendPreset.Cutout; }
                else if (transparent) { blendMode = BlendPreset.Transparent; }
            }
        }

        this.SetMaterialBlendPreset(mat, blendMode);
        this.SetMaterialAutoPropertiesAndKeywords(mat);
    }

    protected virtual void ShowBlendPresetGUI(MaterialEditor matEditor)
    {
        EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
        var mode = (BlendPreset)this.blendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendPreset)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            matEditor.RegisterPropertyChangeUndo("Rendering Mode");
            this.blendMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }

    protected virtual void SetMaterialAutoPropertiesAndKeywords(Material mat)
    {
        var texScaleOffset = mat.GetVector("_TextureScaleOffset");
        bool usesScale = (texScaleOffset.x != 1f) || (texScaleOffset.y != 1f);
        bool usesOffset = (texScaleOffset.z != 0f) || (texScaleOffset.w != 0f);

        mat.SetKeyword("_MainTex_SCALE_ON", usesScale);
        mat.SetKeyword("_MainTex_OFFSET_ON", usesOffset);
    }
    protected static class Styles
    {
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendPreset));

        public static string renderingMode = "Rendering Mode";

        public static GUIContent main = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");

        public static GUIContent textureScaleAndOffset = new GUIContent("Texture Scale and Offset", "Applies to all textures");
    }
}