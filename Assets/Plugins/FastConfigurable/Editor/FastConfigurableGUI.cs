// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity
{
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Editor for FastConfigurable shader
    /// </summary>
    public class FastConfigurableGUI : ShaderGUI
    {
        protected bool firstTimeApply = true;

        //cached material properties
        protected MaterialProperty blendMode;

        protected MaterialProperty vertexColorEnabled;
        protected MaterialProperty mainColorEnabled;
        protected MaterialProperty mainColor;
        protected MaterialProperty mainTexture;
        protected MaterialProperty alphaCutoff;
        protected MaterialProperty occlusionMap;

        protected MaterialProperty ambientLightingEnabled;
        protected MaterialProperty diffuseLightingEnabled;
        protected MaterialProperty useAdditionalLightingData;
        protected MaterialProperty perPixelLighting;

        protected MaterialProperty specularLightingEnabled;
        protected MaterialProperty specularColor;
        protected MaterialProperty specular;
        protected MaterialProperty specularMap;
        protected MaterialProperty gloss;
        protected MaterialProperty glossMap;

        protected MaterialProperty normalMap;

        protected MaterialProperty reflectionsEnabled;
        protected MaterialProperty cubeMap;
        protected MaterialProperty reflectionScale;
        protected MaterialProperty calibrationSpaceReflections;

        protected MaterialProperty rimLightingEnabled;
        protected MaterialProperty rimPower;
        protected MaterialProperty rimColor;

        protected MaterialProperty emissionColorEnabled;
        protected MaterialProperty emissionColor;
        protected MaterialProperty emissionMap;

        protected MaterialProperty useTextureScale;
        protected MaterialProperty useTextureOffset;
        protected MaterialProperty textureScaleAndOffset;

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
                this.SetMaterialAutoPropertiesAndKeywords(matEditor.target as Material);
                this.firstTimeApply = false;
            }

            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();
            {
                this.ShowMainGUI(matEditor);
                this.ShowOutputConfigurationGUI(matEditor);

                var mode = (BlendMode)this.blendMode.floatValue;
                if (mode == BlendMode.Advanced)
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

        protected virtual void ShowMainGUI(MaterialEditor matEditor)
        {
            this.ShowBlendModeGUI(matEditor);

            var mode = (BlendMode)this.blendMode.floatValue;
            var mat = matEditor.target as Material;

            ShaderGUIUtils.BeginHeader("Base Texture and Color");
            {
                matEditor.ShaderProperty(this.vertexColorEnabled, Styles.vertexColorEnabled);

                CustomMaterialEditor.TextureWithToggleableColorAutoScaleOffsetSingleLine(matEditor, Styles.main, this.mainTexture, this.mainColorEnabled, this.mainColor, this.textureScaleAndOffset);

                matEditor.TexturePropertySingleLine(Styles.occlusionMap, this.occlusionMap);

                if (mode == BlendMode.Cutout || mode == BlendMode.Advanced)
                {
                    matEditor.ShaderProperty(this.alphaCutoff, Styles.alphaCutoffText.text);
                }
            }
            ShaderGUIUtils.EndHeader();
            ShaderGUIUtils.HeaderSeparator();

            ShaderGUIUtils.BeginHeader("Lighting");
            {
                matEditor.ShaderProperty(this.ambientLightingEnabled, Styles.ambientLightingEnabled);
                matEditor.ShaderProperty(this.diffuseLightingEnabled, Styles.diffuseLightingEnabled);
                matEditor.ShaderProperty(this.useAdditionalLightingData, Styles.useAdditionalLighingData);
                EditorGUI.BeginDisabledGroup(this.MaterialNeedsPerPixel(mat));
                matEditor.ShaderProperty(this.perPixelLighting, Styles.perPixelLighting);
                EditorGUI.EndDisabledGroup();

                ShaderGUIUtils.BeginHeaderProperty(matEditor, Styles.specularLightingEnabled.text, this.specularLightingEnabled);
                {
                    if (this.specularLightingEnabled.floatValue > 0.0f)
                    {
                        matEditor.ShaderProperty(this.specularColor, Styles.specularColor);

                        //consider a special slider + tex control
                        matEditor.TexturePropertySingleLine(Styles.specular, this.specularMap, this.specular);
                        matEditor.TexturePropertySingleLine(Styles.gloss, this.glossMap, this.gloss);
                    }
                }
                ShaderGUIUtils.EndHeader();

                matEditor.TexturePropertySingleLine(Styles.normalMap, this.normalMap);

                ShaderGUIUtils.BeginHeaderProperty(matEditor, Styles.rimLightingEnabled.text, this.rimLightingEnabled);
                {
                    if (this.rimLightingEnabled.floatValue > 0.0f)
                    {
                        matEditor.ShaderProperty(this.rimPower, Styles.rimPower);
                        matEditor.ShaderProperty(this.rimColor, Styles.rimColor);
                    }
                }
                ShaderGUIUtils.EndHeader();

                ShaderGUIUtils.BeginHeaderProperty(matEditor, Styles.reflectionsEnabled.text, this.reflectionsEnabled);
                {
                    if (this.reflectionsEnabled.floatValue > 0.0f)
                    {
                        matEditor.TexturePropertySingleLine(Styles.cubeMap, this.cubeMap);
                        matEditor.ShaderProperty(this.reflectionScale, Styles.reflectionScale);
                        matEditor.ShaderProperty(this.calibrationSpaceReflections, Styles.calibrationSpaceReflections);
                    }
                }
                ShaderGUIUtils.EndHeader();

                CustomMaterialEditor.TextureWithToggleableColorSingleLine(matEditor, Styles.emission, this.emissionMap, this.emissionColorEnabled, this.emissionColor);
            }
            ShaderGUIUtils.EndHeader();
            ShaderGUIUtils.HeaderSeparator();

            ShaderGUIUtils.BeginHeader("Global");
            {
                CustomMaterialEditor.TextureScaleOffsetVector4Property(matEditor, Styles.textureScaleAndOffset, this.textureScaleAndOffset);
            }
            ShaderGUIUtils.EndHeader();
            ShaderGUIUtils.HeaderSeparator();

            if (mode == BlendMode.Advanced)
            {
                ShaderGUIUtils.BeginHeader("Alpha Blending");
                {
                    matEditor.ShaderProperty(this.srcBlend, Styles.srcBlend);
                    matEditor.ShaderProperty(this.dstBlend, Styles.dstBlend);
                    matEditor.ShaderProperty(this.blendOp, Styles.blendOp);
                }
                ShaderGUIUtils.EndHeader();
                ShaderGUIUtils.HeaderSeparator();
            }
        }

        public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader)
        {
            // _Emission property is lost after assigning, transfer it before assigning the new shader
            if (mat.HasProperty("_Emission"))
            {
                mat.SetColor("_EmissionColor", mat.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(mat, oldShader, newShader);

            if (oldShader == null)
            {
                return;
            }

            BlendMode blendMode = BlendMode.Opaque;

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
                this.SetMaterialLighting(mat, true, true, ShaderGUIUtils.TryGetToggle(mat, "_SpecularHighlights", true), true, true);
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
                    this.SetMaterialLighting(mat, true, true, spec, !directionalLightOnly, vertexLit);
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
                ShaderGUIUtils.BeginHeader("Output Configuration");
                {
                    matEditor.ShaderProperty(this.cullMode, Styles.cullMode);
                    matEditor.ShaderProperty(this.zTest, Styles.zTest);
                    matEditor.ShaderProperty(this.zWrite, Styles.zWrite);
                    matEditor.ShaderProperty(this.colorWriteMask, Styles.colorWriteMask);
                }
                ShaderGUIUtils.EndHeader();
            }
        }

        protected virtual void SetMaterialLighting(Material mat, bool ambient, bool diffuse, bool specular, bool additional, bool perPixel)
        {
            mat.SetFloat("_UseAmbient", ambient ? 1.0f : 0.0f);
            mat.SetFloat("_UseDiffuse", diffuse ? 1.0f : 0.0f);
            mat.SetFloat("_SpecularHighlights", specular ? 1.0f : 0.0f);
            mat.SetFloat("_Shade4", additional ? 1.0f : 0.0f);

            mat.SetFloat("_ForcePerPixel", perPixel ? 1.0f : 0.0f);
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
                    ShaderGUIUtils.SetKeyword(mat, "_ALPHATEST_ON", false);
                    mat.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    mat.SetOverrideTag("RenderType", "TransparentCutout");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    ShaderGUIUtils.SetKeyword(mat, "_ALPHATEST_ON", true);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case BlendMode.Transparent:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    //non pre-multiplied alpha
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 1);
                    ShaderGUIUtils.SetKeyword(mat, "_ALPHATEST_ON", false);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case BlendMode.Advanced:
                    //user configured
                    break;
            }
        }

        protected virtual bool MaterialNeedsPerPixel(Material mat)
        {
            bool usesBumpMap = mat.GetTexture("_BumpMap");
            bool usesSpecMap = mat.GetTexture("_SpecularMap");
            bool usesGlossMap = mat.GetTexture("_GlossMap");
            bool usesEmissionMap = mat.GetTexture("_EmissionMap");

            return (usesBumpMap || usesSpecMap || usesGlossMap || usesEmissionMap);
        }

        protected virtual void SetMaterialAutoPropertiesAndKeywords(Material mat)
        {
            ShaderGUIUtils.SetKeyword(mat, "_USEMAINTEX_ON", mat.GetTexture("_MainTex"));
            ShaderGUIUtils.SetKeyword(mat, "_USEOCCLUSIONMAP_ON", mat.GetTexture("_OcclusionMap"));

            bool usesBumpMap = mat.GetTexture("_BumpMap");
            bool usesSpecMap = mat.GetTexture("_SpecularMap");
            bool usesGlossMap = mat.GetTexture("_GlossMap");
            bool usesEmissionMap = mat.GetTexture("_EmissionMap");

            ShaderGUIUtils.SetKeyword(mat, "_USEBUMPMAP_ON", usesBumpMap);
            ShaderGUIUtils.SetKeyword(mat, "_USESPECULARMAP_ON", usesSpecMap);
            ShaderGUIUtils.SetKeyword(mat, "_USEGLOSSMAP_ON", usesGlossMap);
            ShaderGUIUtils.SetKeyword(mat, "_USEEMISSIONMAP_ON", usesEmissionMap);

            if (usesBumpMap || usesSpecMap || usesGlossMap || usesEmissionMap)
            {
                mat.SetFloat("_ForcePerPixel", 1.0f);
            }

            var texScaleOffset = mat.GetVector("_TextureScaleOffset");
            bool usesScale = texScaleOffset.x > 1.0f || texScaleOffset.y > 1.0f;
            bool usesOffset = texScaleOffset.z > 0.0f || texScaleOffset.w > 0.0f;

            ShaderGUIUtils.SetKeyword(mat, "_MainTex_SCALE_ON", usesScale);
            ShaderGUIUtils.SetKeyword(mat, "_MainTex_OFFSET_ON", usesOffset);
        }

        protected virtual void CacheMainProperties(MaterialProperty[] props)
        {
            this.blendMode = ShaderGUI.FindProperty("_Mode", props);

            this.vertexColorEnabled = ShaderGUI.FindProperty("_UseVertexColor", props);
            this.mainColorEnabled = ShaderGUI.FindProperty("_UseMainColor", props);
            this.mainColor = ShaderGUI.FindProperty("_Color", props);
            this.mainTexture = ShaderGUI.FindProperty("_MainTex", props);
            this.alphaCutoff = ShaderGUI.FindProperty("_Cutoff", props);

            this.occlusionMap = ShaderGUI.FindProperty("_OcclusionMap", props);

            this.ambientLightingEnabled = ShaderGUI.FindProperty("_UseAmbient", props);
            this.diffuseLightingEnabled = ShaderGUI.FindProperty("_UseDiffuse", props);
            this.useAdditionalLightingData = ShaderGUI.FindProperty("_Shade4", props);
            this.perPixelLighting = ShaderGUI.FindProperty("_ForcePerPixel", props);

            this.specularLightingEnabled = ShaderGUI.FindProperty("_SpecularHighlights", props);
            this.specularColor = ShaderGUI.FindProperty("_SpecColor", props);
            this.specular = ShaderGUI.FindProperty("_Specular", props);
            this.specularMap = ShaderGUI.FindProperty("_SpecularMap", props);

            this.gloss = ShaderGUI.FindProperty("_Gloss", props);
            this.glossMap = ShaderGUI.FindProperty("_GlossMap", props);

            this.normalMap = ShaderGUI.FindProperty("_BumpMap", props);

            this.reflectionsEnabled = ShaderGUI.FindProperty("_UseReflections", props);
            this.cubeMap = ShaderGUI.FindProperty("_CubeMap", props);
            this.reflectionScale = ShaderGUI.FindProperty("_ReflectionScale", props);
            this.calibrationSpaceReflections = ShaderGUI.FindProperty("_CalibrationSpaceReflections", props);

            this.rimLightingEnabled = ShaderGUI.FindProperty("_UseRimLighting", props);
            this.rimPower = ShaderGUI.FindProperty("_RimPower", props);
            this.rimColor = ShaderGUI.FindProperty("_RimColor", props);

            this.emissionColorEnabled = ShaderGUI.FindProperty("_UseEmissionColor", props);
            this.emissionColor = ShaderGUI.FindProperty("_EmissionColor", props);
            this.emissionMap = ShaderGUI.FindProperty("_EmissionMap", props);

            this.textureScaleAndOffset = ShaderGUI.FindProperty("_TextureScaleOffset", props);

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

            public static GUIContent occlusionMap = new GUIContent("Occlusion Map", "Additional texture to be overlayed on the main texture");

            public static GUIContent ambientLightingEnabled = new GUIContent("Ambient", "Scene ambient lighting");
            public static GUIContent diffuseLightingEnabled = new GUIContent("Diffuse", "Diffuse (lambertian) lighting from directional lights");
            public static GUIContent useAdditionalLighingData = new GUIContent("Point and Spot", "Apply lighting from point and spot lights (expensive)");
            public static GUIContent perPixelLighting = new GUIContent("Per-Pixel lighting", "Light objects per-pixel instead of per-vertex - using any lighting affecting map will force this on");

            public static GUIContent specularLightingEnabled = new GUIContent("Specular Highlights", "Specular (blinn-phong) lighting from directional lights");
            public static GUIContent specularColor = new GUIContent(" Color", "Tint to apply to specular highlights");
            public static GUIContent specular = new GUIContent("Power", "Specular Power - using a map will turn on per-pixel lighting");
            public static GUIContent gloss = new GUIContent("Gloss", "Specular Scale - using a map will turn on per-pixel lighting");

            public static GUIContent normalMap = new GUIContent("Normal Map", "Normal Map - will turn on per-pixel lighting");

            public static GUIContent reflectionsEnabled = new GUIContent("Reflections", "Cube map based reflections");
            public static GUIContent cubeMap = new GUIContent("Cube Map", "Cube map lookup for reflections");
            public static GUIContent reflectionScale = new GUIContent("Scale", "Reflection strength");
            public static GUIContent calibrationSpaceReflections = new GUIContent("In calibration space", "Keeps reflections consistent across different calibrations");

            public static GUIContent rimLightingEnabled = new GUIContent("Rim Lighting", "Side lighting");
            public static GUIContent rimPower = new GUIContent("Power", "Power of rim lighting");
            public static GUIContent rimColor = new GUIContent("Color", "Color of rim lighting");

            public static GUIContent emission = new GUIContent("Emission", "Emission (RGB)");

            public static GUIContent textureScaleAndOffset = new GUIContent("Texture Scale and Offset", "Applies to all textures");

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
            Transparent,
            Advanced
        }
    }
}