using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseUtils
{
    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        var GUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)), null);
        var assets = new List<T>(GUIDs.Length);

        for (var i = 0; i < GUIDs.Length; ++i)
        {
            Debug.Log(GUIDs[i]);

            var assetPath = AssetDatabase.GUIDToAssetPath(GUIDs[i]);

            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            //TODO: verify cases where this can be null - it'd be nice to allocate an array instead of list
            if (asset != null)
            {
                assets.Add(asset);
            }
        }

        return assets;
    }
}
