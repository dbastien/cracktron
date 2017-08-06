using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// CustomEditor for Object in order to display reorderable list editor for all lists
/// </summary>
[CustomEditor(typeof(Object), true, isFallback = true)]
public class ObjectEditor : Editor
{
    protected class ReorderableListState
    {
        public ReorderableList reorderableList;
        public SerializedProperty serializedProperty;

        public void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            var property = this.serializedProperty.GetArrayElementAtIndex(index);
            var rectOffset = new RectOffset(0, 0, -1, -3);
            rect = rectOffset.Add(rect);
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property, GUIContent.none, false);
        }

        public void DrawHeader(Rect rect)
        {
            GUI.Label(rect, this.serializedProperty.propertyPath);
        }
    }

    private Dictionary<string, ReorderableListState> reorderableListStates;

    protected virtual void OnEnable()
    {
        this.reorderableListStates = new Dictionary<string, ReorderableListState>();
    }

    ~ObjectEditor()
    {
        this.reorderableListStates.Clear();
        this.reorderableListStates = null;
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        var property = this.serializedObject.GetIterator();
        var next = property.NextVisible(true);
        if (next)
        {
            do
            {
                this.PropertyField(property);
            } while (property.NextVisible(false));
        }
        this.serializedObject.ApplyModifiedProperties();
    }

    protected void PropertyField(SerializedProperty property)
    {
        if (property.isArray && property.propertyType != SerializedPropertyType.String)
        {
            this.ArrayField(property);
        }
        else
        {
            bool isScriptProperty = ReflectionUtils.IsScriptProperty(property);

            bool cachedGUIEnabled = GUI.enabled;
            if (isScriptProperty)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.PropertyField(property, property.isExpanded);

            if (isScriptProperty)
            {
                GUI.enabled = cachedGUIEnabled;
            }
        }
    }

    private ReorderableListState GetReorderableListState(SerializedProperty property)
    {
        ReorderableListState reorderableListState = null;

        if (this.reorderableListStates.TryGetValue(property.name, out reorderableListState))
        {
            reorderableListState.serializedProperty = property;
            reorderableListState.reorderableList.serializedProperty = property;
            return reorderableListState;
        }

        reorderableListState = new ReorderableListState()
        {
            serializedProperty = property,
            reorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true)
        };
        reorderableListState.reorderableList.drawElementCallback += reorderableListState.DrawElement;
        reorderableListState.reorderableList.drawHeaderCallback += reorderableListState.DrawHeader;
        this.reorderableListStates.Add(property.propertyPath, reorderableListState);

        return reorderableListState;
    }

    protected void ArrayField(SerializedProperty property)
    {
        var reorderableList = this.GetReorderableListState(property).reorderableList;
        reorderableList.DoLayoutList();
    }
}
