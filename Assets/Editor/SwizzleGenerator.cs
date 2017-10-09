using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SwizzleGenerator : EditorWindow
{
    private string codeText;
    private Vector2 scrollPosition;

    private int popupIndex = 0;

    private static readonly List<string> vector2Props = new List<string>{"x", "y"};
    private static readonly List<string> vector3Props = new List<string>{"x", "y", "z"};
    private static readonly List<string> vector4Props = new List<string>{"x", "y", "z", "w"};

    [MenuItem("Cracktron/Swizzle Generator Window")]
    static void Init()
    {
        var window = GetWindow<SwizzleGenerator>();
        window.minSize = new Vector2(200f, 100f);
        window.maxSize = new Vector2(200f, 100f);
        window.titleContent = new GUIContent("Swizzle");
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.Space();

        popupIndex = EditorGUILayout.Popup(
            popupIndex, 
            new string[] {"Vector2", "Vector3", "Vector4"},
            new GUILayoutOption[0]
            );

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate to clipboard"))
        {
            var codeText = string.Empty;

            if (popupIndex == 0)
            {
                codeText = GenerateSwizzleCodeVector2();
            }
            else if (popupIndex == 1)
            {
                codeText = GenerateSwizzleCodeVector3();
            }
            else
            {
                codeText = GenerateSwizzleCodeVector4();
            }

            EditorGUIUtility.systemCopyBuffer = codeText;

            EditorUtility.DisplayDialog("Complete", "Copied to clipboard.", "OK");
        }
    }

    private string GenerateSwizzleCodeVector2()
    {
        string result = string.Empty;

        result += GenerateSwizzleCode("Vector2", "Vector2", vector2Props, 2);
        result += "\n\n";
        result += GenerateSwizzleCode("Vector3", "Vector2", vector2Props, 3);
        result += "\n\n";
        result += GenerateSwizzleCode("Vector4", "Vector2", vector2Props, 4);

        return result;
    }

    private string GenerateSwizzleCodeVector3()
    {
        string result = string.Empty;

        result += GenerateSwizzleCode("Vector2", "Vector3", vector3Props, 2);
        result += "\n\n";
        result += GenerateSwizzleCode("Vector3", "Vector3", vector3Props, 3);
        result += "\n\n";
        result += GenerateSwizzleCode("Vector4", "Vector3", vector3Props, 4);

        return result;
    }

    private string GenerateSwizzleCodeVector4()
    {
        string result = string.Empty;

        result += GenerateSwizzleCode("Vector2", "Vector4", vector4Props, 2);
        result += "\n\n";
        result += GenerateSwizzleCode("Vector3", "Vector4", vector4Props, 3);
        result += "\n\n";
        result += GenerateSwizzleCode("Vector4", "Vector4", vector4Props, 4);

        return result;
    }

    private string GenerateSwizzleCode
    (
        string classSource,
        string classDest,
        List<string> components,
        int k
    )
    {
        int n = components.Count;

        var permutations = Combinatorics.PermutationWithDuplication(components.Take(n).ToList(), k);

        string result = string.Empty;

        foreach (var p in permutations)
        {
            var swizzleCombination = string.Empty;
            var vecProperties = string.Empty;
            
            for (var i = 0; i < p.Count; ++i)
            {
                var q = p[i];

                swizzleCombination += q;
                vecProperties += (q == "0" || q == "1") ? q : "v." + q;
                if (i != p.Count -1)
                {
                    vecProperties += ", ";
                }
            }          

            result += string.Format("public static {0} {2}(this {1} v) {{return new {0}({3});}}\n",
						            classSource,
                                    classDest,
						            swizzleCombination,
						            vecProperties);
        }

        return result;
    }
}