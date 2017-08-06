using UnityEditor;

[CustomEditor(typeof(Neuron), true)]
public class NeuronEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //we want to hide the "Script" field which just shows the type of object
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serializedObject.ApplyModifiedProperties();
    }
}