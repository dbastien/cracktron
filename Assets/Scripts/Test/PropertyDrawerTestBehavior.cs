using UnityEngine;

public class PropertyDrawerTestBehavior : MonoBehaviour 
{
    [OpenLocalFile] public string OpenLocalFileEditorTest;
    [IntIncremental(1)] public int IntIncrementalTest;
    [FloatIncremental(0.25f)] public float FloatIncrementalTest;
}
