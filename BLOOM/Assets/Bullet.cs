using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float movementSpeed;
    private void Update()
    {
        transform.position += transform.up * movementSpeed * Time.deltaTime;
    }
}
