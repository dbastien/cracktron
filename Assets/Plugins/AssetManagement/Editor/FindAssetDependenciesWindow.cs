using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindAssetDependenciesWindow : EditorWindow
{
    [MenuItem("Assets/Management/Find Dependencies")]
    public static void ShowWindow()
    {
        var w = EditorWindow.GetWindow<FindAssetDependenciesWindow>();
        w.Show();
    }

    Object target;

    public void OnGUI()
    {
        target = EditorGUILayout.ObjectField("Find dependencies of:", target, typeof(GameObject), true);

        if (GUILayout.Button("Search"))
        {
            FindAllReferences();
        }
    }

    public void FindAllReferences()
    {
        var roots = new Object[] { target };

        var dependencies = EditorUtility.CollectDependencies(roots);

        foreach (var o in dependencies)
        {
            Debug.Log(o.name);
        }
    }
}
