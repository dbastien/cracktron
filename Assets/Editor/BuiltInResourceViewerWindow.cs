﻿using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class BuiltInResourceViewerWindow : EditorWindow
{
    [MenuItem("Window/Built-in styles and icons")]
    public static void ShowWindow()
    {
        var w = EditorWindow.GetWindow<BuiltInResourceViewerWindow>();
        w.Show();
    }

    private struct Drawing
    {
        public Rect Rect;
        public Action Draw;
    }

    private List<Drawing> drawings;

    private List<UnityEngine.Object> _objects;
    private float _scrollPos;
    private float _maxY;
    private Rect _oldPosition;

    private bool showingStyles = true;
    private bool showingIcons = false;

    private string _search = "";

    void OnGUI()
    {
        if (position.width != _oldPosition.width && Event.current.type == EventType.Layout)
        {
            drawings = null;
            _oldPosition = position;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Toggle(showingStyles, "Styles", EditorStyles.toolbarButton) != showingStyles)
        {
            showingStyles = !showingStyles;
            showingIcons = !showingStyles;
            drawings = null;
        }

        if (GUILayout.Toggle(showingIcons, "Icons", EditorStyles.toolbarButton) != showingIcons)
        {
            showingIcons = !showingIcons;
            showingStyles = !showingIcons;
            drawings = null;
        }

        GUILayout.EndHorizontal();

        string newSearch = GUILayout.TextField(_search);
        if (newSearch != _search)
        {
            _search = newSearch;
            drawings = null;
        }

        float top = 36;

        if (drawings == null)
        {
            string lowerSearch = _search.ToLower();

            drawings = new List<Drawing>();

            GUIContent inactiveText = new GUIContent("inactive");
            GUIContent activeText = new GUIContent("active");

            float x = 5.0f;
            float y = 5.0f;

            if (showingStyles)
            {
                foreach (var ss in GUI.skin.customStyles)
                {
                    if (lowerSearch != "" && !ss.name.ToLower().Contains(lowerSearch))
                        continue;

                    var thisStyle = ss;

                    var draw = new Drawing();

                    var width = Mathf.Max(100.0f,
                                          GUI.skin.button.CalcSize(new GUIContent(ss.name)).x,
                                          ss.CalcSize(inactiveText, activeText).x) 
                                          + 16.0f;

                    var height = 60.0f;

                    if (x + width > position.width - 32 && x > 5.0f)
                    {
                        x = 5.0f;
                        y += height + 10.0f;
                    }

                    draw.Rect = new Rect(x, y, width, height);

                    width -= 8.0f;

                    draw.Draw = () =>
                    {
                        if (GUILayout.Button(thisStyle.name, GUILayout.Width(width)))
                            CopyText("(GUIStyle)\"" + thisStyle.name + "\"");

                        GUILayout.BeginHorizontal();
                        GUILayout.Toggle(false, inactiveText, thisStyle, GUILayout.Width(width / 2));
                        GUILayout.Toggle(false, activeText, thisStyle, GUILayout.Width(width / 2));
                        GUILayout.EndHorizontal();
                    };

                    x += width + 18.0f;

                    drawings.Add(draw);
                }
            }
            else if (showingIcons)
            {
                if (_objects == null)
                {
                    _objects = new List<UnityEngine.Object>(Resources.FindObjectsOfTypeAll(typeof(Texture2D)));
                    _objects.Sort((pA, pB) => String.Compare(pA.name, pB.name, StringComparison.OrdinalIgnoreCase));
                }

                var rowHeight = 0.0f;

                foreach (var oo in _objects)
                {
                    var texture = (Texture)oo;

                    if (texture.name == "")
                        continue;

                    if (lowerSearch != "" && !texture.name.ToLower().Contains(lowerSearch))
                        continue;

                    var draw = new Drawing();

                    var width = Mathf.Max(
                        GUI.skin.button.CalcSize(new GUIContent(texture.name)).x,
                        texture.width
                    ) + 8.0f;

                    float height = texture.height + GUI.skin.button.CalcSize(new GUIContent(texture.name)).y + 8.0f;

                    if (x + width > position.width - 32.0f)
                    {
                        x = 5.0f;
                        y += rowHeight + 8.0f;
                        rowHeight = 0.0f;
                    }

                    draw.Rect = new Rect(x, y, width, height);

                    rowHeight = Mathf.Max(rowHeight, height);

                    width -= 8.0f;

                    draw.Draw = () =>
                    {
                        if (GUILayout.Button(texture.name, GUILayout.Width(width)))
                            CopyText("EditorGUIUtility.FindTexture( \"" + texture.name + "\" )");

                        var textureRect = GUILayoutUtility.GetRect(texture.width, texture.width, texture.height, texture.height, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
                        EditorGUI.DrawTextureTransparent(textureRect, texture);
                    };

                    x += width + 8.0f;

                    drawings.Add(draw);
                }
            }

            _maxY = y;
        }

        Rect r = position;
        r.y = top;
        r.height -= r.y;
        r.x = r.width - 16;
        r.width = 16;

        var areaHeight = position.height - top;
        _scrollPos = GUI.VerticalScrollbar(r, _scrollPos, areaHeight, 0.0f, _maxY);

        var area = new Rect(0, top, position.width - 16.0f, areaHeight);
        GUILayout.BeginArea(area);

        int count = 0;
        foreach (var draw in drawings)
        {
            Rect newRect = draw.Rect;
            newRect.y -= _scrollPos;

            if (newRect.y + newRect.height > 0 && newRect.y < areaHeight)
            {
                GUILayout.BeginArea(newRect, GUI.skin.textField);
                draw.Draw();
                GUILayout.EndArea();

                ++count;
            }
        }

        GUILayout.EndArea();
    }

    void CopyText(string pText)
    {
        var editor = new TextEditor();

        editor.text = pText;

        editor.SelectAll();
        editor.Copy();
    }
}