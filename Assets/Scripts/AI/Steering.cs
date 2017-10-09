using UnityEngine;

public static class Steering
{
    public static Vector3 Seek(Transform source, Transform target)
    {
        var result = target.position - source.position;
        result.Normalize();
        return result;
    }

    // public static Vector3 Random()
    // {
    //     var result = Random.onUnitSphere;
    //     return result; 
    // }
}