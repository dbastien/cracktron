using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextMeshCurve: MonoBehaviour
{
    public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1.0f), new Keyframe(0.5f, 0), new Keyframe(0.75f, 1.0f), new Keyframe(1, 0f));
    public float CurveScale = 1.0f;

    public float AnimationSpeed = 1.0f;
    private float timeElapsed;

    public bool RotateLetters;
    [Range(0.01f, 10.0f)] public float RotationScale = 1.0f;

    private TMP_Text textComponent;
    private TMP_MeshInfo[] initialMeshInfo;

    void Update()
    {
        timeElapsed += Time.deltaTime;      
        Matrix4x4 matrix;

        if (textComponent == null)
        {
            textComponent = gameObject.GetComponent<TMP_Text>();

            textComponent.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.
            textComponent.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.
            initialMeshInfo = textComponent.textInfo.CopyMeshInfoVertexData();
        }

        textComponent.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.
        textComponent.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.

        var textInfo = textComponent.textInfo;
        int characterCount = textInfo.characterCount;

        float boundsMinX = textComponent.bounds.min.x;
        float boundsMaxX = textComponent.bounds.max.x;

        for (int i = 0; i < characterCount; ++i)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                continue;
            }

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Get the index of the mesh used by this character.
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            var targetVertices = textInfo.meshInfo[materialIndex].vertices;
            var initialVertices = initialMeshInfo[materialIndex].vertices;

            // Compute the baseline mid point for each character
            Vector3 offsetToMidBaseline = new Vector2((initialVertices[vertexIndex + 0].x + initialVertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);

            // Compute the angle of rotation for each character based on the animation curve
            float x0 = (offsetToMidBaseline.x - boundsMinX) / (boundsMaxX - boundsMinX); // Character's position relative to the bounds of the mesh.
            float x1 = x0 + 0.0001f;
            float y0 = VertexCurve.Evaluate(x0 + timeElapsed * AnimationSpeed) * CurveScale;
            float y1 = VertexCurve.Evaluate(x1 + timeElapsed * AnimationSpeed) * CurveScale;

            if (RotateLetters)
            {
                var tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) - new Vector3(offsetToMidBaseline.x, y0);

                var dot = Mathf.Acos(Vector3.Dot(Vector3.right, tangent.normalized)) * Mathf.Rad2Deg * RotationScale;
                var cross = Vector3.Cross(Vector3.right, tangent);
                var angle = cross.z > 0 ? dot : 360 - dot;

                matrix = Matrix4x4.Translate(offsetToMidBaseline) *
                         Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one) *
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
        textComponent.UpdateVertexData();
    }
}
