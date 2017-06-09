using UnityEngine;

public class SelfDestructOnCollisionTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
