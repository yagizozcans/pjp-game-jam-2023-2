using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public float turnSpeed;
    public float movementSpeed;

    public Transform mainFlower;

    private void Start()
    {
        mainFlower = GameObject.FindGameObjectWithTag("main").transform;
    }
    public void Update()
    {
        transform.position += transform.up * Time.deltaTime * movementSpeed;
        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, (mainFlower.position - transform.position).normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "main")
        {
            collision.GetComponent<PlayerFlower>().life--;
            Destroy(gameObject);
        }
        if (collision.tag == "bullet")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
