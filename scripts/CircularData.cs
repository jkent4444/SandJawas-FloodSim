using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CircularData {
    private ushort[][] data;
    private int nextIndex;
    private int maxEntries;
    private ushort[] average;
    private bool filled;
    public CircularData(int numArrays, int numDataEntries)
    {
        data = new ushort[numArrays][];
        average = new ushort[numDataEntries];
        for (int i = 0; i < numArrays; i++)
        {
            data[i] = new ushort[numDataEntries];
        }
        nextIndex = 0;
        this.maxEntries = numArrays;
    }

    //returns average of the data
    public ushort[] averageData {
     get {
        if (!filled)
        {
            return data[0];
        }
        return average;
    }}

    public void addData(ushort[] dataEntries)
    {
        
       

        if (filled)
        {
            int sum; 
            for (int u = 0; u < data[0].Length; u++)
            {
                sum = 0;
                for (int i = 0; i < maxEntries; i++)
                {
                 
                    sum += data[i][u];
                }
                
                average[u] = (ushort)(sum / maxEntries);  
                //if (Mathf.Abs(average[u] - dataEntries[u]) > 100)
                //{
               //     //Debug.Log(average[u]);
                    //filled = false;
                //}
                //else
                //{
                    data[nextIndex][u] = dataEntries[u];
                //}
            }

        }
        else
        {
            Buffer.BlockCopy(dataEntries, 0, data[nextIndex], 0, dataEntries.Length * sizeof(ushort));
        }
        nextIndex = (nextIndex + 1) % maxEntries;
        if (nextIndex == 0)
        {
            filled = true;
        }
    }
}
