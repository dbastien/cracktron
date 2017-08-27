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
    public class FastConfigurableParticlesGUI : ShaderGUI
    {
        protected bool firstTimeApply = true;

        //cached material properties
        protected MaterialProperty blendMode;

        protected MaterialProperty mainColor;
        protected MaterialProperty mainTexture;

        protected MaterialProperty useTextureScale;
        protected MaterialProperty useTextureOffset;
        protected MaterialProperty textureScaleAndOffset;

        protected MaterialProperty alphaTestEnabled;
        protected MaterialProperty alphaCutoff;

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
                this.SetMaterialBlendMode(matTarget, (ParticleBlendMode)this.blendMode.floatValue);
                this.SetMaterialAutoPropertiesAndKeywords(matTarget);
                this.firstTimeApply = false;
            }

            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();
            {
                this.ShowMainGUI(matEditor);
                this.ShowOutputConfigurationGUI(matEditor);

                if ((ParticleBlendMode)this.blendMode.floatValue == ParticleBlendMode.Advanced)
                {
                    matEditor.RenderQueueField();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in this.blendMode.targets)
                {
                    var mat = obj as Material;
                    this.SetMaterialBlendMode(mat, (ParticleBlendMode)this.blendMode.floatValue);
                    this.SetMaterialAutoPropertiesAndKeywords(mat);
                }
            }
        }

        protected virtual void ShowMainGUI(MaterialEditor matEditor)
        {
            this.ShowBlendModeGUI(matEditor);

            var mode = (ParticleBlendMode)this.blendMode.floatValue;
            var mat = matEditor.target as Material;

            ShaderGUIUtils.BeginHeader("Base Texture and Color");
            {
                CustomMaterialEditor.TextureWithAutoScaleOffsetSingleLine(matEditor, Styles.main, this.mainTexture, this.textureScaleAndOffset);
            }
            ShaderGUIUtils.EndHeader();
            ShaderGUIUtils.HeaderSeparator();

            ShaderGUIUtils.BeginHeader("Global");
            {
                CustomMaterialEditor.TextureScaleOffsetVector4Property(matEditor, Styles.textureScaleAndOffset, this.textureScaleAndOffset);
            }
            ShaderGUIUtils.EndHeader();
            ShaderGUIUtils.HeaderSeparator();

            if (mode == ParticleBlendMode.Cutout || mode == ParticleBlendMode.Advanced)
            {
                ShaderGUIUtils.BeginHeader("Alpha Blending");
                {
                    if (mode == ParticleBlendMode.Advanced)
                    {
                        matEditor.ShaderProperty(this.alphaTestEnabled, Styles.alphaTestEnabled.text);                       
                    }
                    matEditor.ShaderProperty(this.alphaCutoff, Styles.alphaCutoff.text);

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
            base.AssignNewShaderToMaterial(mat, oldShader, newShader);

            if (oldShader == null)
            {
                return;
            }

            var blendMode = ParticleBlendMode.Opaque;

            bool legacy = oldShader.name.Contains("Legacy Shaders/");
            bool mobile = oldShader.name.Contains("Mobile/");

            bool transparent = oldShader.name.Contains("Transparent/");
            bool cutout = oldShader.name.Contains("Transparent/Cutout/");

            //FastConfigurable uses shared scale and offset
            //pull them from the main texture on the source material if available
            if (mat.HasProperty("_MainTex_ST"))
            {
                var scaleOffset = mat.GetVector("_MainTex_ST");
                mat.SetVector("_TextureScaleOffset", scaleOffset);
            }

           if (mobile || legacy)
            {
                if (cutout)
                {
                    blendMode = ParticleBlendMode.Cutout;
                }
                else if (transparent)
                {
                    blendMode = ParticleBlendMode.Transparent;
                }
            }

            this.SetMaterialBlendMode(mat, blendMode);
            this.SetMaterialAutoPropertiesAndKeywords(mat);
        }

        protected virtual void ShowBlendModeGUI(MaterialEditor matEditor)
        {
            EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
            var mode = (ParticleBlendMode)this.blendMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (ParticleBlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
            if (EditorGUI.EndChangeCheck())
            {
                matEditor.RegisterPropertyChangeUndo("Rendering Mode");
                this.blendMode.floatValue = (float)mode;
            }

            EditorGUI.showMixedValue = false;
        }

        protected virtual void ShowOutputConfigurationGUI(MaterialEditor matEditor)
        {
            var mode = (ParticleBlendMode)this.blendMode.floatValue;
            if (mode == ParticleBlendMode.Advanced)
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

        protected virtual void SetMaterialBlendMode(Material mat, ParticleBlendMode blendMode)
        {
            switch (blendMode)
            {
                case ParticleBlendMode.Opaque:
                    mat.SetOverrideTag("RenderType", string.Empty);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.SetKeyword("_ALPHATEST_ON", false);
                    mat.renderQueue = -1;
                    break;
                case ParticleBlendMode.Cutout:
                    mat.SetOverrideTag("RenderType", "TransparentCutout");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.SetKeyword("_ALPHATEST_ON", true);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case ParticleBlendMode.Transparent:
                    mat.SetOverrideTag("RenderType", "Transparent");
                    //non pre-multiplied alpha
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 1);
                    mat.SetKeyword("_ALPHATEST_ON", false);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case ParticleBlendMode.Advanced:
                    //user configured
                    break;
            }
        }

        protected virtual void SetMaterialAutoPropertiesAndKeywords(Material mat)
        {
            var texScaleOffset = mat.GetVector("_TextureScaleOffset");
            bool usesScale = (texScaleOffset.x != 1f) || (texScaleOffset.y != 1f);
            bool usesOffset = (texScaleOffset.z != 0f) || (texScaleOffset.w != 0f);

            mat.SetKeyword("_MainTex_SCALE_ON", usesScale);
            mat.SetKeyword("_MainTex_OFFSET_ON", usesOffset);
        }

        protected virtual void CacheMainProperties(MaterialProperty[] props)
        {
            this.blendMode = ShaderGUI.FindProperty("_Mode", props);

            this.mainColor = ShaderGUI.FindProperty("_Color", props);
            this.mainTexture = ShaderGUI.FindProperty("_MainTex", props);
            this.alphaCutoff = ShaderGUI.FindProperty("_Cutoff", props);

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
            public static readonly string[] blendNames = Enum.GetNames(typeof(ParticleBlendMode));

            public static string renderingMode = "Rendering Mode";

            public static GUIContent main = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");

            public static GUIContent textureScaleAndOffset = new GUIContent("Texture Scale and Offset", "Applies to all textures");

            //alpha test
            public static GUIContent alphaTestEnabled = new GUIContent("Alpha Test", "Enables rejection of pixels based on alpha and cutoff");
            public static GUIContent alphaCutoff = new GUIContent("Alpha Cutoff", "Pixels with alpha below this value will be rejected");

            public static GUIContent srcBlend = new GUIContent("Source Blend", "Blend factor for transparency, etc.");
            public static GUIContent dstBlend = new GUIContent("Destination Blend", "Blend factor for transparency, etc.");
            public static GUIContent blendOp = new GUIContent("Blend Operation", "Blend operation for transparency, etc.");

            public static GUIContent cullMode = new GUIContent("Culling Mode", "Type of culling to apply to polygons - typically this is set to backfacing");
            public static GUIContent zTest = new GUIContent("Z Test", "Depth buffer check type - output is not written if this is false");
            public static GUIContent zWrite = new GUIContent("Z Write", "When to write to the depth buffer");
            public static GUIContent colorWriteMask = new GUIContent("Color Write Mask", "Restricts output to specified color channels only");
        }

        public enum ParticleBlendMode
        {
            Opaque,
            Cutout,
            Transparent,
            Advanced
        }
    }
}