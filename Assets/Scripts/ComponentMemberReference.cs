using System;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class ComponentMemberReference
{
    [SerializeField] private Component targetComponent;
    [SerializeField] private string targetFieldName;

    private FieldInfo fieldInfo;

    public object GetValue()
    {
        this.CacheInfo();

        return this.fieldInfo.GetValue(this.targetComponent);
    }

    public void SetValue(object value)
    {
        this.CacheInfo();

        this.fieldInfo.SetValue(this.targetComponent, value);
    }

    private void CacheInfo()
    {
        if (this.fieldInfo == null)
        {
            return;
        }

        var type = this.targetComponent.GetType();

        this.fieldInfo = type.GetField(this.targetFieldName);
    }
}