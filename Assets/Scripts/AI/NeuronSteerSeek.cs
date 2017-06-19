using System;
using UnityEngine;

public class NeuronSteerSeek : NeuronSteer
{
    public NeuronTargetSingle Targeting;

    public override void Update()
    {
        Targeting.Update();

        if (Targeting.Target != null)
        {
            Direction = Targeting.Target.transform.position - owner.transform.position;
            Direction.Normalize();
        }
    }
}