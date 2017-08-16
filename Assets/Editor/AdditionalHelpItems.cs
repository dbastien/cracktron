using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds useful links to the help menu
/// </summary>
public static class AdditionalHelpItems
{
    [MenuItem("Help/Doc Bookmarks/Execution Order")]
    public static void MenuBookmarkExecutionOrder()
    {
        Application.OpenURL("https://docs.unity3d.com/Manual/ExecutionOrder.html");
    }

    [MenuItem("Help/Doc Bookmarks/Platform Dependent Compilation")]
    public static void MenuBookmarkPlatformDependentCompilation()
    {
        Application.OpenURL("https://docs.unity3d.com/Manual/PlatformDependentCompilation.html");
    }   

    [MenuItem("Help/Doc Bookmarks/Special Folder Names")]
    public static void MenuBookmarkSpecialFolderNames()
    {
        Application.OpenURL("https://docs.unity3d.com/Manual/SpecialFolders.html");
    }

    [MenuItem("Help/Doc Bookmarks/Shaders/Built-In Shader Variables")]
    public static void MenuBookmarkShadersBuiltInShaderVariables()
    {
        Application.OpenURL("https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html");
    }

    [MenuItem("Help/Doc Bookmarks/Shaders/Predefined Shader Preprocessor Macros")]
    public static void MenuBookmarkShadersPredefinedShaderPreprocessorMacros()
    {
        Application.OpenURL("https://docs.unity3d.com/Manual/SL-BuiltinMacros.html");
    }   
}