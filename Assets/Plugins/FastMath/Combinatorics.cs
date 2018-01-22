using System.Collections.Generic;

public static class Combinatorics
{
    public static List<List<T>> PermutationWithDuplication<T>(List<T> set, int k)
    {       
        var n = set.Count; 
        var resultCount = Mathi.Pow(n, k);

        var resultSet = new List<List<T>>(resultCount);
        var indices = new List<int>(k);
        for (int i = 0; i < k; ++i)
        {
            indices.Add(0);
        }
        
        for (var r = 0; r < resultCount; ++r)
        {
            var result = new List<T>(k);
            for (var i = k-1; i >= 1; --i)
            {
                if (indices[i] >= n)
                {
                    indices[i] = 0;
                    ++indices[i-1];
                }

                result.Insert(0, set[indices[i]]);
            }

            result.Insert(0, set[indices[0]]);
            ++indices[k-1];
            resultSet.Add(result);
        }
 
        return resultSet;
    }
}