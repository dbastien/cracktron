using UnityEngine;

public static class GameObjectExtensions
{ 
    public static string GetFullPath(this GameObject go)
    {
        string path = "/" + go.name;
        while (go.transform.parent != null)
        {
            go = go.transform.parent.gameObject;
            path = "/" + go.name + path;
        }
        return path;
    }
}
