using System;
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
public sealed class Int3Parameter : ParameterOverride<Int3>
{
    public override void Interp(Int3 from, Int3 to, float t)
    {
        // Int snapping interpolation. Don't use this for enums as they don't necessarily have
        // contiguous values. Use the default interpolator instead (same as bool).
        value = (Int3)(from + (to - from) * t);
    }
}