using System;
using System.Reflection;
using UnityEngine;

public static class TextureUtilWrapper
{
    private static readonly Type type = Type.GetType("UnityEditor.TextureUtil, UnityEditor");

    private static readonly MethodInfo GetStorageMemorySizeMethod = TextureUtilWrapper.type.GetMethod("GetStorageMemorySize");
    private static readonly MethodInfo GetRuntimeMemorySizeMethod = TextureUtilWrapper.type.GetMethod("GetRuntimeMemorySize");
    private static readonly MethodInfo GetTextureFormatStringMethod = TextureUtilWrapper.type.GetMethod("GetTextureFormatString");

    public static int GetStorageMemorySize(Texture t)
    {
        return (int)TextureUtilWrapper.GetStorageMemorySizeMethod.Invoke(null, new object[] { t });
    }

    public static int GetRuntimeMemorySize(Texture t)
    {
        return (int)TextureUtilWrapper.GetRuntimeMemorySizeMethod.Invoke(null, new object[] { t });
    }

    public static string GetTextureFormatString(TextureFormat tf)
    {
        return (string)TextureUtilWrapper.GetTextureFormatStringMethod.Invoke(null, new object[] { tf });
    }



//                 string info = string.Format("{0}x{1}x{2} {3} {4}",
//                     tex.width, tex.height, tex.depth,
//                     TextureUtil.GetTextureFormatString(tex.format),
// EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySizeLong(tex)));
}
