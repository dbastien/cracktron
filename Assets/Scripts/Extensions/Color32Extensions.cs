﻿using UnityEngine;

public static class Color32Extensions
{
    public static Color32 PremultiplyAlpha(this Color32 v)
    {
        Color floatCol = v;        
        return (Color32)floatCol.PremultiplyAlpha(); 
    }
}