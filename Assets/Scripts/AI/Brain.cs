﻿using UnityEngine;

public class Brain : MonoBehaviour
{
    public BrainData BrainData;

    private Rigidbody ownerRigidbody;

    public float MovementSpeed = 6f;

    public void Start()
    {
        this.ownerRigidbody = this.GetComponent<Rigidbody>();

        this.BrainData = Instantiate(this.BrainData);
        this.BrainData.CloneNeurons();

        foreach (var neuron in this.BrainData.SteerNeurons)
        {
            neuron.Attach(this.gameObject);
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

        this.gameObject.transform.forward = direction;

        this.ownerRigidbody.MovePosition(this.gameObject.transform.position + direction * this.MovementSpeed * Time.deltaTime);
    }
}