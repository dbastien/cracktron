using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OrientationConstraint
{
    public static Vector3 GetFacingVector(Vector3 fromPos, Vector3 toPos)
    {
        return (toPos - fromPos).normalized;
    }

    public static Vector3 GetFacingVectorExclusive(Vector3 fromPos, Vector3 toPos, int indexToExclude)
    {
        var facing = GetFacingVector(fromPos, toPos);
        
        return VectorSetIndex(facing, indexToExclude, 0).normalized;
    }

    public static Vector3 GetFacingVectorAroundAxis(Vector3 fromPos, Vector3 toPos, Vector3 axis)
    {
        var facing = GetFacingVector(fromPos, toPos);

        var up = axis;
        var right = Vector3.Cross(up, facing);
        var forward = Vector3.Cross(right, up);

        return forward.normalized;
    }

    public static Vector3 VectorSetIndex(Vector3 vec, int index, float val)
    {
        switch (index)
        {
            case 0: return new Vector3(val, vec.y, vec.z);
            case 1: return new Vector3(vec.x, val, vec.z);
            case 2: return new Vector3(vec.x, vec.y, val);
        }

        throw new Exception("index out of range");
    }
}
