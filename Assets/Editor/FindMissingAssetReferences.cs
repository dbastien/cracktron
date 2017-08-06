using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindMissingAssetReferences
{
    [MenuItem("Assets/Find Missing References")]
    public static void FindMissingReferencesMenu()
    {
        var gameObjects = AssetDatabaseUtils.FindAndLoadAssets<GameObject>();
        Debug.LogFormat("Searching <b>{0}</b> AssetDatabase GameObjects for missing references", gameObjects.Count);
        int missingCount = gameObjects.Sum(go => FindMissingAssetReferences.FindMissingReferences(go));

        var sceneGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        Debug.LogFormat("Searching <b>{0}</b> Scene GameObjects for missing references", sceneGameObjects.Length);
        missingCount += sceneGameObjects.Sum(go => FindMissingAssetReferences.FindMissingReferences(go));

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
                if ((sp.propertyType == SerializedPropertyType.ObjectReference) && (sp.objectReferenceInstanceIDValue != 0) && (sp.objectReferenceValue == null))
                {
                    Debug.LogFormat(go, "Missing asset for: <b>{0}</b>, Component: {1}, Property: {2}", go.GetFullPath(), ObjectNames.GetClassName(component), ObjectNames.NicifyVariableName(sp.name));
                    ++missingCount;
                }
            }
        }

        return missingCount;
    }
}
