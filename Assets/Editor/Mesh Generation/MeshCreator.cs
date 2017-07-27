using UnityEditor;
using UnityEngine;

public class TestQuad : MonoBehaviour
{
    [MenuItem("GameObject/3D Object/TestQuad")]
    private static void CreateTest()
    {
        var go = new GameObject();
        go.name = "TestQuad";

        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();

        var mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            new Vector3(-1,0,-1),
            new Vector3( 1,0,-1),
            new Vector3(-1,0, 1),
            new Vector3( 1,0, 1),
        };

        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        mesh.name = go.name;

        mesh.RecalculateNormals();

        meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");      

        meshFilter.mesh = mesh;
    }
}
