public class NeuronSteerForward : NeuronSteer
{
    public override void Update()
    {
        if (owner)
        {
            this.Direction = owner.transform.forward;
            this.Direction.Normalize();
        }
    }
}