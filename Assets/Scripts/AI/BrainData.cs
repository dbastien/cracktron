﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BrainData : ScriptableObject
{
    public List<NeuronSteer> SteerNeurons = new List<NeuronSteer>();

    public void CloneNeurons()
    {
        for (int i = 0; i < SteerNeurons.Count; ++i)
        {
            SteerNeurons[i] = Object.Instantiate(SteerNeurons[i]);
        }
    }
}