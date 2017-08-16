public class NeuronSteerSeek : NeuronSteer
{
    public NeuronTargetSingle Targeting;

    public override void Update()
    {
        this.Targeting.Update();

        if (this.Targeting.Target != null)
        {
            this.Direction = this.Targeting.Target.transform.position - this.owner.transform.position;
            this.Direction.Normalize();
        }
    }
}