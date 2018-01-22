using UnityEngine;

public static class GameObjectExtensions
{ 
    public static string GetFullPath(this GameObject go)
    {
        if (go.transform.parent == null)
        {
            return go.name;
        }

        return go.transform.parent.gameObject.GetFullPath() + "/" + go.name;
    }
}
