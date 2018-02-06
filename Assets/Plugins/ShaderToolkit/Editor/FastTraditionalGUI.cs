using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor for FastTraditional shader
/// </summary>
public class FastTraditionalGUI : ShaderGUI
{
    protected bool firstTimeApply = true;

    //cached material properties
    protected MaterialProperty blendMode;

    protected MaterialProperty vertexColorEnabled;
    protected MaterialProperty mainColorEnabled;
    protected MaterialProperty mainColor;
    protected MaterialProperty mainTexture;

    protected MaterialProperty ambientLightingEnabled;
    protected MaterialProperty diffuseLightingEnabled;
    protected MaterialProperty useAdditionalLightingData;

    //specular lighting
    protected MaterialProperty specularLightingEnabled;
    protected MaterialProperty specularColor;
    protected MaterialProperty specular;
    protected MaterialProperty specularMap;
    protected MaterialProperty gloss;
    protected MaterialProperty glossMap;

    protected MaterialProperty normalMap;

    //reflections
    protected MaterialProperty reflectionsEnabled;
    protected MaterialProperty cubeMap;
    protected MaterialProperty reflectionScale;

    //rim lighting
    protected MaterialProperty rimLightingEnabled;
    protected MaterialProperty rimPower;
    protected MaterialProperty rimColor;    

    //emission
    protected MaterialProperty emissionColorEnabled;
    protected MaterialProperty emissionColor;
    protected MaterialProperty emissionMap;

    protected MaterialProperty useTextureScale;
    protected MaterialProperty useTextureOffset;
    protected MaterialProperty textureScaleAndOffset;

    //shadows
    protected MaterialProperty normalOffsetShadowsEnabled;
    protected MaterialProperty transparentShadowsEnabled;

    //alpha
    protected MaterialProperty alphaTestEnabled;
    protected MaterialProperty alphaCutoff;
    protected MaterialProperty alphaPremultiplyEnabled;

    //rasterizer state
    protected MaterialProperty srcBlend;
    protected MaterialProperty dstBlend;
    protected MaterialProperty blendOp;

    protected MaterialProperty cullMode;
    protected MaterialProperty zTest;
    protected MaterialProperty zWrite;
    protected MaterialProperty colorWriteMask;

    public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        this.CacheMainProperties(props);
        this.CacheOutputConfigurationProperties(props);

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching from an existing material.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        if (this.firstTimeApply)
        {
            var matTarget = matEditor.target as Material;
            this.SetMaterialBlendMode(matTarget, (BlendMode)this.blendMode.floatValue);
            this.SetMaterialAutoPropertiesAndKeywords(matTarget);
            this.firstTimeApply = false;
        }

        EditorGUIUtility.labelWidth = 0f;

        EditorGUI.BeginChangeCheck();
        {
            this.ShowMainGUI(matEditor);
            this.ShowOutputConfigurationGUI(matEditor);

            //if ((BlendMode)this.blendMode.floatValue == BlendMode.Advanced)
            {
                matEditor.RenderQueueField();
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in this.blendMode.targets)
            {
                var mat = obj as Material;
                this.SetMaterialBlendMode(mat, (BlendMode)this.blendMode.floatValue);
                this.SetMaterialAutoPropertiesAndKeywords(mat);
            }
        }
    }

    protected virtual void ShowLightingGUI(MaterialEditor matEditor, Material mat)
    {
        ShaderGUIUtils.HeaderSection("Lighting", ()=>
        {
            matEditor.ShaderProperty(this.ambientLightingEnabled, Styles.ambientLightingEnabled);
            matEditor.ShaderProperty(this.diffuseLightingEnabled, Styles.diffuseLightingEnabled);
            matEditor.ShaderProperty(this.useAdditionalLightingData, Styles.useAdditionalLightingData);

            //specular lighting
            ShaderGUIUtils.HeaderAutoSection(matEditor, Styles.specularLightingEnabled.text, this.specularLightingEnabled, ()=>
            {
                matEditor.TexturePropertySingleLine(Styles.specularMap, this.specularMap);
                matEditor.ShaderProperty(this.gloss, Styles.gloss);
                matEditor.ShaderProperty(this.specular, Styles.specular);                    
            });

            //matEditor.ShaderProperty(this.normalMap, Styles.normalMap);           
            matEditor.TexturePropertySingleLine(Styles.normalMap, this.normalMap);

            //rim lighting
            ShaderGUIUtils.HeaderAutoSection(matEditor, Styles.rimLightingEnabled.text, this.rimLightingEnabled, ()=>
            {
                matEditor.ShaderProperty(this.rimPower, Styles.rimPower);
                matEditor.ShaderProperty(this.rimColor, Styles.rimColor);
            });

            //reflections
            ShaderGUIUtils.HeaderAutoSection(matEditor, Styles.reflectionsEnabled.text, this.reflectionsEnabled, ()=>
            {
                matEditor.TexturePropertySingleLine(Styles.cubeMap, this.cubeMap);
                matEditor.ShaderProperty(this.reflectionScale, Styles.reflectionScale);
            });

            CustomMaterialEditorUtils.TextureWithToggleableColorSingleLine(
                matEditor, Styles.emission, this.emissionMap, this.emissionColorEnabled, this.emissionColor);
        });
    }

    protected virtual void ShowShadowsGUI(MaterialEditor matEditor, Material mat)
    {
        ShaderGUIUtils.HeaderSection("Shadows", ()=>       
        {
            matEditor.ShaderProperty(this.normalOffsetShadowsEnabled, Styles.normalOffsetShadowsEnabled);
            matEditor.ShaderProperty(this.transparentShadowsEnabled, Styles.transparentShadowsEnabled);
        });
    }

    protected virtual void ShowBlendGUI(MaterialEditor matEditor, Material mat, BlendMode mode)
    {
        //alpha
        if (mode != BlendMode.Cutout && mode != BlendMode.Advanced)
        {
            return;
        }

        ShaderGUIUtils.HeaderSection("Alpha Blending", ()=>       
        {
            if (mode == BlendMode.Advanced)
            {
                matEditor.ShaderProperty(this.alphaTestEnabled, Styles.alphaTestEnabled.text);
                matEditor.ShaderProperty(this.alphaPremultiplyEnabled, Styles.alphaPremultiplyEnabled.text);
            }

            //todo: simplify check
            bool usesAlphaCutoff = (mode == BlendMode.Cutout) ||
                                   ((mode == BlendMode.Advanced) && (this.alphaTestEnabled.floatValue >= 0f));

            if (usesAlphaCutoff)
            {
                matEditor.ShaderProperty(this.alphaCutoff, Styles.alphaCutoff);
            }

            //rasterizer state
            matEditor.ShaderProperty(this.srcBlend, Styles.srcBlend);
            matEditor.ShaderProperty(this.dstBlend, Styles.dstBlend);
            matEditor.ShaderProperty(this.blendOp, Styles.blendOp);
        });
    }

    protected virtual void ShowMainGUI(MaterialEditor matEditor)
    {
        this.ShowBlendModeGUI(matEditor);

        var mode = (BlendMode)this.blendMode.floatValue;
        var mat = matEditor.target as Material;

        ShaderGUIUtils.HeaderSection("Object Color", ()=>
        {
            matEditor.ShaderProperty(this.vertexColorEnabled, Styles.vertexColorEnabled);

            CustomMaterialEditorUtils.TextureWithToggleableColorAutoScaleOffsetSingleLine(
                matEditor, Styles.main, this.mainTexture, this.mainColorEnabled, this.mainColor, this.textureScaleAndOffset);
        });

        ShowLightingGUI(matEditor, mat);

        ShaderGUIUtils.HeaderSection("Global", ()=>
        {
            CustomMaterialEditorUtils.TextureScaleOffsetVector4Property(matEditor, Styles.textureScaleAndOffset, this.textureScaleAndOffset);
        });
        
        ShowShadowsGUI(matEditor, mat);
        ShowBlendGUI(matEditor, mat, mode);
    }

    public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader)
    {
        // _Emission property is lost after assigning, transfer it before assigning the new shader
        if (mat.HasProperty("_Emission"))
        {
            mat.SetColor("_EmissionColor", mat.GetColor("_Emission"));
        }

        //try and grab things with non-unity standard naming
        if (mat.HasProperty("_NormalMap"))
        {
            mat.SetTexture("_BumpMap", mat.GetTexture("_NormalMap"));
        }

        base.AssignNewShaderToMaterial(mat, oldShader, newShader);

        //shared scale and offset
        //pull them from the main texture on the source material if available

        //base.AssignNewShaderToMaterial gets weird as I use a globally applicable
        //_TextureScaleOffset, so do this here
        if (mat.HasProperty("_MainTex"))
        {
            var s = mat.GetTextureScale("_MainTex");
            var o = mat.GetTextureOffset("_MainTex");

            Vector4 so = new Vector4(s.x, s.y, o.x, o.y);
            Debug.Log(so);
            mat.SetVector("_TextureScaleOffset", so);
        }

        var blendMode = BlendMode.Opaque;

        if (oldShader != null)
        {
            bool standard = oldShader.name.Contains("Standard");
            bool legacy = oldShader.name.Contains("Legacy Shaders/");
            bool mobile = oldShader.name.Contains("Mobile/");

            bool transparent = oldShader.name.Contains("Transparent/");
            bool cutout = oldShader.name.Contains("Transparent/Cutout/");
            bool unlit = oldShader.name.Contains("Unlit");
            bool directionalLightOnly = oldShader.name.Contains("DirectionalLight");
            bool vertexLit = oldShader.name.Contains("VertexLit");
            bool spec = !oldShader.name.Contains("Diffuse");

            if (standard)
            {
                //TODO: toggle on albedo if map or color set
                this.SetMaterialLighting(mat, true, true, mat.TryGetToggle("_SpecularHighlights", true), true, true);
            } 
            else if (mobile || legacy)
            {
                if (cutout)
                {
                    blendMode = BlendMode.Cutout;
                }
                else if (transparent)
                {
                    blendMode = BlendMode.Transparent;
                }

                if (unlit)
                {
                    this.SetMaterialLighting(mat, false, false, false, false, false);
                }
                else
                {
                    //TODO: need to handle way more cases
                    this.SetMaterialLighting(mat, true, true, spec, !directionalLightOnly, !vertexLit);
                }
            }            
        }     

        this.SetMaterialBlendMode(mat, blendMode);
        this.SetMaterialAutoPropertiesAndKeywords(mat);
    }

    protected virtual void ShowBlendModeGUI(MaterialEditor matEditor)
    {
        EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
        var mode = (BlendMode)this.blendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            matEditor.RegisterPropertyChangeUndo("Rendering Mode");
            this.blendMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }

    protected virtual void ShowOutputConfigurationGUI(MaterialEditor matEditor)
    {
        var mode = (BlendMode)this.blendMode.floatValue;
        if (mode == BlendMode.Advanced)
        {
            ShaderGUIUtils.HeaderSection("Output Configuration", ()=>
            {
                matEditor.ShaderProperty(this.cullMode, Styles.cullMode);
                matEditor.ShaderProperty(this.zTest, Styles.zTest);
                matEditor.ShaderProperty(this.zWrite, Styles.zWrite);
                matEditor.ShaderProperty(this.colorWriteMask, Styles.colorWriteMask);
            });
        }

        EditorGUILayout.Separator();
    }

    protected virtual void SetMaterialLighting(Material mat, bool ambient, bool diffuse, bool specular, bool additional, bool perPixel)
    {
        mat.SetToggle("_UseAmbient", ambient);
        mat.SetToggle("_UseDiffuse", diffuse);
        mat.SetToggle("_SpecularHighlights", specular);
        mat.SetToggle("_Shade4", additional);

        mat.SetToggle("_ForcePerPixel", perPixel);
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
            case BlendMode.Advanced:
                //user configured
                break;
        }
    }

    protected virtual bool MaterialNeedsPerPixel(Material mat)
    {
        return mat.HasTexture("_BumpMap") || mat.HasTexture("_SpecularMap") || mat.HasTexture("_EmissionMap");
    }

    protected virtual void SetMaterialAutoPropertiesAndKeywords(Material mat)
    {
        mat.SetKeyword("_USEMAINTEX_ON", mat.HasTexture("_MainTex"));
        mat.SetKeyword("_USEOCCLUSIONMAP_ON", mat.HasTexture("_OcclusionMap"));

        mat.SetKeyword("_USECUSTOMCUBEMAP_ON", mat.HasTexture("_CubeMap"));

        mat.SetKeyword("_USEBUMPMAP_ON", mat.HasTexture("_BumpMap"));
        mat.SetKeyword("_USESPECULARMAP_ON", mat.HasTexture("_SpecularMap"));

        var texScaleOffset = mat.GetVector("_TextureScaleOffset");
        bool usesScale = (texScaleOffset.x != 1f) || (texScaleOffset.y != 1f);
        bool usesOffset = (texScaleOffset.z != 0f) || (texScaleOffset.w != 0f);

        mat.SetKeyword("_MainTex_SCALE_ON", usesScale);
        mat.SetKeyword("_MainTex_OFFSET_ON", usesOffset);
    }

    protected virtual void CacheMainProperties(MaterialProperty[] props)
    {
        this.blendMode = ShaderGUI.FindProperty("_Mode", props);

        this.vertexColorEnabled = ShaderGUI.FindProperty("_UseVertexColor", props);
        this.mainColorEnabled = ShaderGUI.FindProperty("_UseMainColor", props);
        this.mainColor = ShaderGUI.FindProperty("_Color", props);
        this.mainTexture = ShaderGUI.FindProperty("_MainTex", props);

        this.ambientLightingEnabled = ShaderGUI.FindProperty("_UseAmbient", props);
        this.diffuseLightingEnabled = ShaderGUI.FindProperty("_UseDiffuse", props);
        this.useAdditionalLightingData = ShaderGUI.FindProperty("_Shade4", props);

        //specular lighting
        this.specularLightingEnabled = ShaderGUI.FindProperty("_SpecularHighlights", props);
        this.specular = ShaderGUI.FindProperty("_Specular", props);
        this.gloss = ShaderGUI.FindProperty("_Gloss", props);        
        this.specularMap = ShaderGUI.FindProperty("_SpecularMap", props);
        this.normalMap = ShaderGUI.FindProperty("_BumpMap", props);

        //reflections
        this.reflectionsEnabled = ShaderGUI.FindProperty("_UseReflections", props);
        this.cubeMap = ShaderGUI.FindProperty("_CubeMap", props);
        this.reflectionScale = ShaderGUI.FindProperty("_ReflectionScale", props);

        //rim lighting
        this.rimLightingEnabled = ShaderGUI.FindProperty("_UseRimLighting", props);
        this.rimPower = ShaderGUI.FindProperty("_RimPower", props);
        this.rimColor = ShaderGUI.FindProperty("_RimColor", props);        

        //emission
        this.emissionColorEnabled = ShaderGUI.FindProperty("_UseEmissionColor", props);
        this.emissionColor = ShaderGUI.FindProperty("_EmissionColor", props);
        this.emissionMap = ShaderGUI.FindProperty("_EmissionMap", props);

        this.textureScaleAndOffset = ShaderGUI.FindProperty("_TextureScaleOffset", props);

        //shadows
        this.normalOffsetShadowsEnabled = ShaderGUI.FindProperty("_UseNormalOffsetShadows", props);
        this.transparentShadowsEnabled = ShaderGUI.FindProperty("_UseSemiTransparentShadows", props);

        //alpha
        this.alphaTestEnabled = ShaderGUI.FindProperty("_AlphaTest", props);
        this.alphaCutoff = ShaderGUI.FindProperty("_Cutoff", props);
        this.alphaPremultiplyEnabled = ShaderGUI.FindProperty("_AlphaPremultiply", props);

        //rasterizer state
        this.srcBlend = ShaderGUI.FindProperty("_SrcBlend", props);
        this.dstBlend = ShaderGUI.FindProperty("_DstBlend", props);
        this.blendOp = ShaderGUI.FindProperty("_BlendOp", props);
    }

    protected virtual void CacheOutputConfigurationProperties(MaterialProperty[] props)
    {
        this.cullMode = ShaderGUI.FindProperty("_Cull", props);
        this.zTest = ShaderGUI.FindProperty("_ZTest", props);
        this.zWrite = ShaderGUI.FindProperty("_ZWrite", props);
        this.colorWriteMask = ShaderGUI.FindProperty("_ColorWriteMask", props);
    }

    protected static class Styles
    {
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));

        public static string renderingMode = "Rendering Mode";

        public static GUIContent vertexColorEnabled = new GUIContent("Vertex Color", "Utilize vertex color from the model?");
        public static GUIContent main = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
        public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");

        public static GUIContent occlusionMap = new GUIContent("Occlusion", "Additional texture to be overlaid on the main texture");

        public static GUIContent ambientLightingEnabled = new GUIContent("Ambient", "Scene ambient lighting");
        public static GUIContent diffuseLightingEnabled = new GUIContent("Diffuse", "Diffuse (lambertian) lighting from directional lights");
        public static GUIContent useAdditionalLightingData = new GUIContent("Point and Spot", "Apply lighting from point and spot lights that don't get a fwdadd pass");

        //specular lighting
        public static GUIContent specularLightingEnabled = new GUIContent("Specular Highlights", "Specular (blinn-phong) lighting from directional lights");
        public static GUIContent specularMap = new GUIContent("Color (RGB) Gloss (A)", "Color can also store intensity");
        public static GUIContent specularColor = new GUIContent(" Color", "Tint to apply to specular highlights");
        public static GUIContent specular = new GUIContent("Power", "Exponential strength of specular - using a map will turn on per-pixel lighting");
        public static GUIContent gloss = new GUIContent("Gloss", "Linear strength of specular - using a map will turn on per-pixel lighting");
        public static GUIContent normalMap = new GUIContent("Normal Map", "Normal Map - will turn on per-pixel lighting");

        //reflections
        public static GUIContent reflectionsEnabled = new GUIContent("Reflections", "Cube map based reflections");
        public static GUIContent cubeMap = new GUIContent("Cube Map", "Cube map lookup for reflections");
        public static GUIContent reflectionScale = new GUIContent("Scale", "Reflection strength");       
        
        //rim lighting
        public static GUIContent rimLightingEnabled = new GUIContent("Rim Lighting", "Side lighting");
        public static GUIContent rimPower = new GUIContent("Strength", "Strength of rim lighting");
        public static GUIContent rimColor = new GUIContent("Color", "Color of rim lighting");

        //emission
        public static GUIContent emission = new GUIContent("Emission", "Emission (RGB)");

        public static GUIContent textureScaleAndOffset = new GUIContent("Texture Scale and Offset", "Applies to all textures");

        //shadows
        //http://c0de517e.blogspot.com/2011/05/shadowmap-bias-notes.html
        public static GUIContent normalOffsetShadowsEnabled = new GUIContent("Normal Offset", "Offset along normal before projecting into shadow space");           
        public static GUIContent transparentShadowsEnabled = new GUIContent("Semi-Transparent", "Enables dithered shadow transparency based on object alpha");

        //alpha
        public static GUIContent alphaTestEnabled = new GUIContent("Alpha Test", "Enables rejection of pixels based on alpha and cutoff");
        public static GUIContent alphaCutoff = new GUIContent("Alpha Cutoff", "Pixels with alpha below this value will be rejected");
        public static GUIContent alphaPremultiplyEnabled = new GUIContent("Premultiply Alpha", "Premultiply RGB by alpha");

        //rasterizer state
        public static GUIContent srcBlend = new GUIContent("Source Blend", "Blend factor for transparency, etc.");
        public static GUIContent dstBlend = new GUIContent("Destination Blend", "Blend factor for transparency, etc.");
        public static GUIContent blendOp = new GUIContent("Blend Operation", "Blend operation for transparency, etc.");

        public static GUIContent cullMode = new GUIContent("Culling Mode", "Type of culling to apply to polygons - typically this is set to backfacing");
        public static GUIContent zTest = new GUIContent("Z Test", "Depth buffer check type - output is not written if this is false");
        public static GUIContent zWrite = new GUIContent("Z Write", "When to write to the depth buffer");
        public static GUIContent colorWriteMask = new GUIContent("Color Write Mask", "Restricts output to specified color channels only");
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Advanced,
        Transparent
    }
}
