using UnityEngine;

public class FloatIncrementalAttribute : PropertyAttribute
{
    public float Incremeent;

    public FloatIncrementalAttribute(float increment)
    {
        this.Incremeent = increment;
    }
}