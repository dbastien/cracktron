using UnityEngine;

public class Brain : MonoBehaviour
{
    public BrainData BrainData;

    private Rigidbody ownerRigidbody;

    public float MovementSpeed = 6f;

    public void Start()
    {
        ownerRigidbody = GetComponent<Rigidbody>();

        this.BrainData = Object.Instantiate(this.BrainData);
        this.BrainData.CloneNeurons();

        foreach (var neuron in this.BrainData.SteerNeurons)
        {
            neuron.Attach(gameObject);
        }
    }

    public void Update()
    {
        var direction = Vector3.zero;

        foreach (var neuron in this.BrainData.SteerNeurons)
        {
            neuron.Update();

            direction += neuron.Direction * neuron.Weight;
        }

        direction.Normalize();

        gameObject.transform.forward = direction;

        ownerRigidbody.MovePosition(gameObject.transform.position + direction * this.MovementSpeed * Time.deltaTime);
    }
}