using UnityEngine;

public class ComponentMemberReferenceCurve : MonoBehaviour 
{
    [SerializeField] public ComponentMemberReference MemberReference;

    [NormalizedAnimationCurve] public AnimationCurve Curve;

    //could be any of several types
    object CurveStart;
    object CurveEnd;

    [FloatIncremental(.1f)] public float LengthScale = 1.0f;   

    private float timeElapsed;   

    public void Reset()
    {
        this.Curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        this.Curve.preWrapMode = WrapMode.PingPong;
        this.Curve.postWrapMode = WrapMode.PingPong;
    }
    
    void Update() 
    {
        this.timeElapsed += Time.deltaTime / this.LengthScale;        
    }
}
