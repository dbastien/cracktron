using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindMissingAssetReferencesWindow : EditorWindow
{
    [MenuItem("Assets/Management/Find Missing References")]
    public static void ShowWindow()
    {
        var w = EditorWindow.GetWindow<FindMissingAssetReferencesWindow>();
        w.Show();
    }

    bool checkAssetDatabase = false;
    bool checkScene = true;

    public void OnGUI()
    {
        checkAssetDatabase = GUILayout.Toggle(checkAssetDatabase, "Search Asset Database");
        checkScene = GUILayout.Toggle(checkScene, "Search Scene");

        if (GUILayout.Button("Search"))
        {
            FindAllMissingReferences();
        }
    }

    public void FindAllMissingReferences()
    {
        int missingCount = 0;

        if (this.checkAssetDatabase)
        {
            var gameObjects = AssetDatabaseUtils.FindAndLoadAssets<GameObject>();
            Debug.LogFormat("Searching <b>{0}</b> AssetDatabase GameObjects for missing references", gameObjects.Count);
            missingCount += gameObjects.Sum(go => FindMissingAssetReferencesWindow.FindMissingReferences(go));
        }

        if (this.checkScene)
        {
            var sceneGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            Debug.LogFormat("Searching <b>{0}</b> Scene GameObjects for missing references", sceneGameObjects.Length);
            missingCount += sceneGameObjects.Sum(go => FindMissingAssetReferencesWindow.FindMissingReferences(go));
        }

        var logString = string.Format("<b>Completed search, {0} missing references</b>", missingCount);

        if (missingCount > 0)
        {
            Debug.LogError(logString);
        }
        else
        {
            Debug.Log(logString);
        }
    }

    private static int FindMissingReferences(GameObject go)
    {
        int missingCount = 0;
        var components = go.GetComponents<Component>();
        foreach (var component in components)
        {
            if (!component)
            {
                Debug.LogError("Missing component for: <b>" + go.GetFullPath() + "</b>");
                ++missingCount;
                continue;
            }

            var so = new SerializedObject(component);

            var sp = so.GetIterator();
            while (sp.NextVisible(true))
            {
                if ((sp.propertyType == SerializedPropertyType.ObjectReference) && 
                    (sp.objectReferenceInstanceIDValue != 0) && 
                    (sp.objectReferenceValue == null))
                {
                    Debug.LogFormat(go, "Missing asset for: <b>{0}</b>, Component: {1}, Property: {2}",
                                    go.GetFullPath(), ObjectNames.GetClassName(component), ObjectNames.NicifyVariableName(sp.name));
                    ++missingCount;
                }
            }
        }

        return missingCount;
    }
}
