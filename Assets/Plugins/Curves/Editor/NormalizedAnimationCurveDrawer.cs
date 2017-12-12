using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomPropertyDrawer(typeof(NormalizedAnimationCurveAttribute))]
public class NormalizedAnimationCurveDrawer : PropertyDrawer
{
    private static ScriptableObject presets;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        EditorGUI.BeginProperty(position, label, property);

        var oldCurve = property.animationCurveValue;

        int wrapMode = (int)oldCurve.preWrapMode;

        property.animationCurveValue = EditorGUILayout.CurveField(property.animationCurveValue, GUILayout.Height(100f), GUILayout.Width(100f));

        EditorGUI.BeginChangeCheck();

        wrapMode = (int)(WrapModeUIFriendly)EditorGUILayout.EnumPopup((WrapModeUIFriendly)wrapMode);

        if (EditorGUI.EndChangeCheck())
        {
            var tempCurve = property.animationCurveValue;
            tempCurve.preWrapMode = (WrapMode)wrapMode;
            tempCurve.postWrapMode = (WrapMode)wrapMode;
            property.animationCurveValue = tempCurve;
            property.serializedObject.ApplyModifiedProperties();
        }

        var curveItemSize = new Vector2(40f, 40f);
        var curveItemPadding = new Vector2(5f, 5f);

        var presetCount = CurvePresetLibraryWrapper.Count(NormalizedAnimationCurveDrawer.presets);

        //TODO: find size of layoutable width - controls are inset
        var rowItems = Mathf.FloorToInt(Screen.width / (curveItemSize.x + curveItemPadding.x)) - 1;

        int p = 0;
        while (p < presetCount)
        {
            EditorGUILayout.BeginHorizontal();
            int itemsThisRow = Mathf.Min(presetCount - p, rowItems);
            for (int i = 0; i < itemsThisRow; ++i)
            {
                var rect = GUILayoutUtility.GetRect(curveItemSize.x,
                                                    curveItemSize.y,
                                                    GUILayout.Height(curveItemSize.x),
                                                    GUILayout.Width(curveItemSize.y));

                var curveName = CurvePresetLibraryWrapper.GetName(NormalizedAnimationCurveDrawer.presets, p);

                if (GUI.Button(rect, new GUIContent(string.Empty, curveName)))
                {
                    var animationCurve = CurvePresetLibraryWrapper.GetPreset(NormalizedAnimationCurveDrawer.presets, p);
                    animationCurve.preWrapMode = (WrapMode)wrapMode;
                    animationCurve.postWrapMode = (WrapMode)wrapMode;
                    property.animationCurveValue = animationCurve;
                }
                if (Event.current.type == EventType.repaint)
                {
                    CurvePresetLibraryWrapper.Draw(NormalizedAnimationCurveDrawer.presets, rect, p);
                }
                if (i != itemsThisRow - 1)
                {
                    GUILayout.Space(curveItemPadding.x);
                }
                ++p;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(curveItemPadding.y);
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }

    //todo: ideally also triggers when asset database refreshes
    [DidReloadScripts]
    private static void LoadPresets()
    {
        var path = Application.dataPath + CurveConstants.NormalizedCurvesPath;
        var objs = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(path);

        NormalizedAnimationCurveDrawer.presets = objs[0] as ScriptableObject;
    }
}