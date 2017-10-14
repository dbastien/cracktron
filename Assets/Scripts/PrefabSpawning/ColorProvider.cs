using UnityEngine;

public abstract class ColorProvider
{   
    public abstract Color Next();
}

public class RandomColor : ColorProvider
{
    public override Color Next() { return RandomExtensions.GetVector4(); }
}