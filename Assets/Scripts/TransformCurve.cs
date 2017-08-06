﻿using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TransformCurve : MonoBehaviour
{
    [NormalizedAnimationCurve] public AnimationCurve Curve;

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

        if (DefaultEndOffsets.TryGetValue(this.CurveTargetName, out this.End))
        {
            this.End += Start;
        }
        else
        {
            this.End = Start + Vector3.up;
        }
    }

    public void Awake()
    {
        UpdateTarget();
    }

    public void Update()
    {
        this.timeElapsed += Time.deltaTime / LengthScale;

        if (this.CurveTarget != null)
        {
            this.CurveTarget.SetValue(this.transform, Start.LerpUnclamped(this.End, this.Curve.Evaluate(this.timeElapsed)), null);
        }
    }
}
