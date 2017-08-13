using UnityEngine;

public class ColorMemberReferenceCurve : MonoBehaviour 
{
    [SerializeField] public ComponentMemberReference MemberReference;

    public Color Begin;
    public Color End;
    [NormalizedAnimationCurve] public AnimationCurve Curve;

    private float timeElapsed;
    private Renderer render;

    public void Reset()
    {
        this.Curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        this.Curve.preWrapMode = WrapMode.PingPong;
        this.Curve.postWrapMode = WrapMode.PingPong;

        this.Begin = Color.white;
        this.End = Color.cyan;
    }

    void Awake()
    {

    }

    void OnEnable()
    {
        this.render = this.GetComponent<Renderer>();
    }

    void Start()
    {

    }

    void OnDisable()
    {

    }

    void Update()
    {
        this.timeElapsed += Time.deltaTime;

        this.MemberReference.SetValue(this.Begin.LerpUnclamped(this.End, this.Curve.Evaluate(this.timeElapsed)));
    }
}
