using UnityEditor;

public class PaletteWindow : EditorWindow
{
    [OpenLocalFile] public string inputFile;

    [SaveLocalFile] public string outputFile;

    [MenuItem("Cracktron/Palette Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<PaletteWindow>();
        window.name = "Palette";
        
        window.Show();
    }

    public void OnGUI()
    {
        //Editor.CreateEditor(inputFile);
    }
}
