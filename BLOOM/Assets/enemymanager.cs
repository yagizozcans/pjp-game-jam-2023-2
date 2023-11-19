using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemymanager : MonoBehaviour
{
    public float radius;
    public GameObject bird;
    public GameObject playerflower;

    float timer = 0;
    public float maxTimer;
    public void InstantiateEnemy()
    {
        float randomAngle = Random.Range(0, 360f);
        Instantiate(bird,(Vector2)playerflower.transform.position + new Vector2(radius*Mathf.Cos(randomAngle * Mathf.PI / 180), radius * Mathf.Sin(randomAngle * Mathf.PI / 180)), Quaternion.identity);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > maxTimer)
        {
            InstantiateEnemy();
            maxTimer = Mathf.Clamp(maxTimer - 0.2f,0.2f,maxTimer);
            timer = 0;
        }
    }
}
