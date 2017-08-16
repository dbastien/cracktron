using UnityEngine;

public static class GUIStyleExtensions
{
    public static Vector2 CalcSize(this GUIStyle style, params GUIContent[] contentItems)
    {
        var s = Vector2.zero;

        for (int i = 0; i < contentItems.Length; ++i)
        {
            s += style.CalcSize(contentItems[i]);
        }

        return s;
    }
}