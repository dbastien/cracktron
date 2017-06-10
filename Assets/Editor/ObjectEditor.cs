using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
public class ObjectEditor : Editor
{
    protected class ReorderableListState
    {
        public ReorderableList reorderableList;
        public SerializedProperty serializedProperty;

        public void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            var property = serializedProperty.GetArrayElementAtIndex(index);
            var rectOffset = new RectOffset(0, 0, -1, -3);
            rect = rectOffset.Add(rect);
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property, GUIContent.none, false);
        }

        public void DrawHeader(Rect rect)
        {
            GUI.Label(rect, serializedProperty.propertyPath);
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
        serializedObject.Update();
        var property = serializedObject.GetIterator();
        var next = property.NextVisible(true);
        if (next)
        {
            do
            {
                this.HandleProperty(property);
            } while (property.NextVisible(false));
        }
        serializedObject.ApplyModifiedProperties();
    }

    protected void HandleProperty(SerializedProperty property)
    {
        if (property.isArray && property.propertyType != SerializedPropertyType.String)
        {
            this.ArrayField(property);
        }
        else
        {
            bool isdefaultScriptProperty = property.name.Equals("m_Script") &&
                                           property.type.Equals("PPtr<MonoScript>") &&
                                           property.propertyType == SerializedPropertyType.ObjectReference &&
                                           property.propertyPath.Equals("m_Script");

            bool cachedGUIEnabled = GUI.enabled;
            if (isdefaultScriptProperty)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.PropertyField(property, property.isExpanded);

            if (isdefaultScriptProperty)
            {
                GUI.enabled = cachedGUIEnabled;
            }
        }
    }

    private ReorderableListState GetReorderableListState(SerializedProperty property)
    {
        ReorderableListState reorderableList = null;
        if (this.reorderableListStates.TryGetValue(property.name, out reorderableList))
        {
            reorderableList.serializedProperty = property;
            reorderableList.reorderableList.serializedProperty = property;
            return reorderableList;
        }
        reorderableList = new ReorderableListState();
        reorderableList.serializedProperty = property;
        reorderableList.reorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true);
        reorderableList.reorderableList.drawElementCallback += reorderableList.DrawElement;
        reorderableList.reorderableList.drawHeaderCallback += reorderableList.DrawHeader;
        this.reorderableListStates.Add(property.propertyPath, reorderableList);
        return reorderableList;
    }

    protected void ArrayField(SerializedProperty property)
    {
        var reorderableList = GetReorderableListState(property).reorderableList;
        reorderableList.DoLayoutList();
    }
}
