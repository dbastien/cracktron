﻿using System.Collections.Generic;
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


    public Vector3 CurveStart;
    public Vector3 CurveEnd;

    public bool RelativeMode;
    public Vector3 CurveOffset;


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

        var currentPos = (Vector3)typeof(Transform).GetProperty(this.CurveTargetName).GetValue(this.transform, null);       

        if (!RelativeMode)
        {
            this.CurveStart = currentPos;        
            if (TransformCurve.DefaultEndOffsets.TryGetValue(this.CurveTargetName, out this.CurveEnd))
            {
                this.CurveEnd += this.CurveStart;
            }
            else
            {
                this.CurveEnd = this.CurveStart + Vector3.up;
            }
        }
        else
        {
            this.CurveStart = Vector3.zero;
            this.CurveEnd = Vector3.zero;
        }
    }

    public void Awake()
    {
        this.UpdateTarget();

        if (RelativeMode)
        {
            var currentPos = (Vector3)typeof(Transform).GetProperty(this.CurveTargetName).GetValue(this.transform, null);
            this.CurveStart = currentPos;
            this.CurveEnd = currentPos + this.CurveOffset;
        }
    }

    public void Update()
    {
        this.timeElapsed += Time.deltaTime / this.LengthScale;

        if (this.CurveTarget != null)
        {
            var interpolatedValue = this.CurveStart.LerpUnclamped(this.CurveEnd, this.Curve.Evaluate(this.timeElapsed));
            this.CurveTarget.SetValue(this.transform, interpolatedValue, null);
        }
    }
}
