using UnityEngine;

public static class Mathi
{
    public static int Pow(int n, int p)
    {
        Debug.Assert(p >= 1);

        int result = 1;

        for (var i = 0; i < p; ++i)
        {
            result *= n;
        }
        
        return result;
    }
}