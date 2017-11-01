using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public static class ComponentMemberReferenceUtils
{
    public static string GetFriendlyName(object component, string member)
    {
        return component.GetType().Name + " " + member;
    }

    public static string[] GetNames(List<ComponentFieldReferenceEntry> list, string choice, out int index)
    {
        index = 0;
        var names = new string[list.Count + 1];

        for (var i = 0; i < list.Count; ++i)
        {
            var entry = list[i];
            var friendlyName = ComponentMemberReferenceUtils.GetFriendlyName(entry.targetComponent,
                                                                             entry.targetMemberName);
            names[i + 1] = friendlyName;

            if (index == 0 && string.Equals(friendlyName, choice))
            {
                index = i+1;
            }
        }

        names[0] = string.IsNullOrEmpty(choice) || index == 0 ? "No Member" : choice;

        return names;
    }

    public static List<ComponentFieldReferenceEntry> GetFields(GameObject target, List<Type> targetTypes)
    {
        var components = target.GetComponents<Component>();
        var entries = new List<ComponentFieldReferenceEntry>();
        var flags = BindingFlags.Instance | BindingFlags.Public;

        foreach (var component in components)
        {
            Type type = component.GetType();

            var fields = type.GetFields(flags).Where(f => targetTypes.Contains(f.FieldType));
            var props = type.GetProperties(flags).Where(p => targetTypes.Contains(p.PropertyType));

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