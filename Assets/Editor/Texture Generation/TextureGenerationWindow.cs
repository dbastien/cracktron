using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureGenerationWindow : EditorWindow
{
    private static readonly GUILayoutOption[] GUILayoutOptionEmptyArray = new GUILayoutOption[0];

    [MenuItem("Window/Texture Generation")]
    static void Init()
    {
        //get existing or spin up new window
        var window = GetWindow<TextureGenerationWindow>();
        window.Show();

        Int3 i1 = Int3.one;
        Int3 i2 = Int3.zero;

        if (i1 == i2)
        {
        }
    }

    void OnGUI()
    {
        GUILayout.Label(" ", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField(Styles.resolution, 0, GUILayoutOptionEmptyArray);
        EditorGUILayout.EndHorizontal();
    }

    protected static class Styles
    {
        public static GUIContent resolution = new GUIContent("Resolution", "Size in pixels");

    }
}
