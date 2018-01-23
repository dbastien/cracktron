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

    private Object target;
    private string results;

    public void OnGUI()
    {
        if (target == null && Selection.objects != null)
        {
            target = Selection.objects[0];
        }

        target = EditorGUILayout.ObjectField("Find dependencies of:", target, typeof(GameObject), true);

        if (GUILayout.Button("Search"))
        {
            results = string.Empty;

            var roots = new Object[] { target };
            var dependencies = EditorUtility.CollectDependencies(roots);

            foreach (var o in dependencies)
            {
                var go = o as GameObject;

                if (go)
                {
                    results += go.GetFullPath() + System.Environment.NewLine;
                }
                else
                {                    
                    results += o.name + System.Environment.NewLine;
                }
            }
        }

        EditorGUILayout.SelectableLabel(results, GUILayout.ExpandHeight(true));
    }
}
