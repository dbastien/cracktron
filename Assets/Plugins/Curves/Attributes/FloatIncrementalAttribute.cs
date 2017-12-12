using UnityEngine;

public class FloatIncrementalAttribute : PropertyAttribute
{
    public float Increment;

    public FloatIncrementalAttribute(float increment)
    {
        this.Increment = increment;
    }
}