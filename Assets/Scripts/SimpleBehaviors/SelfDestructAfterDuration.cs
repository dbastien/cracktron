using UnityEngine;

public class SelfDestructAfterDuration : MonoBehaviour
{
    public float Duration;

    void Start()
    {
        Destroy(this.gameObject, this.Duration);
    }
}
