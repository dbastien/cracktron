using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BrainData : ScriptableObject
{
    public List<NeuronSteer> SteerNeurons = new List<NeuronSteer>();

    public void CloneNeurons()
    {
        for (int i = 0; i < this.SteerNeurons.Count; ++i)
        {
            this.SteerNeurons[i] = Instantiate(this.SteerNeurons[i]);
        }
    }
}