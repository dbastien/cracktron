using UnityEngine;

/// <summary>
/// Sets game camera transformation based on scene editor camera
/// </summary>
[ExecuteInEditMode]
public class GameCameraFromSceneCamera : MonoBehaviour
{
#if UNITY_EDITOR
    public void OnEnable()
    {
        UnityEditor.EditorApplication.update += GameCameraFromSceneCamera.UpdateCameras;
    }

    public void OnDisable()
    {
        UnityEditor.EditorApplication.update -= GameCameraFromSceneCamera.UpdateCameras;
    }

    public static void UpdateCameras()
    {
        if (UnityEditor.SceneView.sceneViews.Count == 0)
        {
            return;
        }

        var sceneView = (UnityEditor.SceneView)UnityEditor.SceneView.sceneViews[0];

        if (sceneView != null)
        {
            Camera.main.transform.rotation = sceneView.camera.transform.rotation;
            Camera.main.transform.position = sceneView.camera.transform.position;
        }
    }
#endif
}
