using UnityEngine;

public class IntIncrementalAttribute : PropertyAttribute
{
    public int Increment;

    public IntIncrementalAttribute(int increment)
    {
        this.Increment = increment;
    }
}