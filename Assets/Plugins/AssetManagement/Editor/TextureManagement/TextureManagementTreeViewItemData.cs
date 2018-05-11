using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class TextureManagementTreeViewItemData
{
    public static readonly string[] props = new string [] { "width", "height"};

    public TextureImporter Importer;


    public float test;

    // public static List<ComponentFieldReferenceEntry> GetFields(GameObject target, List<Type> targetTypes)
    // {
    //     var components = target.GetComponents<Component>();
    //     var entries = new List<ComponentFieldReferenceEntry>();
    //     var flags = BindingFlags.Instance | BindingFlags.Public;

    //     foreach (var component in components)
    //     {
    //         Type type = component.GetType();

    //         var fields = type.GetFields(flags).Where(f => targetTypes.Contains(f.FieldType));
    //         var props = type.GetProperties(flags).Where(p => targetTypes.Contains(p.PropertyType));

    //         entries.AddRange(fields.Select(field => new ComponentFieldReferenceEntry
    //         {
    //             targetComponent = component,
    //             targetMemberName = field.Name
    //         }));

    //         entries.AddRange(props.Select(prop => new ComponentFieldReferenceEntry
    //         {
    //             targetComponent = component,
    //             targetMemberName = prop.Name
    //         }));
    //     }

    //     return entries;
    // }
}