using UnityEditor;
using UnityEngine;

public static class SerializedPropertyExtensions
{
    public static void LogPathWithChildren(this SerializedProperty prop)
    {
        var rootLen = prop.propertyPath.Length;
        var names = prop.propertyPath;
        var propCopy = prop.Copy();

        int level = 0;

        //TODO: log each level to its own line
        while (propCopy.Next(true))
        {
            var levelOld = level;
            level = prop.propertyPath.Split('.').Length;
            if (level != levelOld && !string.IsNullOrEmpty(names))
            {
                Debug.Log(names);
                names = "";
            }
            if (propCopy.propertyPath.Length > rootLen)
            {
                names += propCopy.propertyPath.Substring(rootLen, propCopy.propertyPath.Length - rootLen) + " ";
            }
        }
        if (!string.IsNullOrEmpty(names))
        {
            Debug.Log(names);
        }
    }
}