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
            if (instance == null)
            {
                // first time init
                var sceneInstance = GameObject.FindObjectOfType<T>();
                if (sceneInstance)
                {
                    sceneInstance.Initialize();
                    instance = sceneInstance;
                }
            }

            return instance;
        }
    }

    protected void Initialize()
    {
        lock (initializedLock)
        {
            if (!initialized)
            {
                initialized = true;
                InitializeInternal();
            }
        }
    }

    protected abstract void InitializeInternal();
}