using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


public class TextureManagementTreeView : TreeView
{
    internal class Column : MultiColumnHeaderState.Column
    {
        public delegate void DrawEntry(Rect r, TextureManagementTreeViewItemData tvid);
        public delegate int CompareEntry(TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs);

        public DrawEntry drawDelegate;
        public CompareEntry compareDelegate;
    };

	readonly List<TextureManagementTreeViewItemData> Rows = new List<TextureManagementTreeViewItemData>(100);   
    static GUIStyle LabelFieldError;
    static GUIStyle LabelFieldWarn;

    public TextureManagementTreeView(TreeViewState treeViewState) : base(treeViewState)
    {
        if (LabelFieldError == null)
        {
            LabelFieldError = new GUIStyle(EditorStyles.label);
            LabelFieldError.normal.textColor = Color.red;
        }

        if (LabelFieldWarn == null)
        {
            LabelFieldWarn = new GUIStyle(EditorStyles.label);
            LabelFieldWarn.normal.textColor = Color.yellow;
        }

        GatherData();

        var headerState = TextureManagementTreeView.CreateDefaultMultiColumnHeaderState(640f);
        multiColumnHeader = new MultiColumnHeader(headerState);
        multiColumnHeader.sortingChanged += OnSortingChanged;        
        multiColumnHeader.ResizeToFit();

        Reload();
    }

    public static int CompareObjects(object l, object r)
    {
        var iL = l as IComparable;
        var iR = r as IComparable;

        if (iL == null)
        {
            return (iR == null) ? 0 : -1;
        }

        if (iR == null)
            return 1;

        var tL = l.GetType();
        var tR = r.GetType();

        if (!tL.Equals(tR))
        {
            var ret = tL.GetHashCode().CompareTo(tR.GetHashCode());            
            return (ret != 0) ? ret : string.CompareOrdinal(tL.Name, tR.Name);
        }

        return iL.CompareTo(iR);
    }

     void OnSortingChanged(MultiColumnHeader multiColumnHeader)
     {
        var sortIdx = multiColumnHeader.sortedColumnIndex;
        bool ascend = multiColumnHeader.IsSortedAscending(sortIdx);
        var column = (TextureManagementTreeView.Column)this.multiColumnHeader.GetColumn(sortIdx);

        System.Comparison<TextureManagementTreeViewItemData> sortAscend, sortDescend;
        sortAscend = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) => { return column.compareDelegate(lhs, rhs); };
        sortDescend = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) => { return -column.compareDelegate(lhs, rhs); };

        Rows.Sort(ascend ? sortAscend : sortDescend);
     }

    public void GatherData()
    {
        var t2DGuids = AssetDatabase.FindAssets("t:Texture2D");

        foreach (var t2DGuid in t2DGuids)
        {
            var t2dAssetPath = AssetDatabase.GUIDToAssetPath(t2DGuid);
            var t2d = AssetDatabase.LoadAssetAtPath<Texture2D>(t2dAssetPath);
            var t2dImporter = TextureImporter.GetAtPath(t2dAssetPath) as TextureImporter;

            if ((t2dImporter != null) && (t2d != null))
            {
                var tmtvid = new TextureManagementTreeViewItemData(t2d, t2dImporter);
                Rows.Add(tmtvid);            
            }
        }
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var tvid = Rows[args.item.id];
        for (var i = 0; i < args.GetNumVisibleColumns(); ++i)
        {
            CellGUI(args.GetCellRect(i), args.GetColumn(i), args.item.displayName, tvid);
        }
    }

    protected void CellGUI(Rect r, int col, string displayName, TextureManagementTreeViewItemData tvid)
    {
        CenterRectUsingSingleLineHeight(ref r);

        var column = (TextureManagementTreeView.Column)this.multiColumnHeader.GetColumn(col);

        if (column.drawDelegate != null)
        {
            column.drawDelegate(r, tvid);
        }
    }

    protected override TreeViewItem BuildRoot()
    {
        var rowCount = Rows.Count;

        var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };

        var allItems = new List<TreeViewItem>(rowCount);
        for (var i = 0; i < rowCount; ++i)
        {
            var item = new TreeViewItem(i, 0, i.ToString());
            allItems.Add(item);
        }

        SetupParentsAndChildrenFromDepths(root, allItems);
        return root;
    }

    public static bool IsTextureFormatCompressed(string tf)
    {
        return tf.Contains("Compressed");
    }

    public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float width)
    {
        var columns = new[] 
        {
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Path"),
                width = 150, minWidth = 100, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Importer.assetPath, rhs.Importer.assetPath);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Importer.assetPath);
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Type"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Importer.textureType, rhs.Importer.textureType);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Importer.textureType.ToString());
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Import Override"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.ImporterPlatformActiveSettings.overridden, rhs.ImporterPlatformActiveSettings.overridden);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        var overriden = tvid.ImporterPlatformActiveSettings.overridden;
                        EditorGUI.LabelField(r, overriden.ToString(), overriden ? LabelFieldError : EditorStyles.label);
                    }
            },            
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Format"),
                minWidth = 160, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Texture.format, rhs.Texture.format);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        var tfs = TextureUtilWrapper.GetTextureFormatString(tvid.Texture.format);
                        EditorGUI.LabelField(r, tfs, IsTextureFormatCompressed(tfs) ? EditorStyles.label : LabelFieldError);                        
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Storage Size"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        var sizeLhs = TextureUtilWrapper.GetStorageMemorySize(lhs.Texture);
                        var sizeRhs = TextureUtilWrapper.GetStorageMemorySize(rhs.Texture);
                        return CompareObjects(sizeLhs, sizeRhs);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        //fractional kb isn't really worth worrying about so int is fine                       
                        int kb = TextureUtilWrapper.GetStorageMemorySize(tvid.Texture) / 1024;                      
                        EditorGUI.LabelField(r, kb.ToString() + " kB");
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Runtime Size"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        var sizeLhs = TextureUtilWrapper.GetRuntimeMemorySize(lhs.Texture);
                        var sizeRhs = TextureUtilWrapper.GetRuntimeMemorySize(rhs.Texture);
                        return CompareObjects(sizeLhs, sizeRhs);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        //fractional kb isn't really worth worrying about so int is fine
                        int kb = TextureUtilWrapper.GetRuntimeMemorySize(tvid.Texture) / 1024;
                        EditorGUI.LabelField(r, kb.ToString() + " kB");
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Crunch Compression"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Importer.crunchedCompression, rhs.Importer.crunchedCompression);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Importer.crunchedCompression.ToString());
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Width"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Texture.width, rhs.Texture.width);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Texture.width.ToString());
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Height"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Texture.height, rhs.Texture.height);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Texture.height.ToString());
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Aniso"),
                minWidth = 40, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Importer.anisoLevel, rhs.Importer.anisoLevel);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        var lvl = tvid.Importer.anisoLevel;
                        EditorGUI.LabelField(r, tvid.Importer.anisoLevel.ToString(), (lvl == -1) ? LabelFieldWarn : EditorStyles.label);
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("NPot Scale"),
                minWidth = 80, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Importer.npotScale, rhs.Importer.npotScale);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Importer.npotScale.ToString());
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Mip Maps"),
                minWidth = 60, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Importer.mipmapEnabled, rhs.Importer.mipmapEnabled);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Importer.mipmapEnabled.ToString());
                    }
            },
            new TextureManagementTreeView.Column 
            {
                headerContent = new GUIContent("Packing Tag"),
                minWidth = 100, autoResize = true,
                compareDelegate = (TextureManagementTreeViewItemData lhs, TextureManagementTreeViewItemData rhs) =>
                    {
                        return CompareObjects(lhs.Importer.spritePackingTag, rhs.Importer.spritePackingTag);
                    },
                drawDelegate = (Rect r, TextureManagementTreeViewItemData tvid) =>
                    {
                        EditorGUI.LabelField(r, tvid.Importer.spritePackingTag);
                    }
            },
        };

        var state = new MultiColumnHeaderState(columns);
		return state;
    }
}