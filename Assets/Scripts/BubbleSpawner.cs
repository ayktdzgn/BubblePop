using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BubbleSpawner 
{
    //Powers of Two
    public int[] listOfPowers = new int[11];

    public int GetRandomSpawnedValue()
    {
        List<int> listOfValues = new List<int>();
        for (int i = 0; i < listOfPowers.Length; i++)
        {
            for (int j = 0; j < listOfPowers[i]; j++)
            {
                int value = Mathf.RoundToInt(Mathf.Pow(2,i+1));
                listOfValues.Add(value);
            }
        }

        return listOfValues[UnityEngine.Random.Range(0,listOfValues.Count)];
    }
}
