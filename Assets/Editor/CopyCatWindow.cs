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
        CopyCatWindow.isInPasteMultiMode = false;

        CopyCatWindow.copySourceObject = command.context;
        CopyCatWindow.t = CopyCatWindow.copySourceObject.GetType();

        CopyCatWindow.properties = new List<PropertyInfo>();
        CopyCatWindow.properties.AddRange(CopyCatWindow.t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        CopyCatWindow.fields = new List<FieldInfo>();
        CopyCatWindow.fields.AddRange(CopyCatWindow.t.GetFields(BindingFlags.Public | BindingFlags.Instance));

        var propertiesToRemove = new List<PropertyInfo>();
        foreach (var property in CopyCatWindow.properties)
        {
            if (!property.CanRead || !property.CanWrite)
            {
                propertiesToRemove.Add(property);
            }
        }

        foreach (var property in propertiesToRemove)
        {
            CopyCatWindow.properties.Remove(property);
        }

        CopyCatWindow.propertiesValue = new object[CopyCatWindow.properties.Count];
        CopyCatWindow.propertiesSelected = new bool[CopyCatWindow.properties.Count];

        CopyCatWindow.fieldsValue = new object[CopyCatWindow.fields.Count];
        CopyCatWindow.fieldsSelected = new bool[CopyCatWindow.fields.Count];

        //launch our window to allow for seletion of what to copy - rendered in OnGUI()
        EditorWindow.GetWindow(typeof(CopyCatWindow), true, "It looks like you're trying to copy something, would you like help with that?", true);
    }

    [MenuItem("Edit/PasteCatMulti")]
    public static void PasteMultiMenuSelected()
    {
        CopyCatWindow.isInPasteMultiMode = true;

        //do Resources.FindObjectsOfTypeAll instead to get non scene objects as well maybe?
        CopyCatWindow.possiblePasteTargets = Object.FindObjectsOfType(CopyCatWindow.t);

        if (CopyCatWindow.possiblePasteTargets == null)
        {
            Debug.LogError("CopyCat: No possible paste targets!");
            return;
        }

        CopyCatWindow.possiblePasteTargetsSelected = new bool[CopyCatWindow.possiblePasteTargets.Length];

        //launch our window to allow for seletion of what to paste to - rendered in OnGUI()
        EditorWindow.GetWindow(typeof(CopyCatWindow), true, "It looks like you're trying to paste something, would you like help with that?", true);
    }

    [MenuItem("CONTEXT/Component/PasteCat")]
    public static void PasteMenuSelected(MenuCommand command)
    {
        var pasteTargetObject = command.context;

        CopyCatWindow.Paste(pasteTargetObject);
    }

    public static void Paste(UnityEngine.Object pasteTargetObject)
    {
        //TODO: undo support

        //nonhomogenous copy/paste? INSANITY
        if (pasteTargetObject.GetType() != CopyCatWindow.copySourceObject.GetType())
        {
            return;
        }

        for (var i = 0; i < CopyCatWindow.propertiesSelected.Length; ++i)
        {
            if (CopyCatWindow.propertiesSelected[i])
            {
                CopyCatWindow.properties[i].SetValue(pasteTargetObject, CopyCatWindow.propertiesValue[i], null);
            }
        }

        for (var i = 0; i < CopyCatWindow.fieldsSelected.Length; ++i)
        {
            if (CopyCatWindow.fieldsSelected[i])
            {
                CopyCatWindow.fields[i].SetValue(pasteTargetObject, CopyCatWindow.fieldsValue[i]);
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
        for (var i = 0; i < CopyCatWindow.properties.Count; ++i)
        {
            CopyCatWindow.propertiesSelected[i] = EditorGUILayout.Toggle(CopyCatWindow.properties[i].Name, CopyCatWindow.propertiesSelected[i]);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Fields");
        for (var i = 0; i < CopyCatWindow.fields.Count; ++i)
        {
            CopyCatWindow.fieldsSelected[i] = EditorGUILayout.Toggle(CopyCatWindow.fields[i].Name, CopyCatWindow.fieldsSelected[i]);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        //no cancel - but easy to implement, just keep a backup of propertiesSelected & fieldsSelected
        if (GUILayout.Button("CopyCat"))
        {
            for (var i = 0; i < CopyCatWindow.propertiesSelected.Length; ++i)
            {
                if (CopyCatWindow.propertiesSelected[i])
                {
                    CopyCatWindow.propertiesValue[i] = CopyCatWindow.properties[i].GetValue(CopyCatWindow.copySourceObject, null);
                }
            }

            for (var i = 0; i < CopyCatWindow.fieldsSelected.Length; ++i)
            {
                if (CopyCatWindow.fieldsSelected[i])
                {
                    CopyCatWindow.fieldsValue[i] = CopyCatWindow.fields[i].GetValue(CopyCatWindow.copySourceObject);
                }
            }

            this.Close();
        }
    }

    private void OnGUIPasteMulti()
    {
        for (var i = 0; i < CopyCatWindow.possiblePasteTargets.Length; ++i)
        {
            CopyCatWindow.possiblePasteTargetsSelected[i] = EditorGUILayout.Toggle(CopyCatWindow.possiblePasteTargets[i].name, CopyCatWindow.possiblePasteTargetsSelected[i]);
        }

        if (GUILayout.Button("PasteCat"))
        {
            for (var i = 0; i < CopyCatWindow.possiblePasteTargetsSelected.Length; ++i)
            {
                if (CopyCatWindow.possiblePasteTargetsSelected[i])
                {
                    CopyCatWindow.Paste(CopyCatWindow.possiblePasteTargets[i]);
                }
            }

            this.Close();
        }
    }

    public void OnGUI()
    {
        if (!CopyCatWindow.isInPasteMultiMode)
        {
            this.OnGUICopy();
        }
        else
        {
            this.OnGUIPasteMulti();
        }
    }
}
