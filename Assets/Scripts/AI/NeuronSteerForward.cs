public class NeuronSteerForward : NeuronSteer
{
    public override void Update()
    {
        if (this.owner)
        {
            this.Direction = this.owner.transform.forward;
            this.Direction.Normalize();
        }
    }
}