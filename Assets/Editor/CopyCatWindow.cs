using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class CopyCatWindow : EditorWindow
{
    private static Type t;
    private static UnityEngine.Object copySourceObject;

    private static List<PropertyInfo> properties;
    private static object[] propertiesValue;
    private static bool[] propertiesSelected;

    private static List<FieldInfo> fields;
    private static object[] fieldsValue;
    private static bool[] fieldsSelected;

    private static bool isInPasteMultiMode;

    //for paste multi
    private static UnityEngine.Object[] possiblePasteTargets;
    private static bool[] possiblePasteTargetsSelected;

    //show context menu for all objects that inherit from component
    //for example - right on the header for Button (Script) that's on a game object
    [MenuItem("CONTEXT/Component/CopyCat")]
    public static void CopyMenuSelected(MenuCommand command)
    {
        isInPasteMultiMode = false;

        copySourceObject = command.context;
        t = copySourceObject.GetType();

        properties = new List<PropertyInfo>();
        properties.AddRange(t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        fields = new List<FieldInfo>();
        fields.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.Instance));

        var propertiesToRemove = new List<PropertyInfo>();
        foreach (var property in properties)
        {
            if (!property.CanRead || !property.CanWrite)
            {
                propertiesToRemove.Add(property);
            }
        }

        foreach (var property in propertiesToRemove)
        {
            properties.Remove(property);
        }

        propertiesValue = new object[properties.Count];
        propertiesSelected = new bool[properties.Count];

        fieldsValue = new object[fields.Count];
        fieldsSelected = new bool[fields.Count];

        //launch our window to allow for seletion of what to copy - rendered in OnGUI()
        GetWindow(typeof(CopyCatWindow), true, "It looks like you're trying to copy something, would you like help with that?", true);
    }

    [MenuItem("Edit/PasteCatMulti")]
    public static void PasteMultiMenuSelected()
    {
        isInPasteMultiMode = true;

        //do Resources.FindObjectsOfTypeAll instead to get non scene objects as well maybe?
        possiblePasteTargets = GameObject.FindObjectsOfType(t);

        if (possiblePasteTargets == null)
        {
            Debug.LogError("CopyCat: No possible paste targets!");
            return;
        }

        possiblePasteTargetsSelected = new bool[possiblePasteTargets.Length];

        //launch our window to allow for seletion of what to paste to - rendered in OnGUI()
        GetWindow(typeof(CopyCatWindow), true, "It looks like you're trying to paste something, would you like help with that?", true);
    }

    [MenuItem("CONTEXT/Component/PasteCat")]
    public static void PasteMenuSelected(MenuCommand command)
    {
        var pasteTargetObject = command.context;

        Paste(pasteTargetObject);
    }

    public static void Paste(UnityEngine.Object pasteTargetObject)
    {
        //TODO: undo support

        //nonhomogenous copy/paste? INSANITY
        if (pasteTargetObject.GetType() != copySourceObject.GetType())
        {
            return;
        }

        for (var i = 0; i < propertiesSelected.Length; ++i)
        {
            if (propertiesSelected[i])
            {
                properties[i].SetValue(pasteTargetObject, propertiesValue[i], null);
            }
        }

        for (var i = 0; i < fieldsSelected.Length; ++i)
        {
            if (fieldsSelected[i])
            {
                fields[i].SetValue(pasteTargetObject, fieldsValue[i]);
            }
        }

        //refresh with changes
        //EditorUtility.SetDirty(pasteTargetObject);
        //SceneView.RepaintAll();
    }

    private void OnGUICopy()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Properties");
        for (var i = 0; i < properties.Count; ++i)
        {
            propertiesSelected[i] = EditorGUILayout.Toggle(properties[i].Name, propertiesSelected[i]);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Fields");
        for (var i = 0; i < fields.Count; ++i)
        {
            fieldsSelected[i] = EditorGUILayout.Toggle(fields[i].Name, fieldsSelected[i]);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        //no cancel - but easy to implement, just keep a backup of propertiesSelected & fieldsSelected
        if (GUILayout.Button("CopyCat"))
        {
            for (var i = 0; i < propertiesSelected.Length; ++i)
            {
                if (propertiesSelected[i])
                {
                    propertiesValue[i] = properties[i].GetValue(copySourceObject, null);
                }
            }

            for (var i = 0; i < fieldsSelected.Length; ++i)
            {
                if (fieldsSelected[i])
                {
                    fieldsValue[i] = fields[i].GetValue(copySourceObject);
                }
            }

            this.Close();
        }
    }

    private void OnGUIPasteMulti()
    {
        for (var i = 0; i < possiblePasteTargets.Length; ++i)
        {
            possiblePasteTargetsSelected[i] = EditorGUILayout.Toggle(possiblePasteTargets[i].name, possiblePasteTargetsSelected[i]);
        }

        if (GUILayout.Button("PasteCat"))
        {
            for (var i = 0; i < possiblePasteTargetsSelected.Length; ++i)
            {
                if (possiblePasteTargetsSelected[i])
                {
                    Paste(possiblePasteTargets[i]);
                }
            }

            this.Close();
        }
    }

    public void OnGUI()
    {
        if (!isInPasteMultiMode)
        {
            OnGUICopy();
        }
        else
        {
            OnGUIPasteMulti();
        }
    }
}
