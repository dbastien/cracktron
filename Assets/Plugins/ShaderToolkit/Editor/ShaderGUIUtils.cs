﻿using UnityEditor;
using UnityEngine;
using System;

/// <summary>
/// Helper class for custom shader editors
/// </summary>
public static class ShaderGUIUtils
{
    public static readonly GUILayoutOption[] GUILayoutEmptyArray = new GUILayoutOption[0];

    public static int IndentAmount = 1;

    //re-implementation of MaterialEditor internal
    public static Rect GetControlRectForSingleLine()
    {
        return EditorGUILayout.GetControlRect(true, 18f, EditorStyles.layerMaskField, ShaderGUIUtils.GUILayoutEmptyArray);
    }

    //re-implementation of EditorGUI internal
    public static void GetRectsForMiniThumbnailField(Rect position, out Rect thumbRect, out Rect labelRect)
    {
        thumbRect = EditorGUI.IndentedRect(position);
        thumbRect.y -= 1f;
        thumbRect.height = 18f;
        thumbRect.width = 32f;
        var num = thumbRect.x + 30f;
        labelRect = new Rect(num, position.y, thumbRect.x + EditorGUIUtility.labelWidth - num, position.height);
    }

    public static void HeaderSection(string headerText, Action func)
    {
        ShaderGUIUtils.BeginHeader(headerText);
        func();
        ShaderGUIUtils.EndHeader();
    }

    public static void HeaderAutoSection(MaterialEditor matEditor, string headerText, MaterialProperty prop, Action func)
    {
        if (ShaderGUIUtils.BeginHeaderAutoProperty(matEditor, headerText, prop))
        {
            func();
        }
        ShaderGUIUtils.EndHeader();
    }

    public static bool BeginHeaderAutoProperty(MaterialEditor matEditor, string headerText, MaterialProperty prop)
    {
        BeginHeaderProperty(matEditor, headerText, prop);
        return (prop.floatValue > 0f);
    }

    public static void BeginHeaderProperty(MaterialEditor matEditor, string headerText, MaterialProperty prop)
    {
        matEditor.ShaderProperty(prop, GUIContent.none);
        var rect = GUILayoutUtility.GetLastRect();
        EditorGUI.indentLevel += ShaderGUIUtils.IndentAmount;
        EditorGUI.LabelField(rect, headerText, EditorStyles.boldLabel);
    }

    public static void BeginHeader(string headerText)
    {
        EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
        EditorGUI.indentLevel += ShaderGUIUtils.IndentAmount;
    }

    public static void EndHeader()
    {
        EditorGUI.indentLevel -= ShaderGUIUtils.IndentAmount;
    }

    public static void HeaderSeparator()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
    }
}