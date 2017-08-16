﻿using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextMeshCurve : MonoBehaviour
{
    [NormalizedAnimationCurve] public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1.0f), new Keyframe(0.5f, 0), new Keyframe(0.75f, 1.0f), new Keyframe(1, 0f));

    [FloatIncremental(.1f)] public float CurveScale = 1.0f;
    [FloatIncremental(.1f)] public float AnimationSpeed = 1.0f;

    public bool RotateLetters;
    [Range(0.01f, 10.0f)] public float RotationScale = 1.0f;
    [Range(0.0f, 1.0f)] public float RotationSmoothing = 0.5f;

    private float timeElapsed;
    private TMP_Text textComponent;
    private TMP_MeshInfo[] initialMeshInfo;

    private Quaternion lastLetterRotation = Quaternion.identity;

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

            // Compute the baseline mid point for each character
            var offsetToMidBaseline = new Vector3((initialVertices[vertexIndex].x + initialVertices[vertexIndex + 2].x) * 0.5f,
                                                   textInfo.characterInfo[i].baseLine,
                                                   0.0f);

            // Compute the angle of rotation for each character based on the animation curve
            var x0 = (offsetToMidBaseline.x - boundsMinX) / (boundsMaxX - boundsMinX); // Character's position relative to the bounds of the mesh.
            var x1 = x0 + 0.0001f;
            var y0 = this.VertexCurve.Evaluate(x0 + (this.timeElapsed * this.AnimationSpeed)) * this.CurveScale;
            var y1 = this.VertexCurve.Evaluate(x1 + (this.timeElapsed * this.AnimationSpeed)) * this.CurveScale;

            Matrix4x4 matrix;
            if (this.RotateLetters)
            {
                var tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) - new Vector3(offsetToMidBaseline.x, y0);

                var dot = Mathf.Acos(Vector3.Dot(Vector3.right, tangent.normalized)) * Mathf.Rad2Deg * this.RotationScale;
                var cross = Vector3.Cross(Vector3.right, tangent);
                var angle = cross.z > 0 ? dot : 360 - dot;

                var curLetterRotation = Quaternion.Euler(0, 0, angle);
                var q = Quaternion.Slerp(this.lastLetterRotation, curLetterRotation, (Time.deltaTime * 100f * (1f - this.RotationSmoothing)));
                this.lastLetterRotation = q;

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
