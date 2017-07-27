using UnityEngine;
using UnityEditor;

public class PaletteWindow : EditorWindow
{
    [OpenLocalFile] public string inputFile;

    [SaveLocalFile] public string outputFile;

    [MenuItem("Cracktron/Palette Window")]
    static void ShowWindow()
    {
        var window = GetWindow<PaletteWindow>();
        window.name = "Palette";
        
        window.Show();
    }

    void OnGUI()
    {
        //Editor.CreateEditor(inputFile);
    }
}
