using UnityEngine;

public abstract class Vector3Provider
{   
    public abstract Vector3 Next();
}

public class RandomUniform : Vector3Provider
{
    public override Vector3 Next() 
    {
        var r = Random.value;
        return new Vector3(r, r, r);
    }
}

public class RandomSphere : Vector3Provider
{
    public override Vector3 Next() { return Random.insideUnitSphere; }
}

public class RandomSphereSurface : Vector3Provider
{
    public override Vector3 Next() { return Random.onUnitSphere; }
}

public class RandomCube : Vector3Provider
{
    public override Vector3 Next() { return RandomExtensions.GetVector3(); }    
}