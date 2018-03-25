using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class AdvancedShaderGUI : ShaderGUI 
{
    protected bool firstTimeApply = true;
    protected Dictionary<string, MaterialProperty> propsByName;

    protected Dictionary<string, MaterialProperty> PropertiesToDictionary(MaterialProperty[] props)
    {
        if (props == null) { return null; }

        var dict = new Dictionary<string, MaterialProperty>(props.Length);
        foreach (var p in props)
        {
            var prop = ShaderGUI.FindProperty(p.name, props);
            if (prop != null) { dict.Add(p.name, prop); }
        }
        return dict;
    }

    public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] props)
    {
        CacheProperties(props);
        CheckAndHandleFirstTimeApply(matEditor);

        EditorGUIUtility.labelWidth = 0f;
        EditorGUI.BeginChangeCheck();
        {
            ShowMainGUI(matEditor);
        }
        if (EditorGUI.EndChangeCheck())
        {
            UpdateAllTargets();
        }
   }    

    protected virtual void CheckAndHandleFirstTimeApply(MaterialEditor matEditor) { }
    protected virtual void CacheProperties(MaterialProperty[] props) { }
    protected virtual void UpdateAllTargets() {}
    protected virtual void ShowMainGUI(MaterialEditor matEditor) {}

    protected virtual void SetMaterialBlendPreset(Material mat, BlendPreset blendMode)
    {
        switch (blendMode)
        {
            case BlendPreset.Opaque:
                mat.SetOverrideTag("RenderType", string.Empty);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.SetKeyword("_ALPHATEST_ON", false);
                mat.SetKeyword("_ALPHAPREMULTIPLY_ON", false);     
                mat.renderQueue = -1;
                break;
            case BlendPreset.Cutout:
                mat.SetOverrideTag("RenderType", "TransparentCutout");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.SetKeyword("_ALPHATEST_ON", true);
                mat.SetKeyword("_ALPHAPREMULTIPLY_ON", false);                   
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendPreset.Fade:
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.SetKeyword("_ALPHATEST_ON", false);
                mat.SetKeyword("_ALPHAPREMULTIPLY_ON", false);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;                    
            case BlendPreset.Transparent:
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

    public void HeaderSectionWithProps(string sectionTitle, MaterialEditor matEditor, params string[] props)
    {
        ShaderGUIUtils.HeaderSection(sectionTitle, ()=> { ShaderProps(matEditor, props); });
    }

    public void ShaderProp(MaterialEditor matEditor, string prop)
    { 
        matEditor.ShaderProperty(this.propsByName[prop], this.propsByName[prop].displayName);
    }

    public void ShaderProps(MaterialEditor matEditor, params string[] props)
    { 
        for (int i = 0; i < props.Length; ++i)
        {
            matEditor.ShaderProperty(this.propsByName[props[i]], this.propsByName[props[i]].displayName);
        }
    }

    public void ShaderProp(MaterialEditor matEditor, string prop, GUIContent guiContent)
    { 
        matEditor.ShaderProperty(this.propsByName[prop], guiContent);
    }    

    public void ShaderProp(MaterialEditor matEditor, string prop, string label)
    {
        matEditor.ShaderProperty(this.propsByName[prop], label); 
    }    
}
