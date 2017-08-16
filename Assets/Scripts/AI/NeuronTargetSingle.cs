using System;
using UnityEngine;

public abstract class NeuronTargetSingle : Neuron
{
    [NonSerialized]
    public GameObject Target;
}