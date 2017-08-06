using System;
using UnityEngine;

public abstract class Neuron : ScriptableObject
{
    protected GameObject owner;

    public virtual void Update()
    {
    }

    public virtual void OnEnable()
    {
        //don't break out as separate sub-items in the asset pane
        this.hideFlags = HideFlags.HideInHierarchy;
    }

    public virtual void Attach(GameObject owner)
    {
        this.owner = owner;
    }
}