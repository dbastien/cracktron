using System;
using UnityEngine;

public class NeuronSteerForward : NeuronSteer
{
    public override void Update()
    {
        if (owner)
        {
            Direction = owner.transform.forward;
            Direction.Normalize();
        }
    }
}