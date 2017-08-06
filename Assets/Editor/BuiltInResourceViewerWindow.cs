using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    private List<UnityEngine.Object> objects;
    private float scrollPos;
    private float maxY;
    private Rect oldPosition;

    private bool showingStyles = true;
    private bool showingIcons = false;

    private string search = string.Empty;

    public void OnGUI()
    {
        if (position.width != oldPosition.width && Event.current.type == EventType.Layout)
        {
            drawings = null;
            oldPosition = position;
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

        string newSearch = GUILayout.TextField(search);
        if (newSearch != search)
        {
            search = newSearch;
            drawings = null;
        }

        float top = 36;

        if (drawings == null)
        {
            string lowerSearch = search.ToLower();

            drawings = new List<Drawing>();

            GUIContent inactiveText = new GUIContent("inactive");
            GUIContent activeText = new GUIContent("active");

            float x = 5.0f;
            float y = 5.0f;

            if (showingStyles)
            {
                foreach (var ss in GUI.skin.customStyles)
                {
                    if (lowerSearch != string.Empty && !ss.name.ToLower().Contains(lowerSearch))
                    {
                        continue;
                    }

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
                        {
                            CopyText("(GUIStyle)\"" + thisStyle.name + "\"");
                        }

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
                if (objects == null)
                {
                    objects = new List<UnityEngine.Object>(Resources.FindObjectsOfTypeAll(typeof(Texture2D)));
                    objects.Sort((pA, pB) => String.Compare(pA.name, pB.name, StringComparison.OrdinalIgnoreCase));
                }

                var rowHeight = 0.0f;

                foreach (var oo in objects)
                {
                    var texture = (Texture)oo;

                    if (texture.name == string.Empty)
                    {
                        continue;
                    }

                    if (lowerSearch != string.Empty && !texture.name.ToLower().Contains(lowerSearch))
                    {
                        continue;
                    }

                    var draw = new Drawing();

                    var width = Mathf.Max(GUI.skin.button.CalcSize(new GUIContent(texture.name)).x, texture.width) + 8.0f;

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
                        {
                            CopyText("EditorGUIUtility.FindTexture( \"" + texture.name + "\" )");
                        }

                        var textureRect = GUILayoutUtility.GetRect(texture.width, texture.width, texture.height, texture.height, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
                        EditorGUI.DrawTextureTransparent(textureRect, texture);
                    };

                    x += width + 8.0f;

                    drawings.Add(draw);
                }
            }

            maxY = y;
        }

        Rect r = position;
        r.y = top;
        r.height -= r.y;
        r.x = r.width - 16;
        r.width = 16;

        var areaHeight = position.height - top;
        scrollPos = GUI.VerticalScrollbar(r, scrollPos, areaHeight, 0.0f, maxY);

        var area = new Rect(0, top, position.width - 16.0f, areaHeight);
        GUILayout.BeginArea(area);

        int count = 0;
        foreach (var draw in drawings)
        {
            Rect newRect = draw.Rect;
            newRect.y -= scrollPos;

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

    private void CopyText(string text)
    {
        var editor = new TextEditor()
        {
            text = text
        };

        editor.SelectAll();
        editor.Copy();
    }
}