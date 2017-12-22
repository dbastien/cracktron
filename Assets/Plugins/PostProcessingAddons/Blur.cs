using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(BlurRenderer), PostProcessEvent.AfterStack, "Custom/Blur")]
public sealed class Blur : PostProcessEffectSettings
{
}
 
public sealed class BlurRenderer : PostProcessEffectRenderer<Blur>
{
    public override void Render(PostProcessRenderContext context)
    {
        var cmd = context.command;

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Blur"));
        var tex = context.GetScreenSpaceTemporaryRT(0);        
        context.command.BlitFullscreenTriangle(context.source, tex, sheet, 0);    
        context.command.BlitFullscreenTriangle(tex, context.destination, sheet, 1);
    }
}