using UnityEngine;

public static class GUIStyleExtensions
{
    public static Vector2 CalcSize(this GUIStyle style, params GUIContent[] contentItems)
    {
        var s = Vector2.zero;

        foreach (var contentItem in contentItems)
        {
            s += style.CalcSize(contentItem);
        }

        return s;
    }
}