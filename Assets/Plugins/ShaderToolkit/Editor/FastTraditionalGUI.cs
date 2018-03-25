using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FastTraditionalGUI : AdvancedShaderGUI
{
    public enum BlendMode { Opaque, Cutout, Fade, Custom, Transparent }
    protected MaterialProperty blendMode;

    public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        CacheProperties(props);
        CheckAndHandleFirstTimeApply(matEditor);

        EditorGUIUtility.labelWidth = 0f;
        EditorGUI.BeginChangeCheck();
        {
            this.ShowMainGUI(matEditor);
            EditorGUILayout.Separator();    
            this.ShowAdvancedGUI(matEditor);
            EditorGUILayout.Separator();
            matEditor.RenderQueueField();

        }
        if (EditorGUI.EndChangeCheck())
        {
            UpdateAllTargets();
        }
   }

    protected virtual void CacheProperties(MaterialProperty[] props)
    {
        // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        this.blendMode = ShaderGUI.FindProperty("_Mode", props);
        propsByName = PropertiesToDictionary(props);
    }

    protected virtual void CheckAndHandleFirstTimeApply(MaterialEditor matEditor)
    {
        if (!this.firstTimeApply) { return; }

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching from an existing material.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        var matTarget = matEditor.target as Material;
        this.SetMaterialBlendMode(matTarget, (BlendMode)this.blendMode.floatValue);
        this.SetMaterialAutoPropertiesAndKeywords(matTarget);
        this.firstTimeApply = false;
    }

    protected virtual void UpdateAllTargets()
    {
        foreach (var obj in this.blendMode.targets)
        {
            var mat = obj as Material;
            this.SetMaterialBlendMode(mat, (BlendMode)this.blendMode.floatValue);
            this.SetMaterialAutoPropertiesAndKeywords(mat);
        }
    }

    protected virtual void ShowMainGUI(MaterialEditor matEditor)
    {
        this.ShowBlendModeGUI(matEditor);

        var mode = (BlendMode)this.blendMode.floatValue;
        var mat = matEditor.target as Material;

        ShaderProp(matEditor, "_UseVertexColor");

        MaterialEditorUtils.TextureColorToggleAutoSTInline(matEditor, Styles.main,
            this.propsByName["_MainTex"], this.propsByName["_UseMainColor"], 
            this.propsByName["_Color"], this.propsByName["_TextureScaleOffset"]);

        ShowLightingGUI(matEditor, mat);

        ShaderGUIUtils.HeaderSection("Global UV", ()=>
        {
            MaterialEditorUtils.STVector4Prop(matEditor, Styles.textureScaleAndOffset, this.propsByName["_TextureScaleOffset"]);
        });
        
        ShowBlendGUI(matEditor, mat, mode);
    }

    protected virtual void ShowBlendModeGUI(MaterialEditor matEditor)
    {
        EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
        var mode = (BlendMode)this.blendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup("Rendering Mode", (int)mode, Styles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            matEditor.RegisterPropertyChangeUndo("Rendering Mode");
            this.blendMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    } 

    protected virtual void ShowLightingGUI(MaterialEditor matEditor, Material mat)
    {
        ShaderGUIUtils.HeaderAutoSection(matEditor, Styles.specularLightingEnabled.text, this.propsByName["_SpecularHighlights"], ()=>
        {
            matEditor.TexturePropertySingleLine(Styles.specularMap, this.propsByName["_SpecularMap"]);
        });

        //matEditor.ShaderProperty(this.normalMap, Styles.normalMap);           
        matEditor.TexturePropertySingleLine(Styles.normalMap, this.propsByName["_BumpMap"]);

        ShaderGUIUtils.HeaderAutoSection(matEditor, "Rim Lighting", this.propsByName["_UseRimLighting"], ()=>
        {
            ShaderProps(matEditor, "_RimPower", "_RimColor");
        });
        
        ShaderGUIUtils.HeaderAutoSection(matEditor, Styles.reflectionsEnabled.text, this.propsByName["_UseReflections"], ()=>
        {
            matEditor.TexturePropertySingleLine(Styles.cubeMap, this.propsByName["_CubeMap"]);
            ShaderProp(matEditor, "_ReflectionScale");
        });
        
        MaterialEditorUtils.TextureColorToggleInline(matEditor, Styles.emission, this.propsByName["_EmissionMap"], this.propsByName["_UseEmissionColor"], this.propsByName["_EmissionColor"]);
    }

    protected virtual void ShowBlendGUI(MaterialEditor matEditor, Material mat, BlendMode mode)
    {
        if (mode != BlendMode.Cutout && mode != BlendMode.Custom) { return; }

        ShaderGUIUtils.HeaderSection("Alpha Blending", ()=>       
        {
            if (mode == BlendMode.Custom) { ShaderProps(matEditor, "_AlphaTest", "_AlphaPremultiply"); }

            if ( (mode == BlendMode.Cutout) ||
                 ((mode == BlendMode.Custom) && (this.propsByName["_AlphaTest"].floatValue >= 0f)))
            {
                ShaderProp(matEditor, "_Cutoff");
            }

            ShaderProps(matEditor, "_SrcBlend", "_DstBlend", "_BlendOp");
        });
    }

    protected virtual void ShowAdvancedGUI(MaterialEditor matEditor)
    {
        ShaderGUIUtils.HeaderAutoSection(matEditor, "Advanced Settings", this.propsByName["_ShowAdvanced"], ()=>
        {
            ShaderGUIUtils.HeaderSection("Lighting", ()=>
            {
                ShaderProps(matEditor, "_UseAmbient", "_UseDiffuse", "_Shade4");
            });

            ShaderGUIUtils.HeaderSection("Specular Weights", ()=>
            {
                ShaderProps(matEditor, "_Specular", "_Gloss");
            });

            ShaderGUIUtils.HeaderSection("Shadows", ()=>
            {
                ShaderProps(matEditor, "_UseNormalOffsetShadows", "_UseSemiTransparentShadows");
            });

            ShaderGUIUtils.HeaderSection("Output Configuration", ()=>
            {
                ShaderProps(matEditor, "_Cull", "_ZTest", "_ZWrite", "_ColorWriteMask");
            });
        }); 
    } 

    public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader)
    {
        // _Emission property is lost after assigning, transfer it before assigning the new shader
        if (mat.HasProperty("_Emission"))  { mat.SetColor("_EmissionColor", mat.GetColor("_Emission")); }

        //try and grab things with non-unity standard naming
        if (mat.HasProperty("_NormalMap")) { mat.SetTexture("_BumpMap", mat.GetTexture("_NormalMap")); }

        base.AssignNewShaderToMaterial(mat, oldShader, newShader);

        //base.AssignNewShaderToMaterial gets weird as I use a global _TextureScaleOffset, so do this here
        if (mat.HasProperty("_MainTex"))
        {
            var s = mat.GetTextureScale("_MainTex");
            var o = mat.GetTextureOffset("_MainTex");
            mat.SetVector("_TextureScaleOffset", new Vector4(s.x, s.y, o.x, o.y));
        }

        //TODO: might not get pulled in from standard?
        var blendMode = BlendMode.Opaque;

        if (oldShader != null)
        {
            bool standard = oldShader.name.Contains("Standard");
            bool legacy = oldShader.name.Contains("Legacy Shaders/");
            bool mobile = oldShader.name.Contains("Mobile/");

            if (standard)
            {
                //TODO: toggle on albedo if map or color set
                //TODO: pull blend mode?
                this.SetMaterialLighting(mat, true, true, mat.TryGetToggle("_SpecularHighlights", true), true);
            } 
            else if (mobile || legacy)
            {
                bool transparent = oldShader.name.Contains("Transparent/");
                bool cutout = oldShader.name.Contains("Transparent/Cutout/");
                bool unlit = oldShader.name.Contains("Unlit");
                bool directionalLightOnly = oldShader.name.Contains("DirectionalLight");
                bool vertexLit = oldShader.name.Contains("VertexLit");
                bool spec = !oldShader.name.Contains("Diffuse");

                if      (cutout)      { blendMode = BlendMode.Cutout; }
                else if (transparent) { blendMode = BlendMode.Transparent; }
                this.SetMaterialLighting(mat, false, false, unlit ? false : spec, unlit ? false : !directionalLightOnly);
            }            
        }     

        this.SetMaterialBlendMode(mat, blendMode);
        this.SetMaterialAutoPropertiesAndKeywords(mat);
    }

    protected virtual void SetMaterialLighting(Material mat, bool ambient, bool diffuse, bool specular, bool additional)
    {
        mat.SetToggle("_UseAmbient", ambient);
        mat.SetToggle("_UseDiffuse", diffuse);
        mat.SetToggle("_SpecularHighlights", specular);
        mat.SetToggle("_Shade4", additional);
    }
    
    protected virtual void SetMaterialBlendMode(Material mat, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                mat.SetOverrideTag("RenderType", string.Empty);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.SetKeyword("_ALPHATEST_ON", false);
                mat.SetKeyword("_ALPHAPREMULTIPLY_ON", false);     
                mat.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                mat.SetOverrideTag("RenderType", "TransparentCutout");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.SetKeyword("_ALPHATEST_ON", true);
                mat.SetKeyword("_ALPHAPREMULTIPLY_ON", false);                   
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.SetKeyword("_ALPHATEST_ON", false);
                mat.SetKeyword("_ALPHAPREMULTIPLY_ON", false);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;                    
            case BlendMode.Transparent:
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.SetKeyword("_ALPHATEST_ON", false);
                mat.SetKeyword("_ALPHAPREMULTIPLY_ON", true);                    
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }

    protected virtual void SetMaterialAutoPropertiesAndKeywords(Material mat)
    {
        mat.SetKeyword("_USEMAINTEX_ON", mat.HasTexture("_MainTex"));
        mat.SetKeyword("_USEOCCLUSIONMAP_ON", mat.HasTexture("_OcclusionMap"));
        mat.SetKeyword("_USECUSTOMCUBEMAP_ON", mat.HasTexture("_CubeMap"));
        mat.SetKeyword("_USEBUMPMAP_ON", mat.HasTexture("_BumpMap"));
        mat.SetKeyword("_USESPECULARMAP_ON", mat.HasTexture("_SpecularMap"));
        mat.SetKeyword("_USEEMISSIONMAP_ON", mat.HasTexture("_EmissionMap"));       

        var texScaleOffset = mat.GetVector("_TextureScaleOffset");
        mat.SetKeyword("_MainTex_SCALE_ON", (texScaleOffset.x != 1f) || (texScaleOffset.y != 1f));
        mat.SetKeyword("_MainTex_OFFSET_ON", (texScaleOffset.z != 0f) || (texScaleOffset.w != 0f));
    }

    protected static class Styles
    {
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));

        public static GUIContent main = new GUIContent("Albedo", "Albedo(RGB) Alpha(A)");

        //specular lighting
        public static GUIContent specularLightingEnabled = new GUIContent("Specular Highlights", "Specular (blinn-phong) lighting from directional lights");
        public static GUIContent specularMap = new GUIContent("Spec(R) Gloss(A)", "");
        public static GUIContent normalMap = new GUIContent("Normal Map", "Normal Map - will turn on per-pixel lighting");

        //reflections
        public static GUIContent reflectionsEnabled = new GUIContent("Reflections", "Cube map based reflections");
        public static GUIContent cubeMap = new GUIContent("Cube Map", "Cube map lookup for reflections");

        //emission
        public static GUIContent emission = new GUIContent("Emission", "Emission (RGB)");

        public static GUIContent textureScaleAndOffset = new GUIContent("Texture Scale and Offset", "Applies to all textures");
    }
}
