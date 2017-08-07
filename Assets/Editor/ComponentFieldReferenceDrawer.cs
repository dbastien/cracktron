using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

//TODO: rename to ComponentMemberReferenceDrawer, support properties
[CustomPropertyDrawer(typeof(ComponentFieldReference))]
public class ComponentFieldReferenceDrawer : PropertyDrawer
{
    public class ComponentFieldReferenceEntry
    {
        public Component targetComponent;
        public string targetFieldName;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = 0;

        var targetComponent = property.FindPropertyRelative("targetComponent");
        var targetFieldName = property.FindPropertyRelative("targetFieldName");

        position.height = 16f;
        EditorGUI.PropertyField(position, targetComponent, label);

        var component = targetComponent.objectReferenceValue as Component;

        if (component == null)
        {
            return;
        }

        position.y += 18f;
        GUI.changed = false;

        var entries = ComponentFieldReferenceDrawer.GetFields(component.gameObject);
        //var current = PropertyReference.ToString(target.objectReferenceValue as Component, field.stringValue);
        var current = targetFieldName.stringValue;

        var names = ComponentFieldReferenceDrawer.GetNames(entries, current, out index);

        // Draw a selection list
        GUI.changed = false;
        position.xMin += EditorGUIUtility.labelWidth;
        position.width -= 18f;
        var choice = EditorGUI.Popup(position, string.Empty, index, names);

        // Update the target object and property name
        if (GUI.changed && choice > 0)
        {
            var entry = entries[choice - 1];
            targetComponent.objectReferenceValue = entry.targetComponent;
            targetFieldName.stringValue = entry.targetFieldName;
        }
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        var target = prop.FindPropertyRelative("targetComponent");
        var comp = target.objectReferenceValue as Component;
        return (comp != null) ? 36f : 16f;
    }

    public static string[] GetNames(List<ComponentFieldReferenceEntry> list, string choice, out int index)
    {
        index = 0;
        var names = new string[list.Count + 1];

        for (var i = 0; i < list.Count; ++i)
        {
            var entry = list[i];
            var name = entry.targetFieldName;
            names[i+1] = name;

            if (index == 0 && string.Equals(name, choice))
            {
                index = i+1;
            }
        }

        names[0] = string.IsNullOrEmpty(choice) || index == 0 ? "Select Field" : choice;

        return names;
    }

    public static List<ComponentFieldReferenceEntry> GetFields(GameObject target)
    {
        var components = target.GetComponents<Component>();

        var entries = new List<ComponentFieldReferenceEntry>();

        for (var c = 0; c < components.Length; ++c)
        {
            var component = components[c];
            Type type = component.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public;

            var fields = type.GetFields(flags);

            for (var f = 0; f < fields.Length; ++f)
            {
                var field = fields[f];
                var entry = new ComponentFieldReferenceEntry
                {
                    targetComponent = component,
                    targetFieldName = field.Name
                };
                entries.Add(entry);
            }
        }

        return entries;
    }
}