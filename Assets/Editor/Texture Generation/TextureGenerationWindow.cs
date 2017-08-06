using UnityEditor;
using UnityEngine;

public class TextureGenerationWindow : EditorWindow
{
    public int ExportWidth = 512;
    public int ExportHeight = 512;

    public Texture2D Texture;

    private static readonly GUILayoutOption[] GUILayoutOptionEmptyArray = new GUILayoutOption[0];

    [MenuItem("Window/Texture Generation")]
    public static void Init()
    {
        //get existing or spin up new window
        var window = EditorWindow.GetWindow<TextureGenerationWindow>();
        window.minSize = new Vector2(100, 100);
        window.titleContent.text = "Texture Generation";
        window.Texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        window.FillTexture();

        window.Show();
    }

    public void OnGUI()
    {
        GUILayout.Label(" ", EditorStyles.boldLabel);

        var previewLayout = new GUILayoutOption[] { GUILayout.MaxWidth(100f), GUILayout.MaxHeight(100f) };

        EditorGUILayout.BeginHorizontal();
        this.ExportWidth = EditorGUILayout.IntField(Styles.resolutionX, this.ExportWidth, TextureGenerationWindow.GUILayoutOptionEmptyArray);
        this.ExportHeight = EditorGUILayout.IntField(Styles.resolutionY, this.ExportHeight, TextureGenerationWindow.GUILayoutOptionEmptyArray);
        EditorGUILayout.EndHorizontal();

        //https://docs.unity3d.com/ScriptReference/EditorGUI.DrawPreviewTexture.html
        EditorGUILayout.BeginHorizontal();
        Rect previewRect;
        previewRect = GUILayoutUtility.GetRect(100f, 100f, previewLayout);
        EditorGUI.DrawPreviewTexture(previewRect, this.Texture);

        previewRect = GUILayoutUtility.GetRect(100f, 100f, previewLayout);
        EditorGUI.DrawTextureTransparent(previewRect, this.Texture);

        previewRect = GUILayoutUtility.GetRect(100f, 100f, previewLayout);
        EditorGUI.DrawTextureAlpha(previewRect, this.Texture);
        EditorGUILayout.EndHorizontal();        

        if (GUILayout.Button("Export", TextureGenerationWindow.GUILayoutOptionEmptyArray))
        {
        }
    }

    private void FillTexture()
    {
        for (var y = 0; y < this.Texture.height; ++y)
        {
            for (var x = 0; x < this.Texture.width; ++x)
            {
                float xNormalized = x / (float)this.Texture.width;
                float yNormalized = y / (float)this.Texture.height;

                float a = 1.0f;
                float r = Mathf.PerlinNoise(xNormalized, yNormalized);
                float g = Mathf.PerlinNoise(xNormalized, yNormalized);
                float b = Mathf.PerlinNoise(xNormalized, yNormalized);
                this.Texture.SetPixel(x, y, new Color(r, g, b, a));
            }
        }

        this.Texture.Apply();
    }

    protected static class Styles
    {
        public static GUIContent resolution = new GUIContent("Resolution", "Size in pixels");
        public static GUIContent resolutionX = new GUIContent("Width", "Size in pixels");
        public static GUIContent resolutionY = new GUIContent("Height", "Size in pixels");
    }
}
