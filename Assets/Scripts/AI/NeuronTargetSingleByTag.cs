public class NeuronTargetSingleByTag : NeuronTargetSingle
{
    public string Tag;

    public override void Update()
    {
        this.Target = TargetingManager.Instance.FindByTag(this.Tag);
    }
}