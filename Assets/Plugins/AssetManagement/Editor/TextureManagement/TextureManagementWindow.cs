using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class TextureManagementWindow : EditorWindow
{
    // SerializeField is used to ensure the view state is written to the window 
    // layout file. This means that the state survives restarting Unity as long as the window
    // is not closed. If the attribute is omitted then the state is still serialized/deserialized.
    [SerializeField] TreeViewState TreeViewState;

    [SerializeField] MultiColumnHeaderState MultiColumnHeaderState;   

    //The TreeView is not serializable, so it should be reconstructed from the tree data.
    TextureManagementTreeView TreeView;

    [MenuItem("Assets/Management/Texture Management")]
    public static void ShowWindow()
    {
        var w = EditorWindow.GetWindow<TextureManagementWindow>();
        w.Show();
    }

    void OnGUI()
    {
        TreeView.OnGUI(new Rect(0, 0, position.width, position.height));
    }    

    void OnEnable()
    {
        // Check if there is a serialized view state (that survived assembly reloading)
        if (TreeViewState == null)
        {
            TreeViewState = new TreeViewState();
        }

        TreeView = new TextureManagementTreeView(TreeViewState);
    }
}
