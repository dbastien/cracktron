using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public static class PropertySheetExtensions
{
    public static void SetKeyword(this PropertySheet sheet, string keyword, bool state)
    {
        if (state)
        {
            sheet.EnableKeyword(keyword);
        }
        else
        {
            sheet.DisableKeyword(keyword);
        }
    }
}

[Serializable]
public sealed class Vector3IntParameter : ParameterOverride<Vector3Int>
{
    public override void Interp(Vector3Int from, Vector3Int to, float t)
    {
        // Int snapping interpolation. Don't use this for enums as they don't necessarily have
        // contiguous values. Use the default interpolator instead (same as bool).
        value = Vector3Int.RoundToInt((Vector3)from + (Vector3)(to - from) * t);
    }
}