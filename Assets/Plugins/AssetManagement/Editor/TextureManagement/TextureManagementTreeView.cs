using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


public class TextureManagementTreeView : TreeView
{
	readonly List<TextureManagementTreeViewItemData> Rows = new List<TextureManagementTreeViewItemData>(100);

    public TextureManagementTreeView(TreeViewState treeViewState) : base(treeViewState)
    {
        GatherData();

        var headerState = TextureManagementTreeView.CreateDefaultMultiColumnHeaderState(640f);

        multiColumnHeader = new MultiColumnHeader(headerState);
        multiColumnHeader.ResizeToFit();
        
        Reload();
    }

    public void GatherData()
    {
        var t2DGuids = AssetDatabase.FindAssets("t:Texture2D");

        foreach (var t2DGuid in t2DGuids)
        {
            var t2dAssetPath = AssetDatabase.GUIDToAssetPath(t2DGuid);
            var t2dImporter = TextureImporter.GetAtPath(t2dAssetPath) as TextureImporter;

            if (t2dImporter != null)
            {
                var tmtvid = new TextureManagementTreeViewItemData{ Importer = t2dImporter, test = t2dImporter.anisoLevel };
                Rows.Add(tmtvid);            
            }
        }
    }


    protected override void RowGUI(RowGUIArgs args)
    {
        var tvid = Rows[args.item.id];
        for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
        {
            CellGUI(args.GetCellRect(i), args.GetColumn(i), args.item.displayName, tvid);
        }
    }

    protected void CellGUI(Rect cellRect, int col, string displayName, TextureManagementTreeViewItemData item)
    {
        CenterRectUsingSingleLineHeight(ref cellRect);

        if (col == 0)
        {
            EditorGUI.LabelField(cellRect, displayName);
        }
        else
        {
            //find property with same name as column?
            //EditorGUI.ObjectField(cellRect, "", item.test, "", "",)
            item.test = EditorGUI.FloatField(cellRect, item.test);
        }
    }

    protected override TreeViewItem BuildRoot()
    {
        int arraySize = Rows.Count;

        var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };

        var allItems = new List<TreeViewItem>(arraySize);
        for (int i = 0; i < arraySize; ++i)
        {
            var item = new TreeViewItem(i, 0, i.ToString());
            allItems.Add(item);
        }

        SetupParentsAndChildrenFromDepths(root, allItems);
        return root;
    }

    public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float width)
    {
        var columns = new[] 
        {
            new MultiColumnHeaderState.Column 
            {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150, minWidth = 60,
                autoResize = false,
            },
            new MultiColumnHeaderState.Column 
            {
                headerContent = new GUIContent("Test", "Width of processed texture"),
                headerTextAlignment = TextAlignment.Right,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 110, minWidth = 60,
                autoResize = true
            },
        };

        var state = new MultiColumnHeaderState(columns);
		return state;
    }
}
