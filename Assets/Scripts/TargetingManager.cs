using UnityEngine;
using System.Collections;

public class TargetingManager : MonoBehaviour
{
    public static TargetingManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Multiple instances of singleton created", this);
        }

        Instance = this;
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

        foreach (var gameObject in gameObjects)
        {
            Vector3 diff = gameObject.transform.position - searchPosition;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = gameObject;
                distance = curDistance;
            }
        }
        return closest;
    }

    void Start()
    {
    }

    void Update()
    {
    }
}
