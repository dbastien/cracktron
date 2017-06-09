using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialCurve : MonoBehaviour
{
    public Color Start;
    public Color End;
    public AnimationCurve Curve;

    private float timeElapsed;

    private Renderer render;

    void Reset()
    {
        Curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        Curve.preWrapMode = WrapMode.PingPong;
        Curve.postWrapMode = WrapMode.PingPong;

        Start = Color.white;
        End = Color.cyan;
    }

    void OnEnable()
    {
        this.render = GetComponent<Renderer>();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        render.material.color = Start.LerpUnclamped(End, Curve.Evaluate(timeElapsed));
    }
}