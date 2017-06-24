using UnityEditor;
using UnityEngine;

public class TextureGenerationWindow : EditorWindow
{
    public int ExportWidth = 512;
    public int ExportHeight = 512;

    public Texture2D Texture;

    private static readonly GUILayoutOption[] GUILayoutOptionEmptyArray = new GUILayoutOption[0];

    [MenuItem("Window/Texture Generation")]
    static void Init()
    {
        //get existing or spin up new window
        var window = GetWindow<TextureGenerationWindow>();
        window.minSize = new Vector2(100, 100);
        window.titleContent.text = "Texture Generation";
        window.Texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        window.FillTexture();

        window.Show();
    }

    void FillTexture()
    {
        for (var y = 0; y < Texture.height; ++y)
        {
            for (var x = 0; x < Texture.width; ++x)
            {
                float xNormalized = x / (float)Texture.width;
                float yNormalized = y / (float)Texture.height;

                float a = 1.0f;
                float r = Mathf.PerlinNoise(xNormalized, yNormalized);
                float g = Mathf.PerlinNoise(xNormalized, yNormalized);
                float b = Mathf.PerlinNoise(xNormalized, yNormalized);
                Texture.SetPixel(x, y, new Color(r, g, b, a));
            }
        }

        Texture.Apply();
    }

    void OnGUI()
    {
        GUILayout.Label(" ", EditorStyles.boldLabel);

        var previewLayout = new GUILayoutOption[] { GUILayout.MaxWidth(100f), GUILayout.MaxHeight(100f) };

        EditorGUILayout.BeginHorizontal();
        ExportWidth = EditorGUILayout.IntField(Styles.resolutionX, ExportWidth, GUILayoutOptionEmptyArray);
        ExportHeight = EditorGUILayout.IntField(Styles.resolutionY, ExportHeight, GUILayoutOptionEmptyArray);
        EditorGUILayout.EndHorizontal();

        //https://docs.unity3d.com/ScriptReference/EditorGUI.DrawPreviewTexture.html
        EditorGUILayout.BeginHorizontal();
        Rect previewRect;
        previewRect = GUILayoutUtility.GetRect(100f, 100f, previewLayout);
        EditorGUI.DrawPreviewTexture(previewRect, Texture);

        previewRect = GUILayoutUtility.GetRect(100f, 100f, previewLayout);
        EditorGUI.DrawTextureTransparent(previewRect, Texture);

        previewRect = GUILayoutUtility.GetRect(100f, 100f, previewLayout);
        EditorGUI.DrawTextureAlpha(previewRect, Texture);
        EditorGUILayout.EndHorizontal();        

        if (GUILayout.Button("Export", GUILayoutOptionEmptyArray))
        {

        }
    }

    protected static class Styles
    {
        public static GUIContent resolution = new GUIContent("Resolution", "Size in pixels");
        public static GUIContent resolutionX = new GUIContent("Width", "Size in pixels");
        public static GUIContent resolutionY = new GUIContent("Height", "Size in pixels");
    }
}
