using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindReferencesInAssetsWindow : EditorWindow
{
    [MenuItem("Assets/Management/Find References")]
    public static void ShowWindow()
    {
        var w = EditorWindow.GetWindow<FindReferencesInAssetsWindow>();
        w.Show();
    }

    bool checkAssetDatabase = false;
    bool checkScene = true;
    Object target;

    public void OnGUI()
    {
        target = EditorGUILayout.ObjectField("Find references to:", target, typeof(Object), true);
        checkAssetDatabase = GUILayout.Toggle(checkAssetDatabase, "Search Asset Database");
        checkScene = GUILayout.Toggle(checkScene, "Search Scene");

        if (GUILayout.Button("Search"))
        {
            FindAllReferences();
        }
    }

    public void FindAllReferences()
    {
        var path = AssetDatabase.GetAssetOrScenePath(target);

        var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (asset == null)
        {
            Debug.LogError("Couldn't load asset!");
            return;
        }

        int countFound = 0;

        if (this.checkAssetDatabase)
        {
            var gameObjects = AssetDatabaseUtils.FindAndLoadAssets<GameObject>();
            Debug.LogFormat(asset, "Searching <b>{0}</b> AssetDatabase GameObjects for references to: <b>{1}</b>", gameObjects.Count, asset.name);
            countFound += gameObjects.Sum(go => FindReferencesInAssetsWindow.FindReferences(asset, go));
        }

        if (this.checkScene)
        {
            var sceneGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            Debug.LogFormat(asset, "Searching <b>{0}</b> Scene GameObjects for references to: <b>{1}</b>", sceneGameObjects.Length, asset.name);
            countFound += sceneGameObjects.Sum(go => FindReferencesInAssetsWindow.FindReferences(asset, go));
        }

        Debug.LogFormat(asset, "<b>Completed search, {0} references found</b>", countFound);
    }
    
    private static int FindReferences(Object asset, GameObject go)
    {
        int countFound = 0;
        if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
        {
            if (PrefabUtility.GetPrefabParent(go) == asset)
            {
                Debug.LogFormat("Reference at: <b>{0}</b>", go.GetFullPath());
                ++countFound;
            }
        }

        var components = go.GetComponents<Component>();
        foreach (var component in components)
        {
            if (!component)
            {
                continue;
            }

            var so = new SerializedObject(component);

            var sp = so.GetIterator();
            while (sp.NextVisible(true))
            {
                if ((sp.propertyType == SerializedPropertyType.ObjectReference) && (sp.objectReferenceValue == asset))
                {
                    Debug.LogFormat(go, "Reference at: <b>{0}</b>, Component {1}, Property {2}", go.GetFullPath(), ObjectNames.GetClassName(component), ObjectNames.NicifyVariableName(sp.name));
                    ++countFound;
                }
            }
        }

        return countFound;
    }
}
