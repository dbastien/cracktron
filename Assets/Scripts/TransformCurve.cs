using System.Reflection;
using UnityEngine;

public class TransformCurve : MonoBehaviour
{
    public AnimationCurve Curve;
    public Vector3 Start;
    public Vector3 End;

    [HideInInspector] public PropertyInfo CurveTarget;
    [HideInInspector] public string CurveTargetName;

    private float timeElapsed;

    void Reset()
    {
        Curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        Curve.preWrapMode = WrapMode.PingPong;
        Curve.postWrapMode = WrapMode.PingPong;

        CurveTargetName = "position";
        Start = transform.position;
        End = transform.position + Vector3.up;
    }

    void Awake()
    {
        CurveTarget = typeof(Transform).GetProperty(CurveTargetName);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (CurveTarget != null)
        {
            CurveTarget.SetValue(this.transform, Start.UnclampedLerp(End, Curve.Evaluate(timeElapsed)), null);
        }
    }
}
