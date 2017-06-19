using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public BrainData BrainData;

    private Rigidbody ownerRigidbody;

    public float MovementSpeed = 6f;

    void Start()
    {
        ownerRigidbody = GetComponent<Rigidbody>();

        BrainData = Object.Instantiate(BrainData);
        BrainData.CloneNeurons();

        foreach (var neuron in BrainData.SteerNeurons)
        {
            neuron.Attach(gameObject);
        }
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;

        foreach (var neuron in BrainData.SteerNeurons)
        {
            neuron.Update();

            direction += neuron.Direction * neuron.Weight;
        }

        direction.Normalize();

        gameObject.transform.forward = direction;

        ownerRigidbody.MovePosition(gameObject.transform.position + direction * MovementSpeed * Time.deltaTime);
    }
}