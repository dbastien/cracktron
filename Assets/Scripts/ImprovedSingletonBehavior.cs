using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class ImprovedSingletonBehavior<T> : MonoBehaviour where T : ImprovedSingletonBehavior<T>
{
    private bool initialized;
    private object initializedLock = new object();

    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Necessary for pattern")]
    private static T instance;
    public static T Instance
    {
        get
        {
            if (ImprovedSingletonBehavior<T>.instance == null)
            {
                // first time init
                var sceneInstance = Object.FindObjectOfType<T>();
                if (sceneInstance)
                {
                    sceneInstance.Initialize();
                    ImprovedSingletonBehavior<T>.instance = sceneInstance;
                }
            }

            return ImprovedSingletonBehavior<T>.instance;
        }
    }

    protected void Initialize()
    {
        lock (this.initializedLock)
        {
            if (!this.initialized)
            {
                this.initialized = true;
                this.InitializeInternal();
            }
        }
    }

    protected abstract void InitializeInternal();
}