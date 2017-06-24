using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[Serializable]
public class PerfAnalyzerWindow : EditorWindow
{
    private bool analyzeMaterialsRan;
    private string[] materialInfo;

    [SerializeField] public GUIStyle settingSuggestedLabel;
    [SerializeField] public GUIStyle settingNotSuggestedLabel;

    [SerializeField] public GUILayoutOption[] layoutColumnWidth = new GUILayoutOption[] { GUILayout.Width(325f) };
    [SerializeField] public GUILayoutOption[] layoutButtonWidth = new GUILayoutOption[] { GUILayout.Width(325f) };

    [MenuItem("Cracktron/Perf Analyzer Window")]
    static void ShowWindow()
    {
        var window = GetWindow<PerfAnalyzerWindow>();
        window.name = "Perf Analyzer";

        window.settingSuggestedLabel = new GUIStyle(EditorStyles.label);
        window.settingSuggestedLabel.normal.textColor = Color.green;

        window.settingNotSuggestedLabel = new GUIStyle(EditorStyles.label);
        window.settingNotSuggestedLabel.normal.textColor = Color.red;

        window.Show();
    }

    public void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
    }

    void OnGUI()
    {
        EditorGUILayout.SelectableLabel("PlayerSettings", EditorStyles.boldLabel);
        PlayerSettings.graphicsJobs = DrawSuggestedSettingBool("graphicsJobs", PlayerSettings.graphicsJobs, true);
        PlayerSettings.virtualRealitySupported = DrawSuggestedSettingBool("virtualRealitySupported", PlayerSettings.virtualRealitySupported, true);
        PlayerSettings.stereoRenderingPath = (StereoRenderingPath)DrawSuggestedSettingEnum("stereoRenderingPath", PlayerSettings.stereoRenderingPath, StereoRenderingPath.SinglePass); //TODO: shouldn't be <=

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.SelectableLabel("QualitySettings", EditorStyles.boldLabel);
        QualitySettings.pixelLightCount = DrawSuggestedSettingInt("pixelLightCount", QualitySettings.pixelLightCount, 1, 0, 16);
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)DrawSuggestedSettingEnum("anisotropicFiltering", QualitySettings.anisotropicFiltering, AnisotropicFiltering.Enable);
        QualitySettings.antiAliasing = DrawSuggestedSettingInt("antiAliasing", QualitySettings.antiAliasing, 0, 0, 8);
        QualitySettings.shadows = (ShadowQuality)DrawSuggestedSettingEnum("shadows", QualitySettings.shadows, ShadowQuality.HardOnly);
        QualitySettings.shadowResolution = (ShadowResolution)DrawSuggestedSettingEnum("shadowResolution", QualitySettings.shadowResolution, ShadowResolution.Medium);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUILayout.Button("Analyze Materials"))
        {
            analyzeMaterialsRan = true;
            this.materialInfo = AnalyzeMaterials();
        }

        if (analyzeMaterialsRan)
        {
            EditorGUILayout.LabelField(this.materialInfo.Length.ToString());
            for (var i = 0; i < this.materialInfo.Length; ++i)
            {
                EditorGUILayout.LabelField(this.materialInfo[i]);
            }
        }
    }

    string[] AnalyzeMaterials()
    {
        var assets = AssetDatabaseUtils.FindAssetsByType<Material>();
        var matInfo = new string[assets.Count];

        for (var i = 0; i < assets.Count; ++i)
        {
            matInfo[i] = assets[i].name;
        }

        return matInfo;
    }

    Enum DrawSuggestedSettingEnum(string friendlyName, Enum curValue, Enum suggestedValue)
    {
        EditorGUILayout.BeginHorizontal();
        var setValue = EditorGUILayout.EnumPopup(friendlyName, curValue, layoutColumnWidth);
        GUILayout.FlexibleSpace();
        bool isSuggestedOrBetter = setValue.CompareTo(suggestedValue) <= 0;

        EditorGUILayout.LabelField("Suggested Value: <= " + suggestedValue.ToString(),
                                   isSuggestedOrBetter ? settingSuggestedLabel : settingNotSuggestedLabel,
                                   layoutColumnWidth);

        if (!isSuggestedOrBetter && GUILayout.Button("Set to suggested", layoutButtonWidth))
        {
            setValue = suggestedValue;
        }

        EditorGUILayout.EndHorizontal();
        return setValue;
    }

    int DrawSuggestedSettingInt(string friendlyName, int curValue, int suggestedValue, int min, int max)
    {
        EditorGUILayout.BeginHorizontal();
        var setValue = EditorGUILayout.IntSlider(friendlyName, curValue, min, max, layoutColumnWidth);
        GUILayout.FlexibleSpace();
        bool isSuggestedOrBetter = setValue.CompareTo(suggestedValue) <= 0;

        EditorGUILayout.LabelField("Suggested Value: <= " + suggestedValue.ToString(), 
                                   isSuggestedOrBetter ? settingSuggestedLabel : settingNotSuggestedLabel,
                                   layoutColumnWidth);

        if (!isSuggestedOrBetter && GUILayout.Button("Set to suggested", layoutButtonWidth))
        {
            setValue = suggestedValue;
        }

        EditorGUILayout.EndHorizontal();
        return setValue;
    }


    bool DrawSuggestedSettingBool(string friendlyName, bool curValue, bool suggestedValue)
    {
        EditorGUILayout.BeginHorizontal();
        var setValue = EditorGUILayout.Toggle(friendlyName, curValue, layoutColumnWidth);
        GUILayout.FlexibleSpace();
        bool isSuggested = (setValue == suggestedValue);

        EditorGUILayout.LabelField("Suggested Value: " + suggestedValue.ToString(),
                                   isSuggested ? settingSuggestedLabel : settingNotSuggestedLabel,
                                   layoutColumnWidth);

        if (!isSuggested && GUILayout.Button("Set to suggested", layoutButtonWidth))
        {
            setValue = suggestedValue;
        }

        EditorGUILayout.EndHorizontal();
        return setValue;
    }
}

