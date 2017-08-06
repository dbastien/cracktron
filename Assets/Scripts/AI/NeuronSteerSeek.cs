public class NeuronSteerSeek : NeuronSteer
{
    public NeuronTargetSingle Targeting;

    public override void Update()
    {
        this.Targeting.Update();

        if (this.Targeting.Target != null)
        {
            this.Direction = Targeting.Target.transform.position - owner.transform.position;
            this.Direction.Normalize();
        }
    }
}