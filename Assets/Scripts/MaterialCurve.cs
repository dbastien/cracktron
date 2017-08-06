using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialCurve : MonoBehaviour
{
    public Color Start;
    public Color End;
    [NormalizedAnimationCurve] public AnimationCurve Curve;

    private float timeElapsed;

    private Renderer render;

    public void Reset()
    {
        this.Curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        this.Curve.preWrapMode = WrapMode.PingPong;
        this.Curve.postWrapMode = WrapMode.PingPong;

        this.Start = Color.white;
        this.End = Color.cyan;
    }

    public void OnEnable()
    {
        this.render = GetComponent<Renderer>();
    }

    public void Update()
    {
        this.timeElapsed += Time.deltaTime;

        this.render.material.color = this.Start.LerpUnclamped(this.End, this.Curve.Evaluate(timeElapsed));
    }
}