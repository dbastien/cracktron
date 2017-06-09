using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorCurve : MonoBehaviour
{
    public Color Start;
    public Color End;
    public AnimationCurve Curve;

    private float timeElapsed;

    private Renderer render;

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