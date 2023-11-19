using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtParticle : MonoBehaviour
{
    public Vector2 velocity;

    private void Start()
    {
        Destroy(gameObject, 3);
    }
    void Update()
    {
        velocity += Vector2.down * GeneralManager.instance.gravity * Time.deltaTime;
        transform.position = (Vector2)transform.position + velocity * Time.deltaTime;
    }
}
