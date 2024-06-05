using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombinationUtil<T>
{
    public static T[,] Combinations(T[] items, int choose)
    {
        T[,] combinations = new T[items.Length, choose];
        for (int i = 0; i < items.Length; i++)
        {
            combinations[i, 0] = items[i];
            for (int j = 0; j < items.Length; j++)
            {
                for (int k = j; k < j + choose; k++)
                {
                    combinations[i, k] = items[k];
                }
            }
        }

        return combinations;
    }

    public static T[,] Permutations(T[] items)
    {
        return null;
    }
}