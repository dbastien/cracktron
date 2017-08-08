using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorReflectionUtils
{
    private static PropertyInfo cachedInspectorModeInfo;

    public static bool IsScriptProperty(SerializedProperty property)
    {
        return property.name.Equals("m_Script") &&
               property.type.Equals("PPtr<MonoScript>") &&
               property.propertyType == SerializedPropertyType.ObjectReference &&
               property.propertyPath.Equals("m_Script");
    }

    public static long GetLocalIdentifierInFileForObject(Object unityObject)
    {
        if (unityObject == null)
        {
            return -1;
        }

        var serializedObject = new SerializedObject(unityObject);

        if (EditorReflectionUtils.cachedInspectorModeInfo == null)
        {
            EditorReflectionUtils.cachedInspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        EditorReflectionUtils.cachedInspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
        var serializedProperty = serializedObject.FindProperty("m_LocalIdentfierInFile");
        var id = serializedProperty.longValue;

        if (id <= 0)
        {
            var prefabType = PrefabUtility.GetPrefabType(unityObject);
            if (prefabType != PrefabType.None)
            {
                id = EditorReflectionUtils.GetLocalIdentifierInFileForObject(PrefabUtility.GetPrefabParent(unityObject));
            }
            else
            {
                // this will work for the new objects in scene which weren't saved yet
                id = unityObject.GetInstanceID();
            }
        }

        if (id <= 0)
        {
            var go = unityObject as GameObject;
            if (go != null)
            {
                id = go.transform.GetSiblingIndex();
            }
        }

        return id;    
    }
}
