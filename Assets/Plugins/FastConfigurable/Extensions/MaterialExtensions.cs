using UnityEngine;
using UnityEditor;

public static class MaterialExtensions
{
    public static bool TryGetToggle(this Material mat, string property, bool defaultVal)
    {
        if (mat.HasProperty(property))
        {
            return mat.GetFloat(property) == 1f;
        }
        return defaultVal;
    }

    public static void SetKeyword(this Material mat, string keyword, bool state)
    {
        if (state)
        {
            mat.EnableKeyword(keyword);
        }
        else
        {
            mat.DisableKeyword(keyword);
        }
    }
}