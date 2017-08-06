using UnityEditor;
using UnityEngine;

public class TestQuad : MonoBehaviour
{
    [MenuItem("GameObject/3D Object/TestQuad")]
    private static void CreateTest()
    {
        var go = new GameObject()
        {
            name = "TestQuad"
        };
        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();

        var mesh = new Mesh()
        {
            vertices = new Vector3[]
            {
                new Vector3(-1, 0, -1),
                new Vector3( 1, 0, -1),
                new Vector3(-1, 0,  1),
                new Vector3( 1, 0,  1),
            },

            uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            },

            triangles = new int[] { 0, 2, 1, 2, 3, 1 },
            name = go.name
        };
        mesh.RecalculateNormals();

        meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");      

        meshFilter.mesh = mesh;
    }
}
