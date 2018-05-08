using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(ColorBlindnessRenderer), PostProcessEvent.AfterStack, "Custom/ColorBlindness")]
public sealed class ColorBlindness : PostProcessEffectSettings
{
}
 
public sealed class ColorBlindnessRenderer : PostProcessEffectRenderer<ColorBlindness>
{
    public override void Render(PostProcessRenderContext context)
    {
        var cmd = context.command;

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ColorBlindness"));
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}