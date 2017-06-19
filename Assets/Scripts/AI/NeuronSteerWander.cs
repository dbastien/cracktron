using System;
using UnityEngine;

public class NeuronSteerWander : NeuronSteer
{
    public Vector3 DirectionWeight = new Vector3(1, 0, 1);
    public float Jitter = 20.0f;
    public bool Smoothing = true;

    public float UpdateFrequencySeconds = 0.02f;

    private float updateFrequencyRemainderSeconds;
    private Vector3 targetDirection;

    public override void Update()
    {
        if (null == owner)
            return;

        updateFrequencyRemainderSeconds += Time.deltaTime;

        if (updateFrequencyRemainderSeconds >= UpdateFrequencySeconds)
        {
            var randomPos = UnityEngine.Random.onUnitSphere;
            randomPos.Scale(DirectionWeight);
            
            targetDirection = owner.transform.forward + randomPos * Jitter * UpdateFrequencySeconds;
            targetDirection.Normalize();

            updateFrequencyRemainderSeconds -= UpdateFrequencySeconds;
        }

        if (Smoothing)
        {
            //TODO: forward is adjusting as we're doing this so it's not a linear ease really, and should slerp anyhow...
            Direction = Vector3.Lerp(owner.transform.forward, targetDirection, updateFrequencyRemainderSeconds / UpdateFrequencySeconds);
        }
        else
        {
            Direction = targetDirection;
        }

        Direction.Normalize();
    }
}