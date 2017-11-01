using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ComponentMemberReference))]
public class ComponentMemberReferenceDrawer : PropertyDrawer
{
    //todo: how to allow user to supply filter?
    public static readonly List<Type> TargetTypes = new List<Type>
    {
        typeof(float),
        typeof(Vector2), typeof(Vector3), typeof(Vector4),
        typeof(Color)
    };
    
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

        var entries = ComponentMemberReferenceUtils.GetFields(component.gameObject,
                                                              ComponentMemberReferenceDrawer.TargetTypes);

        var current = ComponentMemberReferenceUtils.GetFriendlyName((Component)targetComponent.objectReferenceValue,
                                                                    targetMemberName.stringValue);

        var names = ComponentMemberReferenceUtils.GetNames(entries, current, out index);

        // draw popup list
        GUI.changed = false;
        position.xMin += EditorGUIUtility.labelWidth;
        position.width -= 18f;
        var choice = EditorGUI.Popup(position, string.Empty, index, names);

        // update the target object and member name
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
}