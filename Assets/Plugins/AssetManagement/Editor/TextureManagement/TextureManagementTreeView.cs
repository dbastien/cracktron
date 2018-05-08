using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


public class TextureManagementTreeView : TreeView
{
    public TextureManagementTreeView(TreeViewState treeViewState) : base(treeViewState)
    {
        Reload();
    }
        
    protected override TreeViewItem BuildRoot()
    {
        // BuildRoot is called every time Reload is called.
        // IDs must be unique.
        // The root item is required to have a depth of -1
        var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
        var allItems = new List<TreeViewItem> 
        {
            new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
            new TreeViewItem {id = 2, depth = 0, displayName = "Mammals"},
            new TreeViewItem {id = 3, depth = 0, displayName = "Tiger"},
            new TreeViewItem {id = 4, depth = 0, displayName = "Elephant"},
            new TreeViewItem {id = 5, depth = 0, displayName = "Okapi"},
            new TreeViewItem {id = 6, depth = 0, displayName = "Armadillo"},
        };
            
        // Utility method that initializes the TreeViewItem.children and .parent for all items.
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
                headerContent = new GUIContent("Width", "Width of processed texture"),
                headerTextAlignment = TextAlignment.Right,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 110, minWidth = 60,
                autoResize = true
            },
            new MultiColumnHeaderState.Column 
            {
                headerContent = new GUIContent("Height", "Height of processed texture"),
                headerTextAlignment = TextAlignment.Right,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 95,	minWidth = 60,
                autoResize = true,
            },
        };

        var state = new MultiColumnHeaderState(columns);
		return state;
    }
}
