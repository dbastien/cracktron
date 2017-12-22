using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(CRTRenderer), PostProcessEvent.AfterStack, "Custom/CRT")]
public sealed class CRT : PostProcessEffectSettings
{
}
 
public sealed class CRTRenderer : PostProcessEffectRenderer<Blur>
{
    public override void Render(PostProcessRenderContext context)
    {
        var cmd = context.command;

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/CRT"));
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);    
    }
}