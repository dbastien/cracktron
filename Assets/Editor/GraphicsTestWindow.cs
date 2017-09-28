using UnityEditor;
using UnityEngine;

public class GraphicsTestWindow : EditorWindow
{
    [SerializeField] public GUILayoutOption[] layoutColumnWidth = new GUILayoutOption[] { GUILayout.Width(325f) };
    [SerializeField] public GUILayoutOption[] layoutButtonWidth = new GUILayoutOption[] { GUILayout.Width(325f) };

    [MenuItem("Cracktron/Show Graphics Test Window")]
    static void ShowWindow()
    {
        var window = GetWindow(typeof(GraphicsTestWindow));
        window.name = "Graphics Test";

        window.Show();
    }

    public void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
    }

    void OnGUI()
    {
        var buildTarget = BuildTargetGroup.Standalone;// EditorUserBuildSettings.selectedBuildTargetGroup;
        //editor defaults to tier 3 - doesn't seem to be a way to get programatically
        var tier = UnityEngine.Rendering.GraphicsTier.Tier3;      
        var tierSettings = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(buildTarget, tier);

        RenderingPathRestricted renderingPath = RenderingPathRestricted.Forward;
        if (tierSettings.renderingPath != RenderingPath.UsePlayerSettings)
        {
            renderingPath = (RenderingPathRestricted)tierSettings.renderingPath;
        }

        EditorGUI.BeginChangeCheck();
        //should not include useplayersettings
        renderingPath = (RenderingPathRestricted)EditorGUILayout.EnumPopup(Styles.renderingPath, renderingPath, new GUILayoutOption[0]);
        if (EditorGUI.EndChangeCheck())
        {
            tierSettings.renderingPath = (RenderingPath)renderingPath;
            UnityEditor.Rendering.EditorGraphicsSettings.SetTierSettings(buildTarget, tier, tierSettings);
        }

        //RenderSettings.fog
        EditorGUI.BeginChangeCheck();
        RenderSettings.fog = EditorGUILayout.Toggle(Styles.fog, RenderSettings.fog, new GUILayoutOption[0]);
        if (EditorGUI.EndChangeCheck() && RenderSettings.fog)
        {
            RenderSettings.fogColor = Color.yellow;
            RenderSettings.fogDensity = 0.08f;
            RenderSettings.fogStartDistance = 0.5f;
            RenderSettings.fogEndDistance = 10.0f;
            RenderSettings.fogMode = FogMode.Exponential;
        }

        //lightmapping
        EditorGUI.BeginChangeCheck();
        Lightmapping.bakedGI = EditorGUILayout.Toggle(Styles.lightmapping, Lightmapping.bakedGI, new GUILayoutOption[0]);
        if (EditorGUI.EndChangeCheck())
        {
            var lights = GameObject.FindObjectsOfType<Light>();
            foreach (var light in lights)
            {
#if UNITY_5 && !UNITY_5_6
                light.lightmappingMode = Lightmapping.bakedGI ? LightmappingMode.Baked : LightmappingMode.Realtime;
#else
                light.lightmapBakeType = Lightmapping.bakedGI ? LightmapBakeType.Baked : LightmapBakeType.Realtime;
#endif
            }

            var renderers = GameObject.FindObjectsOfType<Renderer>();
            foreach (var renderer in renderers)
            {
                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, Lightmapping.bakedGI ? StaticEditorFlags.LightmapStatic : 0);

                if (!Lightmapping.bakedGI)
                {
                    renderer.lightmapIndex = -1;
                }
            }

            if (Lightmapping.bakedGI == true)
            {
                LightmapSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
                Lightmapping.BakeAsync();
            }
            else
            {
                Lightmapping.Clear();
            }
        }

        //visual quality and perf comparison with standard and other unity shaders

        //migration
    }

    protected static class Styles
    {
        public static GUIContent renderingPath = new GUIContent("Rendering Path", "How Unity's rendering engine operates");
        public static GUIContent fog = new GUIContent("Fog", "Toggles fog");
        public static GUIContent lightmapping = new GUIContent("Lightmapping", "Toggles lightmapping");
    }

    protected enum RenderingPathRestricted
    {
        //no use player settings which is -1
        VertexLit,
        Forward,
        DeferredLighting,
        DeferredShading
    };
}
