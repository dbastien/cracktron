// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

internal class SerializedPropertyDataStore
{
    internal class Data
    {
        UnityEngine.Object m_Object;
        SerializedObject m_SerializedObject;
        SerializedProperty[] m_Properties;

        public Data(UnityEngine.Object obj, string[] props)
        {
            m_Object = obj;
            m_SerializedObject = new SerializedObject(obj);
            m_Properties = new SerializedProperty[props.Length];

            for (int i = 0; i < props.Length; i++)
                m_Properties[i] = m_SerializedObject.FindProperty(props[i]);
        }

        public void Dispose()
        {
            foreach (var prop in m_Properties)
            {
                if (prop != null)
                    prop.Dispose();
            }

            m_SerializedObject.Dispose();

            m_Object            = null;
            m_SerializedObject  = null;
            m_Properties        = null;
        }

        public string name { get { return m_Object != null ? m_Object.name : string.Empty; } }
        public SerializedObject serializedObject { get { return m_SerializedObject; } }
        public SerializedProperty[] properties { get { return m_Properties; } }

        public int objectId
        {
            get
            {
                if (!m_Object)
                    return 0;

                Component comp = m_Object as Component;
                return comp != null ? comp.gameObject.GetInstanceID() : m_Object.GetInstanceID();
            }
        }

        public bool Update() { return m_Object != null ? m_SerializedObject.UpdateIfRequiredOrScript() : false; }
        public void Store() { if (m_Object != null) {  m_SerializedObject.ApplyModifiedProperties(); } }
    }

    // LightTreeDataStore members
    internal delegate UnityEngine.Object[] GatherDelegate();
    UnityEngine.Object[] m_Objects;
    Data[] m_Elements;
    string[] m_PropNames;
    GatherDelegate m_GatherDel;

    public Data[] GetElements() { return m_Elements; }

    public SerializedPropertyDataStore(string[] propNames, GatherDelegate gatherDel)
    {
        m_PropNames = propNames;
        m_GatherDel = gatherDel;

        Repopulate();
    }

    ~SerializedPropertyDataStore()
    {
        Clear();
    }

    public bool Repopulate()
    {
        UnityEngine.Object[] objs = m_GatherDel();

        if (m_Objects != null)
        {
            // If nothing's changed -> bail
            if (objs.Length == m_Objects.Length)
                return false;

            Clear();
        }

        // Recreate data store
        m_Objects = objs;
        m_Elements = new Data[objs.Length];
        for (int i = 0; i < objs.Length; i++)
            m_Elements[i] = new Data(objs[i], m_PropNames);

        return true;
    }

    private void Clear()
    {
        // Clear current
        for (int i = 0; i < m_Elements.Length; i++)
            m_Elements[i].Dispose();

        m_Objects  = null;
        m_Elements = null;
    }
}

internal class SerializedPropertyTreeView : TreeView
{
    internal static class Styles
    {
        public static readonly GUIStyle entryEven = "OL EntryBackEven";
        public static readonly GUIStyle entryOdd  = "OL EntryBackOdd";
        public static readonly string focusHelper = "SerializedPropertyTreeViewFocusHelper";
        public static readonly string serializeFilterSelection = "_FilterSelection";
        public static readonly string serializeFilterDisable   = "_FilterDisable";
        public static readonly string serializeFilterInvert    = "_FilterInvert";
        public static readonly string serializeTreeViewState    = "_TreeViewState";
        public static readonly string serializeColumnHeaderState = "_ColumnHeaderState";
        public static readonly string serializeFilter = "_Filter_";
    }

    // this gets stuffed into the view and displayed on screen. It is a visible subset of the actual data
    internal class SerializedPropertyItem : TreeViewItem
    {
        SerializedPropertyDataStore.Data m_Data;
        public SerializedPropertyItem(int id, int depth, SerializedPropertyDataStore.Data ltd) : base(id, depth, ltd != null ? ltd.name : "root")
        {
            m_Data = ltd;
        }

        public SerializedPropertyDataStore.Data GetData() { return m_Data; }
    }

    internal class Column : MultiColumnHeaderState.Column
    {
        public delegate void DrawEntry(Rect r, SerializedProperty prop, SerializedProperty[] dependencies);
        public delegate int CompareEntry(SerializedProperty lhs, SerializedProperty rhs);
        public delegate void CopyDelegate(SerializedProperty target, SerializedProperty source);

        public string                               propertyName;
        public int[]                                dependencyIndices;
        public DrawEntry                            drawDelegate;
        public CompareEntry                         compareDelegate;
        public CopyDelegate                         copyDelegate;
    };

    private struct ColumnInternal
    {
        public SerializedProperty[] dependencyProps;    // helper array, must be the same size as dependencyIndices
    };

    internal class DefaultDelegates
    {
        public static readonly Column.DrawEntry s_DrawDefault = (Rect r, SerializedProperty prop, SerializedProperty[] dependencies) =>
            {
                EditorGUI.PropertyField(r, prop, GUIContent.none);
            };
        public static readonly Column.DrawEntry s_DrawCheckbox = (Rect r, SerializedProperty prop, SerializedProperty[] dependencies) =>
            {
                float off = (r.width / 2) - 8;
                r.x += off >= 0 ? off : 0;
                EditorGUI.PropertyField(r, prop, GUIContent.none);
            };
        public static readonly Column.DrawEntry s_DrawName = (Rect r, SerializedProperty prop, SerializedProperty[] dependencies) => {};

        public static readonly Column.CopyDelegate s_CopyDefault = (SerializedProperty target, SerializedProperty source) =>
            {
                target.serializedObject.CopyFromSerializedProperty(source);
            };
    }
    // reference to the data store (not owned by this class)
    SerializedPropertyDataStore m_DataStore;
    ColumnInternal[]            m_ColumnsInternal;
    List<TreeViewItem>          m_Items;
    int                         m_ChangedId;
    bool                        m_bFilterSelection;
    int[]                       m_SelectionFilter;

    public SerializedPropertyTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, SerializedPropertyDataStore dataStore) : base(state, multicolumnHeader)
    {
        m_DataStore = dataStore;
        // initialize internal data for the columns
        int colcnt = multiColumnHeader.state.columns.Length;
        m_ColumnsInternal = new ColumnInternal[colcnt];
        for (int i = 0; i < colcnt; i++)
        {
            Column c = Col(i);
            if (c.propertyName != null)
                m_ColumnsInternal[i].dependencyProps = new SerializedProperty[c.propertyName.Length];
        }

        multiColumnHeader.visibleColumnsChanged += OnVisibleColumnChanged;

        showAlternatingRowBackgrounds = true;
        showBorder = true;
        rowHeight = EditorGUIUtility.singleLineHeight;
    }

    public void SerializeState(string uid)
    {
        SessionState.SetBool(uid + Styles.serializeFilterSelection, m_bFilterSelection);

        SessionState.SetString(uid + Styles.serializeTreeViewState, JsonUtility.ToJson(state));
        SessionState.SetString(uid + Styles.serializeColumnHeaderState, JsonUtility.ToJson(multiColumnHeader.state));
    }

    public void DeserializeState(string uid)
    {
        m_bFilterSelection = SessionState.GetBool(uid + Styles.serializeFilterSelection, false);

        string treeViewState     = SessionState.GetString(uid + Styles.serializeTreeViewState, "");
        string columnHeaderState = SessionState.GetString(uid + Styles.serializeColumnHeaderState, "");

        if (!string.IsNullOrEmpty(treeViewState))
            JsonUtility.FromJsonOverwrite(treeViewState, state);
        if (!string.IsNullOrEmpty(columnHeaderState))
            JsonUtility.FromJsonOverwrite(columnHeaderState, multiColumnHeader.state);
    }

    public bool IsFilteredDirty()
    {
        return m_ChangedId != 0 && (m_ChangedId != GUIUtility.keyboardControl || !EditorGUIUtility.editingTextField);
    }

    public bool Update()
    {
        var rows = GetRows();
        int first, last;

        GetFirstAndLastVisibleRows(out first, out last);

        bool changed = false;

        if (last != -1)
        {
            for (int i = first; i <= last; i++)
            {
                changed = changed || ((SerializedPropertyItem)rows[i]).GetData().Update();
            }
        }

        return changed;
    }

    public void FullReload()
    {
        m_Items = null;
        Reload();
    }

    protected override TreeViewItem BuildRoot()
    {
        return new SerializedPropertyItem(-1, -1, null);
    }

    // this gets called whenever someone issues a reload() call to the parent
    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
        if (m_Items == null)
        {
            var rawData = m_DataStore.GetElements();
            m_Items = new List<TreeViewItem>(rawData.Length);

            for (int i = 0; i < rawData.Length; i++)
            {
                var item = new SerializedPropertyItem(rawData[i].objectId, 0, rawData[i]);
                m_Items.Add(item);
            }
        }

        // filtering
        IEnumerable<TreeViewItem> tmprows = m_Items;

        if (m_bFilterSelection)
        {
            if (m_SelectionFilter == null)
                m_SelectionFilter = Selection.instanceIDs;
            tmprows = m_Items.Where((TreeViewItem item) => { return m_SelectionFilter.Contains(item.id); });
        }
        else
            m_SelectionFilter = null;

        var rows = tmprows.ToList();

        m_ChangedId = 0;

        // We still need to setup the child parent information for the rows since this
        // information is used by the TreeView internal logic (navigation, dragging etc)
        SetupParentsAndChildrenFromDepths(root, rows);

        return rows;
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (SerializedPropertyItem)args.item;

        for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
        {
            CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
        }
    }

    void CellGUI(Rect cellRect, SerializedPropertyItem item, int columnIndex, ref RowGUIArgs args)
    {
        CenterRectUsingSingleLineHeight(ref cellRect);
        var ltd = item.GetData();
        Column column = (Column)this.multiColumnHeader.GetColumn(columnIndex);

        if (column.drawDelegate == DefaultDelegates.s_DrawName)
        {
            DefaultGUI.Label(cellRect, ltd.name, IsSelected(args.item.id), false);
        }
        else if (column.drawDelegate != null)
        {
            SerializedProperty[] props = ltd.properties;
            int depcnt = column.dependencyIndices != null ? column.dependencyIndices.Length : 0;

            for (int i = 0; i < depcnt; i++)
                m_ColumnsInternal[columnIndex].dependencyProps[i] = props[column.dependencyIndices[i]];

            // allow to capture tabs
            if (args.item.id == state.lastClickedID && HasFocus() && columnIndex == multiColumnHeader.state.visibleColumns[multiColumnHeader.state.visibleColumns[0] == 0 ? 1 : 0])
                GUI.SetNextControlName(Styles.focusHelper);

            SerializedProperty prop = ltd.properties[columnIndex];

            EditorGUI.BeginChangeCheck();

            column.drawDelegate(cellRect, prop, m_ColumnsInternal[columnIndex].dependencyProps);

            if (EditorGUI.EndChangeCheck())
            {
                // update all selected items if the current row was part of the selection list
                ltd.Store();

                var selIds = GetSelection();

                if (selIds.Contains(ltd.objectId))
                {
                    IList<TreeViewItem> rows = FindRows(selIds);

                    Undo.RecordObjects(rows.Select(r => ((SerializedPropertyItem)r).GetData().serializedObject.targetObject).ToArray(), "Modify Multiple Properties");

                    foreach (var r in rows)
                    {
                        if (r.id == args.item.id)
                            continue;

                        var data = ((SerializedPropertyItem)r).GetData();

                        if (!IsEditable(data.serializedObject.targetObject))
                            continue;

                        if (column.copyDelegate != null)
                            column.copyDelegate(data.properties[columnIndex], prop);
                        else
                            DefaultDelegates.s_CopyDefault(data.properties[columnIndex], prop);

                        data.Store();
                    }
                }
            }
        }
    }

    private static bool IsEditable(Object target)
    {
        return ((target.hideFlags & HideFlags.NotEditable) == 0);
    }

    protected override void BeforeRowsGUI()
    {
        var rows = GetRows();
        int first, last;
        GetFirstAndLastVisibleRows(out first, out last);

        if (last != -1)
        {
            for (int i = first; i <= last; i++)
                ((SerializedPropertyItem)rows[i]).GetData().Update();
        }

        // don't forget to refresh selected objects that got shoved off screen
        var selected = FindRows(GetSelection());
        foreach (SerializedPropertyItem sel in selected)
            sel.GetData().Update();

        base.BeforeRowsGUI();
    }

    protected override void KeyEvent()
    {
        if (Event.current.type != EventType.KeyDown)
            return;

        // We always focus as this callback only fires when the name column is selected.
        // Tabbing through the property columns never goes through here.
        // Also note that we're not capturing the KeyCode Tab. There are 2 tab events,
        // one for the keycode, and one for the character code. The keycode comes first,
        // so if we change the focus based on the keycode, we never get the second character code
        // event as the treeview loses focus and doesn't fire the callback. The second tab event
        // then gets handled as an ordinary tab, leading to a double tab.
        if (Event.current.character == '\t')
        {
            GUI.FocusControl(Styles.focusHelper);
            Event.current.Use();
        }
    }

    void OnVisibleColumnChanged(MultiColumnHeader header)
    {
        Reload();
    }

    private bool IsColumnVisible(int idx)
    {
        for (int i = 0; i < multiColumnHeader.state.visibleColumns.Length; i++)
            if (multiColumnHeader.state.visibleColumns[i] == idx)
                return true;

        return false;
    }

    // casting helper
    private Column Col(int idx) { return (Column)multiColumnHeader.state.columns[idx]; }
}
