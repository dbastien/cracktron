// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity
{
    using UnityEditor;

    /// <summary>
    /// Editor for FastConfigurable2Sided shader
    /// </summary>
    public class FastConfigurable2SidedGUI : FastConfigurableGUI
    {
        protected override void ShowOutputConfigurationGUI(MaterialEditor matEditor)
        {
            ShaderGUIUtils.BeginHeader("Output Configuration");
            {
                matEditor.ShaderProperty(this.zTest, Styles.zTest);
                matEditor.ShaderProperty(this.zWrite, Styles.zWrite);
                matEditor.ShaderProperty(this.colorWriteMask, Styles.colorWriteMask);
                matEditor.RenderQueueField();
            }
            ShaderGUIUtils.EndHeader();
        }

        protected override void CacheOutputConfigurationProperties(MaterialProperty[] props)
        {
            this.zTest = ShaderGUI.FindProperty("_ZTest", props);
            this.zWrite = ShaderGUI.FindProperty("_ZWrite", props);
            this.colorWriteMask = ShaderGUI.FindProperty("_ColorWriteMask", props);
        }
    }
}