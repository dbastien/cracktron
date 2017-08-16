using UnityEngine;

public class SimpleThirdPersonController : MonoBehaviour 
{
    public float MovementSpeed = 1.0f;
    public float TurnSpeed = 40.0f;

    public void Update() 
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        this.transform.Rotate(this.transform.up, h * Time.deltaTime * this.TurnSpeed);

        this.transform.position += this.transform.forward * v * Time.deltaTime * this.MovementSpeed;
    }
}
