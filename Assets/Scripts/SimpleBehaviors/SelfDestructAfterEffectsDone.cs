using UnityEngine;

public class SelfDestructAfterEffectsDone : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    private AudioSource[] audioSources;

    void Awake()
    {
        particleSystems = GetComponents<ParticleSystem>();
        audioSources = GetComponents<AudioSource>();
    }

    void Update()
    {
        for (int i = 0; i < particleSystems.Length; ++i)
        {
            if (particleSystems[i].IsAlive())
            {
                return;
            }
        }

        for (int j = 0; j < audioSources.Length; ++j)
        {
            if (audioSources[j].isPlaying)
            {
                return;
            }
        }

        Destroy(this.gameObject);
    }
}
