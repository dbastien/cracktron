using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TransformCurve : MonoBehaviour
{
    public AnimationCurve Curve;

    public Vector3 Start;
    public Vector3 End;

    //[Range(0.01f, 10.0f)]
    public float LengthScale = 1.0f;

    public PropertyInfo CurveTarget;
    public string CurveTargetName;

    private float timeElapsed;

    public static readonly Dictionary<string, Vector3> DefaultEndOffsets = new Dictionary<string, Vector3>()
    {
        { "eulerAngles", new Vector3(0.0f, 360.0f, 0.0f) },
        { "localEulerAngles", new Vector3(0.0f, 360.0f, 0.0f) },
    };

    void Reset()
    {
        Curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        Curve.preWrapMode = WrapMode.PingPong;
        Curve.postWrapMode = WrapMode.PingPong;

        CurveTargetName = "position";

        ResetStartEnd();
    }

    public void UpdateTarget()
    {
        CurveTarget = typeof(Transform).GetProperty(CurveTargetName);
    }

    public void ResetStartEnd()
    {
        this.
        UpdateTarget();
        Start = (Vector3)typeof(Transform).GetProperty(CurveTargetName).GetValue(this.transform, null);

        if (DefaultEndOffsets.TryGetValue(CurveTargetName, out End))
        {
            End += Start;
        }
        else
        {
            End = Start + Vector3.up;
        }
    }

    void Awake()
    {
        UpdateTarget();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime / LengthScale;

        if (CurveTarget != null)
        {
            CurveTarget.SetValue(this.transform, Start.LerpUnclamped(End, Curve.Evaluate(timeElapsed)), null);
        }
    }
}
