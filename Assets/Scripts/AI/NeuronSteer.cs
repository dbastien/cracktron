using System;
using UnityEngine;

public abstract class NeuronSteer : Neuron
{
    public float Weight = 1.0f;

    [NonSerialized]
    public Vector3 Direction = Vector3.forward;
}