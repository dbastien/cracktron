﻿using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextMeshCurve : MonoBehaviour
{
    [NormalizedAnimationCurve] public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1.0f), new Keyframe(0.5f, 0), new Keyframe(0.75f, 1.0f), new Keyframe(1, 0f));

    [FloatIncremental(.1f)] public float CurveScale = 1.0f;
    [FloatIncremental(.1f)] public float AnimationSpeed = 1.0f;

    public bool RotateLetters;
    [NormalizedAnimationCurve] public AnimationCurve RotationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1.0f), new Keyframe(0.5f, 0), new Keyframe(0.75f, 1.0f), new Keyframe(1, 0f));   
    [Range(0.001f, .25f)] public float RotationScale = 0.05f;

    private float timeElapsed;

    private TMP_Text textComponent;
    private TMP_MeshInfo[] initialMeshInfo;

    public void Update()
    {
        if (!this.textComponent)
        {
            this.textComponent = this.gameObject.GetComponent<TMP_Text>();
            this.textComponent.havePropertiesChanged = true;
            this.textComponent.ForceMeshUpdate();
            this.initialMeshInfo = this.textComponent.textInfo.CopyMeshInfoVertexData();
        }

        this.timeElapsed += Time.deltaTime;

        this.textComponent.havePropertiesChanged = true;
        this.textComponent.ForceMeshUpdate();

        var textInfo = this.textComponent.textInfo;
        var characterCount = textInfo.characterCount;

        var boundsMinX = this.textComponent.bounds.min.x;
        var boundsMaxX = this.textComponent.bounds.max.x;
        float boundsDelta = (boundsMaxX - boundsMinX);

        float animScaledTime = this.timeElapsed * this.AnimationSpeed;

        for (var i = 0; i < characterCount; ++i)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                continue;
            }

            var vertexIndex = textInfo.characterInfo[i].vertexIndex;
            var materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            var targetVertices = textInfo.meshInfo[materialIndex].vertices;
            var initialVertices = this.initialMeshInfo[materialIndex].vertices;

            // character baseline mid point
            var offsetToMidBaseline = new Vector3((initialVertices[vertexIndex].x + initialVertices[vertexIndex + 2].x) * .5f,
                                                   textInfo.characterInfo[i].baseLine,
                                                   0f);

            // character position along curve
            var x0 = (offsetToMidBaseline.x - boundsMinX) / boundsDelta; // Character's position relative to the bounds of the mesh.
            var y0 = this.VertexCurve.Evaluate(x0 + animScaledTime) * this.CurveScale;

            Matrix4x4 matrix;
            if (this.RotateLetters)
            {
                // animScaledTime so synced with the translation motion
                var angle = (-.5f + this.RotationCurve.Evaluate(x0 + animScaledTime)) * this.RotationScale * 360f;

                var q = Quaternion.Euler(0, 0, angle);

                matrix = Matrix4x4.Translate(offsetToMidBaseline) *
                         Matrix4x4.TRS(new Vector3(0, y0, 0), q, Vector3.one) *
                         Matrix4x4.Translate(-offsetToMidBaseline);
            }
            else
            {
                matrix = Matrix4x4.Translate(new Vector3(0, y0, 0));
            }

            targetVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(initialVertices[vertexIndex + 0]);
            targetVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(initialVertices[vertexIndex + 1]);
            targetVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(initialVertices[vertexIndex + 2]);
            targetVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(initialVertices[vertexIndex + 3]);
        }

        this.textComponent.UpdateVertexData();
    }
}
