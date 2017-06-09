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

    public void Awake()
    {
        CurveTarget = typeof(Transform).GetProperty(CurveTargetName);
    }

    public void Update()
    {
        timeElapsed += Time.deltaTime;

        if (CurveTarget != null)
        {
            CurveTarget.SetValue(this.transform, Start.UnclampedLerp(End, Curve.Evaluate(timeElapsed)), null);
        }
    }
}
