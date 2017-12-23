using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(RetroRenderer), PostProcessEvent.AfterStack, "Custom/Retro")]
public sealed class Retro : PostProcessEffectSettings
{
    [Tooltip("Number of horizontal pixels")]
    public IntParameter ResolutionX = new IntParameter() { value = 320 };
    [Tooltip("Number of vertical pixels")]
    public IntParameter ResolutionY = new IntParameter() { value = 180 };

    public BoolParameter ColorQuantization = new BoolParameter() { value = true };
    [Range(1,256)]
    public Int3Parameter ColorQuantizationBuckets = new Int3Parameter() { value = new Int3(5,5,5) };

    public FloatParameter Brightness = new FloatParameter { value = 1.0f };
    public FloatParameter Contrast = new FloatParameter { value = 1.0f };
}
 
public sealed class RetroRenderer : PostProcessEffectRenderer<Retro>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Retro"));
        var cmd = context.command;

        sheet.SetKeyword("_USECOLORQUANT_ON", settings.ColorQuantization.value);
        var b = settings.ColorQuantizationBuckets.value;
        sheet.properties.SetVector("_ColorQuantizationBuckets", new Vector4(b.x, b.y, b.z, 0.0f));

        int tw = settings.ResolutionX.value;
        int th = settings.ResolutionY.value;
        sheet.properties.SetVector("_Resolution", new Vector4(tw, th, 0.0f, 0.0f));

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);    
    }
}