using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PerfAnalyzerWindow : EditorWindow
{
    [SerializeField] public GUIStyle settingSuggestedLabel;
    [SerializeField] public GUIStyle settingNotSuggestedLabel;

    [SerializeField] public GUILayoutOption[] layoutColumnWidth = { GUILayout.Width(325f) };
    [SerializeField] public GUILayoutOption[] layoutButtonWidth = { GUILayout.Width(325f) };

    private bool analyzeMaterialsRan;
    private string[] materialInfo;

    [MenuItem("Cracktron/Perf Analyzer Window")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<PerfAnalyzerWindow>();
        window.name = "Perf Analyzer";

        window.settingSuggestedLabel = new GUIStyle(EditorStyles.label);
        window.settingSuggestedLabel.normal.textColor = Color.green;

        window.settingNotSuggestedLabel = new GUIStyle(EditorStyles.label);
        window.settingNotSuggestedLabel.normal.textColor = Color.red;

        window.Show();
    }

    public void OnEnable()
    {
        this.hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnGUI()
    {
        EditorGUILayout.SelectableLabel("PlayerSettings", EditorStyles.boldLabel);
        PlayerSettings.graphicsJobs = this.DrawSuggestedSettingBool("graphicsJobs", PlayerSettings.graphicsJobs, true);
        PlayerSettings.virtualRealitySupported = this.DrawSuggestedSettingBool("virtualRealitySupported", PlayerSettings.virtualRealitySupported, true);
        PlayerSettings.stereoRenderingPath = (StereoRenderingPath)this.DrawSuggestedSettingEnum("stereoRenderingPath", PlayerSettings.stereoRenderingPath, StereoRenderingPath.SinglePass); //TODO: shouldn't be <=

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.SelectableLabel("QualitySettings", EditorStyles.boldLabel);
        QualitySettings.pixelLightCount = this.DrawSuggestedSettingInt("pixelLightCount", QualitySettings.pixelLightCount, 1, 0, 16);
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)this.DrawSuggestedSettingEnum("anisotropicFiltering", QualitySettings.anisotropicFiltering, AnisotropicFiltering.Enable);
        QualitySettings.antiAliasing = this.DrawSuggestedSettingInt("antiAliasing", QualitySettings.antiAliasing, 0, 0, 8);
        QualitySettings.shadows = (ShadowQuality)this.DrawSuggestedSettingEnum("shadows", QualitySettings.shadows, ShadowQuality.HardOnly);
        QualitySettings.shadowResolution = (ShadowResolution)this.DrawSuggestedSettingEnum("shadowResolution", QualitySettings.shadowResolution, ShadowResolution.Medium);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUILayout.Button("Analyze Materials"))
        {
            this.analyzeMaterialsRan = true;
            this.materialInfo = this.AnalyzeMaterials();
        }

        if (this.analyzeMaterialsRan)
        {
            EditorGUILayout.LabelField(this.materialInfo.Length.ToString());
            for (var i = 0; i < this.materialInfo.Length; ++i)
            {
                EditorGUILayout.LabelField(this.materialInfo[i]);
            }
        }
    }

    private string[] AnalyzeMaterials()
    {
        var assets = AssetDatabaseUtils.FindAssetsByType<Material>();
        var matInfo = new string[assets.Count];

        for (var i = 0; i < assets.Count; ++i)
        {
            matInfo[i] = assets[i].name;
        }

        return matInfo;
    }

    private Enum DrawSuggestedSettingEnum(string friendlyName, Enum curValue, Enum suggestedValue)
    {
        EditorGUILayout.BeginHorizontal();
        var setValue = EditorGUILayout.EnumPopup(friendlyName, curValue, this.layoutColumnWidth);
        GUILayout.FlexibleSpace();
        bool isSuggestedOrBetter = setValue.CompareTo(suggestedValue) <= 0;

        EditorGUILayout.LabelField("Suggested Value: <= " + suggestedValue.ToString(),
                                   isSuggestedOrBetter ? this.settingSuggestedLabel : this.settingNotSuggestedLabel, this.layoutColumnWidth);

        if (!isSuggestedOrBetter && GUILayout.Button("Set to suggested", this.layoutButtonWidth))
        {
            setValue = suggestedValue;
        }

        EditorGUILayout.EndHorizontal();
        return setValue;
    }

    private int DrawSuggestedSettingInt(string friendlyName, int curValue, int suggestedValue, int min, int max)
    {
        EditorGUILayout.BeginHorizontal();
        var setValue = EditorGUILayout.IntSlider(friendlyName, curValue, min, max, this.layoutColumnWidth);
        GUILayout.FlexibleSpace();
        bool isSuggestedOrBetter = setValue.CompareTo(suggestedValue) <= 0;

        EditorGUILayout.LabelField("Suggested Value: <= " + suggestedValue.ToString(), 
                                   isSuggestedOrBetter ? this.settingSuggestedLabel : this.settingNotSuggestedLabel, this.layoutColumnWidth);

        if (!isSuggestedOrBetter && GUILayout.Button("Set to suggested", this.layoutButtonWidth))
        {
            setValue = suggestedValue;
        }

        EditorGUILayout.EndHorizontal();
        return setValue;
    }

    private bool DrawSuggestedSettingBool(string friendlyName, bool curValue, bool suggestedValue)
    {
        EditorGUILayout.BeginHorizontal();
        var setValue = EditorGUILayout.Toggle(friendlyName, curValue, this.layoutColumnWidth);
        GUILayout.FlexibleSpace();
        bool isSuggested = (setValue == suggestedValue);

        EditorGUILayout.LabelField("Suggested Value: " + suggestedValue.ToString(),
                                   isSuggested ? this.settingSuggestedLabel : this.settingNotSuggestedLabel, 
                                   this.layoutColumnWidth);

        if (!isSuggested && GUILayout.Button("Set to suggested", this.layoutButtonWidth))
        {
            setValue = suggestedValue;
        }

        EditorGUILayout.EndHorizontal();
        return setValue;
    }
}