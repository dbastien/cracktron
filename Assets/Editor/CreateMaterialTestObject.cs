using UnityEditor;
using UnityEngine;

public static class CreateMaterialTestObject
{
    [MenuItem("Assets/Create/Material Test Object")]
    public static void CreateTestObject()
    {
        var selection = Selection.activeObject as Material;

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = selection.name;

        var meshRenderer = go.GetComponent<MeshRenderer>();

        meshRenderer.sharedMaterial = selection;

        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("Assets/Create/Material Test Object", true)]
    public static bool ValidateCreateTestObject()
    {
        return Selection.activeObject.GetType() == typeof(Material);
    }
}
