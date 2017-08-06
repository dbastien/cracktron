using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuiltInResourceViewerWindow : EditorWindow
{
    [MenuItem("Window/Built-in styles and icons")]
    public static void ShowWindow()
    {
        var w = GetWindow<BuiltInResourceViewerWindow>();
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
        if (this.position.width != this.oldPosition.width && Event.current.type == EventType.Layout)
        {
            this.drawings = null;
            this.oldPosition = this.position;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Toggle(this.showingStyles, "Styles", EditorStyles.toolbarButton) != this.showingStyles)
        {
            this.showingStyles = !this.showingStyles;
            this.showingIcons = !this.showingStyles;
            this.drawings = null;
        }

        if (GUILayout.Toggle(this.showingIcons, "Icons", EditorStyles.toolbarButton) != this.showingIcons)
        {
            this.showingIcons = !this.showingIcons;
            this.showingStyles = !this.showingIcons;
            this.drawings = null;
        }

        GUILayout.EndHorizontal();

        string newSearch = GUILayout.TextField(this.search);
        if (newSearch != this.search)
        {
            this.search = newSearch;
            this.drawings = null;
        }

        float top = 36;

        if (this.drawings == null)
        {
            string lowerSearch = this.search.ToLower();

            this.drawings = new List<Drawing>();

            GUIContent inactiveText = new GUIContent("inactive");
            GUIContent activeText = new GUIContent("active");

            float x = 5.0f;
            float y = 5.0f;

            if (this.showingStyles)
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

                    if (x + width > this.position.width - 32 && x > 5.0f)
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
                            this.CopyText("(GUIStyle)\"" + thisStyle.name + "\"");
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.Toggle(false, inactiveText, thisStyle, GUILayout.Width(width / 2));
                        GUILayout.Toggle(false, activeText, thisStyle, GUILayout.Width(width / 2));
                        GUILayout.EndHorizontal();
                    };

                    x += width + 18.0f;

                    this.drawings.Add(draw);
                }
            }
            else if (this.showingIcons)
            {
                if (this.objects == null)
                {
                    this.objects = new List<UnityEngine.Object>(Resources.FindObjectsOfTypeAll(typeof(Texture2D)));
                    this.objects.Sort((pA, pB) => String.Compare(pA.name, pB.name, StringComparison.OrdinalIgnoreCase));
                }

                var rowHeight = 0.0f;

                foreach (var oo in this.objects)
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

                    if (x + width > this.position.width - 32.0f)
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
                            this.CopyText("EditorGUIUtility.FindTexture( \"" + texture.name + "\" )");
                        }

                        var textureRect = GUILayoutUtility.GetRect(texture.width, texture.width, texture.height, texture.height, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));
                        EditorGUI.DrawTextureTransparent(textureRect, texture);
                    };

                    x += width + 8.0f;

                    this.drawings.Add(draw);
                }
            }

            this.maxY = y;
        }

        Rect r = this.position;
        r.y = top;
        r.height -= r.y;
        r.x = r.width - 16;
        r.width = 16;

        var areaHeight = this.position.height - top;
        this.scrollPos = GUI.VerticalScrollbar(r, this.scrollPos, areaHeight, 0.0f, this.maxY);

        var area = new Rect(0, top, this.position.width - 16.0f, areaHeight);
        GUILayout.BeginArea(area);

        int count = 0;
        foreach (var draw in this.drawings)
        {
            Rect newRect = draw.Rect;
            newRect.y -= this.scrollPos;

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