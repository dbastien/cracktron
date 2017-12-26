using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(RetroRenderer), PostProcessEvent.AfterStack, "Custom/Retro")]
public sealed class Retro : PostProcessEffectSettings
{
    [Tooltip("Number of horizontal pixels")]
    public IntParameter GameResolutionX = new IntParameter() { value = 320 };
    [Tooltip("Number of vertical pixels")]
    public IntParameter GameResolutionY = new IntParameter() { value = 180 };

    [Tooltip("Number of horizontal pixels")]
    public IntParameter CRTResolutionX = new IntParameter() { value = 640 };
    [Tooltip("Number of vertical pixels")]
    public IntParameter CRTResolutionY = new IntParameter() { value = 360 };
    [Range(0.0f, 2.0f)]
    public FloatParameter CRTBarrelDistortion = new FloatParameter { value = 0.3f };

    [Tooltip("Pixel mask for CRT - i.e. aperture grill or shadow mask")]
    public TextureParameter CRTMask = new TextureParameter();
    [Tooltip("How many pixels (not sub-pixels) does the mask represent?")]
    public Vector2Parameter CRTMaskSizePixels = new Vector2Parameter { value = new Vector2(2.0f, 1.0f) };
    public ColorParameter CRTMaskWeight = new ColorParameter() { value = Color.white };

    public BoolParameter ColorQuantization = new BoolParameter() { value = false  };
    [Range(1, 256)]
    public Int3Parameter ColorQuantizationBuckets = new Int3Parameter() { value = new Int3(5,5,5) };

    [Range(-1.0f, 1.0f)]
    public FloatParameter Brightness = new FloatParameter { value = 1.0f };
    [Range(0.0f, 2.0f)]
    public FloatParameter Contrast = new FloatParameter { value = 1.0f };
    [Range(0.0f, 4.0f)]
    public FloatParameter Saturation = new FloatParameter { value = 1.0f };  
}
 
public sealed class RetroRenderer : PostProcessEffectRenderer<Retro>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Retro"));
        var cmd = context.command;

        int tw = settings.GameResolutionX.value;
        int th = settings.GameResolutionY.value;
        sheet.properties.SetVector("_GameResolution", new Vector4(tw, th, 0.0f, 0.0f));

        sheet.SetKeyword("_USECOLORQUANT_ON", settings.ColorQuantization.value);
        var b = settings.ColorQuantizationBuckets.value;
        sheet.properties.SetVector("_ColorQuantizationBuckets", new Vector4(b.x, b.y, b.z, 0.0f));

        tw = settings.CRTResolutionX.value;
        th = settings.CRTResolutionY.value;
        sheet.properties.SetVector("_CRTResolution", new Vector4(tw, th, 0.0f, 0.0f));
        sheet.properties.SetFloat("_CRTBarrelDistortion", settings.CRTBarrelDistortion.value);

        bool hasMask = settings.CRTMask.value != null;
        sheet.SetKeyword("_USECRTMASK_ON", hasMask);
        if (hasMask)
        {
            sheet.properties.SetTexture("_CRTMask", settings.CRTMask.value);
            sheet.properties.SetVector("_CRTMaskSizePixels", settings.CRTMaskSizePixels.value);
            sheet.properties.SetColor("_CRTMaskWeight", settings.CRTMaskWeight.value);
        }

        sheet.properties.SetFloat("_Brightness", settings.Brightness);
        sheet.properties.SetFloat("_Contrast", settings.Contrast);
        sheet.properties.SetFloat("_Saturation", settings.Saturation);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);    
    }
}