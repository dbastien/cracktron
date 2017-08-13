using UnityEngine;

public class LocalFileEditorTestBehavior : MonoBehaviour 
{
    [OpenLocalFile] public string OpenLocalFileEditorTest;
    [FloatIncremental(0.25f)] public float FloatEasyButtonTest;
}
