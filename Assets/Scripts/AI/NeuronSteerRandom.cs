using UnityEngine;

public class NeuronSteerRandom : NeuronSteer
{
    public Vector3 DirectionWeight = new Vector3(1, 0, 1);
    public bool Smoothing = true;

    public float UpdateFrequencySeconds = 0.02f;

    private float updateFrequencyRemainderSeconds;
    private Vector3 targetDirection;

    public override void Update()
    {
        if (!this.owner)
        {
            return;
        }

        this.updateFrequencyRemainderSeconds += Time.deltaTime;

        if (this.updateFrequencyRemainderSeconds >= this.UpdateFrequencySeconds)
        {
            var randomPos = Random.onUnitSphere;
            randomPos.Scale(this.DirectionWeight);

            this.targetDirection = randomPos;
            this.targetDirection.Normalize();

            this.updateFrequencyRemainderSeconds -= this.UpdateFrequencySeconds;
        }

        if (this.Smoothing)
        {
            //TODO: forward is adjusting as we're doing this so it's not a linear ease really, and should slerp anyhow...
            this.Direction = Vector3.Lerp(this.owner.transform.forward, this.targetDirection, this.updateFrequencyRemainderSeconds / this.UpdateFrequencySeconds);
        }
        else
        {
            this.Direction = this.targetDirection;
        }

        this.Direction.Normalize();
    }
}