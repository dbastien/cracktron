using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Property drawers do not get called for the collection itself, only children
//  this draws all children when called for the first, but the standard unity 
//  collection header will be shown in addition
/// </summary>
[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
public class ReorderableListDrawer : PropertyDrawer
{
    private ReorderableList reorderableListCached;
    private string propertyPath;

    private void Init(SerializedProperty serializedProperty)
    {
        if (this.reorderableListCached != null)
        {
            return;
        }

        var serializedObject = serializedProperty.serializedObject;
        this.propertyPath = serializedProperty.propertyPath;

        //look for array component of property - this is the secret sauce to making it generic
        this.propertyPath = this.propertyPath.Substring(0, serializedProperty.propertyPath.LastIndexOf("Array") - 1);

        SerializedProperty elements = serializedObject.FindProperty(this.propertyPath);
        this.reorderableListCached = new ReorderableList(serializedProperty.serializedObject, elements, true, true, true, true)
        {
            drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawElement),
            drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawHeader)
        };
        this.reorderableListCached.elementHeight += 16f;
    }

    public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
    {
        if (!serializedProperty.propertyPath.Contains("0"))
        {
            return;
        }

        this.Init(serializedProperty);
        this.reorderableListCached.DoList(position);
    }

    private void DrawHeader(Rect rect)
    {
        GUI.Label(rect, this.propertyPath);
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty arrayElementAtIndex = this.reorderableListCached.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty property = arrayElementAtIndex;
        RectOffset rectOffset = new RectOffset(0, 0, -1, -3);
        rect = rectOffset.Add(rect);
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, property, false);
    }

    public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label)
    {
        //we'll get called for every element, so only size for the first one
        if (!serializedProperty.propertyPath.Contains("0"))
        {
            return 0f;
        }

        this.Init(serializedProperty);
        return this.reorderableListCached.GetHeight();
    }
}
