using UnityEditor;

/// <summary>
/// Add duplicate (copy+paste as new) functionality to Unity's project view
/// </summary>
public static class DuplicateAsset
{
    //TODO: replace with true copy/paste
    [MenuItem("Assets/Duplicate")]
    public static void Duplicate()
    {
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
    }

    [MenuItem("Assets/Duplicate", true)]
    public static bool ValidateDuplicate()
    {
        return Selection.activeObject != null;
    }
}
