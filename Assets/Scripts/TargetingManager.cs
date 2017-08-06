using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    public static TargetingManager Instance { get; private set; }

    public void Awake()
    {
        if (TargetingManager.Instance != null)
        {
            Debug.Log("Multiple instances of singleton created", this);
        }

        TargetingManager.Instance = this;
    }

    public GameObject FindByTag(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag);
    }

    public GameObject FindByTagAndDistance(string tag, Vector3 searchPosition)
    {
        var gameObjects = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;

        foreach (var go in gameObjects)
        {
            Vector3 diff = go.transform.position - searchPosition;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}
