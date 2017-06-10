using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
public class ReorderableListDrawer : PropertyDrawer
{
    private ReorderableList list_;

    public void OnEnable()
    {
    }

    public override void OnGUI(Rect rect, SerializedProperty serializedProperty, GUIContent label)
    {
        SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list_");
        ReorderableList list = GetList(listProperty);

        float height = 0f;
        for (var i = 0; i < listProperty.arraySize; i++)
        {
            height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
        }
        list.elementHeight = height;
        list.DoList(rect);
    }

    public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label)
    {
        SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list_");
        return GetList(listProperty).GetHeight();
    }

    private ReorderableList GetList(SerializedProperty serializedProperty)
    {
        if (list_ == null)
        {
            list_ = new ReorderableList(serializedProperty.serializedObject, serializedProperty);
        }

        return list_;
    }
}
