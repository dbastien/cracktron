﻿using UnityEngine;

public static class Mathi
{
    public static int Pow(int n, int p)
    {
        int result = 1;

        for (var i = 0; i < p; ++i)
        {
            result *= n;
        }
        
        return result;
    }
}