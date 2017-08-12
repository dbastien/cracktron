using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

[CustomPropertyDrawer(typeof(ComponentMemberReference))]
public class ComponentMemberReferenceDrawer : PropertyDrawer
{
    public static readonly List<Type> TargetTypes = new List<Type>
    {
        typeof(float),
        typeof(Vector2), typeof(Vector3), typeof(Vector4),
        typeof(Color)
    };

    public class ComponentFieldReferenceEntry
    {
        public Component targetComponent;
        public string targetMemberName;
    }

    private static string GetFriendlyName(object component, string member)
    {
        return component.GetType().Name + " " + member;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int index = 0;

        var targetComponent = property.FindPropertyRelative("targetComponent");
        var targetMemberName = property.FindPropertyRelative("targetMemberName");

        position.height = 16f;
        EditorGUI.PropertyField(position, targetComponent, label);

        var component = targetComponent.objectReferenceValue as Component;

        if (component == null)
        {
            return;
        }

        position.y += 18f;
        GUI.changed = false;

        var entries = ComponentMemberReferenceDrawer.GetFields(component.gameObject);
        var current = ComponentMemberReferenceDrawer.GetFriendlyName((Component)targetComponent.objectReferenceValue, targetMemberName.stringValue);

        var names = ComponentMemberReferenceDrawer.GetNames(entries, current, out index);

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
            targetMemberName.stringValue = entry.targetMemberName;
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
            var friendlyName = ComponentMemberReferenceDrawer.GetFriendlyName(entry.targetComponent, entry.targetMemberName);
            names[i + 1] = friendlyName;

            if (index == 0 && string.Equals(friendlyName, choice))
            {
                index = i+1;
            }
        }

        names[0] = string.IsNullOrEmpty(choice) || index == 0 ? "No Member" : choice;

        return names;
    }

    public static List<ComponentFieldReferenceEntry> GetFields(GameObject target)
    {
        var components = target.GetComponents<Component>();
        var entries = new List<ComponentFieldReferenceEntry>();
        var flags = BindingFlags.Instance | BindingFlags.Public;

        foreach (var component in components)
        {
            Type type = component.GetType();

            var fields = type.GetFields(flags).Where(f => ComponentMemberReferenceDrawer.TargetTypes.Contains(f.FieldType));
            var props = type.GetProperties(flags).Where(p => ComponentMemberReferenceDrawer.TargetTypes.Contains(p.PropertyType));

            entries.AddRange(fields.Select(field => new ComponentFieldReferenceEntry
            {
                targetComponent = component,
                targetMemberName = field.Name
            }));

            entries.AddRange(props.Select(prop => new ComponentFieldReferenceEntry
            {
                targetComponent = component,
                targetMemberName = prop.Name
            }));
        }

        return entries;
    }
}