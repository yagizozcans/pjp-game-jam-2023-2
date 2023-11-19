using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public float turnSpeed;
    public float movementSpeed;

    public Transform mainFlower;

    public void Update()
    {
        transform.position += transform.up * Time.deltaTime * movementSpeed;
        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, (mainFlower.position - transform.position).normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
    }
}
