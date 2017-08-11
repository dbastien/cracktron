using System;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class ComponentMemberReference
{
    [SerializeField] private Component targetComponent;
    [SerializeField] private string targetMemberName;

    private FieldInfo fieldInfo;
    private PropertyInfo propertyInfo;

    public object GetValue()
    {
        this.CacheInfo();

        if (this.fieldInfo != null)
        {
            return this.fieldInfo.GetValue(this.targetComponent);
        }

        return this.propertyInfo.GetValue(this.targetComponent);
    }

    public void SetValue(object value)
    {
        this.CacheInfo();

        if (this.fieldInfo != null)
        {
            this.fieldInfo.SetValue(this.targetComponent, value);
        }
        else
        {
            this.propertyInfo.SetValue(this.targetComponent, value);
        }
    }

    private void CacheInfo()
    {
        if (this.fieldInfo == null)
        {
            return;
        }

        var type = this.targetComponent.GetType();

        this.fieldInfo = type.GetField(this.targetMemberName);
        this.propertyInfo = type.GetProperty(this.targetMemberName);
    }
}