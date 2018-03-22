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
