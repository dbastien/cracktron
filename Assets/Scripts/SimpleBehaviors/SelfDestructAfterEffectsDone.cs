using UnityEngine;

public class SelfDestructAfterEffectsDone : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    private AudioSource[] audioSources;

    void Awake()
    {
        this.particleSystems = GetComponents<ParticleSystem>();
        this.audioSources = GetComponents<AudioSource>();
    }

    void Update()
    {
        for (var i = 0; i < particleSystems.Length; ++i)
        {
            if (this.particleSystems[i].IsAlive())
            {
                return;
            }
        }

        for (var j = 0; j < audioSources.Length; ++j)
        {
            if (this.audioSources[j].isPlaying)
            {
                return;
            }
        }

        Destroy(this.gameObject);
    }
}
