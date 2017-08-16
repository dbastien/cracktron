using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TransformCurve : MonoBehaviour
{
    public static readonly Dictionary<string, Vector3> DefaultEndOffsets = new Dictionary<string, Vector3>()
    {
        { "eulerAngles", new Vector3(0.0f, 360.0f, 0.0f) },
        { "localEulerAngles", new Vector3(0.0f, 360.0f, 0.0f) },
    };

    [NormalizedAnimationCurve] public AnimationCurve Curve;

    public Vector3 Start;
    public Vector3 End;

    [FloatIncremental(.1f)] public float LengthScale = 1.0f;

    public PropertyInfo CurveTarget;
    public string CurveTargetName;

    private float timeElapsed;

    public void Reset()
    {
        this.Curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        this.Curve.preWrapMode = WrapMode.PingPong;
        this.Curve.postWrapMode = WrapMode.PingPong;

        this.CurveTargetName = "position";

        this.ResetStartEnd();
    }

    public void UpdateTarget()
    {
        this.CurveTarget = typeof(Transform).GetProperty(this.CurveTargetName);
    }

    public void ResetStartEnd()
    {
        this.UpdateTarget();
        this.Start = (Vector3)typeof(Transform).GetProperty(this.CurveTargetName).GetValue(this.transform, null);

        if (TransformCurve.DefaultEndOffsets.TryGetValue(this.CurveTargetName, out this.End))
        {
            this.End += this.Start;
        }
        else
        {
            this.End = this.Start + Vector3.up;
        }
    }

    public void Awake()
    {
        this.UpdateTarget();
    }

    public void Update()
    {
        this.timeElapsed += Time.deltaTime / this.LengthScale;

        if (this.CurveTarget != null)
        {
            this.CurveTarget.SetValue(this.transform, this.Start.LerpUnclamped(this.End, this.Curve.Evaluate(this.timeElapsed)), null);
        }
    }
}
