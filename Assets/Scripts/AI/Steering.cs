using UnityEngine;

public static class Steering
{
    public static Vector3 Seek(Transform source, Transform target)
    {
        var result = target.position - source.position;
        result.Normalize();
        return result;
    }

    public static Vector3 Pursue
    (
        Transform source, Transform target, float timeToLookAhead
    )
    {
 //       var targetPos = target.position + targetVelocity * timeToLookAhead;
         return Vector3.one; 
    }

    public static Vector3 Random()
    {
//        Vector3 result = Random.onUnitSphere;
        return Vector3.one; 
    }
}