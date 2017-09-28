using System;
using UnityEngine;

public static class TextureCropper
{
    public static Rect CreateCropRectPixels(int sourceWidth, int sourceHeight, float targetAspect)
    {
        if (sourceWidth <= 0 || sourceHeight <= 0 || targetAspect <= 0.0)
        {
            throw new ArgumentOutOfRangeException("Invalid args!");
        }

        float sourceAspect = sourceWidth / (float)sourceHeight;
        float sourceToTargetAspect = sourceAspect / targetAspect;

        bool sourceWider = sourceToTargetAspect >= 1.0f;

        float width = sourceWider ? sourceWidth / sourceToTargetAspect : sourceWidth;
        float height = sourceWider ? sourceHeight : sourceHeight * sourceToTargetAspect;

        float x = sourceWider ? (sourceWidth - width) * 0.5f : 0;
        float y = sourceWider ? 0 : (sourceHeight - height) * 0.5f;

        return new Rect(x, y, width, height);
    }

    public static Rect CreateCropRectNormalized(int sourceWidth, int sourceHeight, float targetAspect)
    {
        var targetRect = TextureCropper.CreateCropRectPixels(sourceWidth, sourceHeight, targetAspect);

        targetRect.x /= sourceWidth;
        targetRect.y /= sourceHeight;
        targetRect.width /= sourceWidth;
        targetRect.height /= sourceHeight;

        return targetRect;
    }
}
