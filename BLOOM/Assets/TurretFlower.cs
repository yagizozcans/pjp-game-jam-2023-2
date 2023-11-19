using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFlower : MonoBehaviour
{
    public float radius;

    public LayerMask layer;

    public GameObject closestEnemy;
    float oldclosestDistance;

    public float attackTimer;
    float timer;

    public Sprite thorn;

    private void Start()
    {
        StartCoroutine(startFunction());
    }
    private void Update()
    {
        RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, radius, Vector3.forward,5,layer);
        timer += Time.deltaTime;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (closestEnemy == null)
            {
                closestEnemy = enemies[i].transform.gameObject;
            }
            float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - enemies[i].transform.position.x, 2) + Mathf.Pow(transform.position.y - enemies[i].transform.position.y, 2));
            oldclosestDistance = Mathf.Sqrt(Mathf.Pow(transform.position.x - closestEnemy.transform.position.x, 2) + Mathf.Pow(transform.position.y - closestEnemy.transform.position.y, 2));
            if (distance < oldclosestDistance)
            {
                closestEnemy = enemies[i].transform.gameObject;
                oldclosestDistance = distance;
            }
        }
        if(closestEnemy != null)
        {
            if(timer > attackTimer)
            {
                Attack();
                timer = 0;
            }
        }
    }
    IEnumerator startFunction()
    {
        yield return new WaitForEndOfFrame();
        GeneralManager.instance.DrawLR(gameObject.GetComponentInChildren<LineRenderer>(), radius, 16, 0.1f);
    }

    public void Attack()
    {
        GameObject thornObj = new GameObject();
        thornObj.AddComponent<SpriteRenderer>();
        thornObj.GetComponent<SpriteRenderer>().sprite = thorn;
        thornObj.AddComponent<Bullet>();
        thornObj.GetComponent<Bullet>().movementSpeed = 12f;
        thornObj.AddComponent<BoxCollider2D>();
        thornObj.transform.localScale = Vector2.one * 2;
        thornObj.transform.position = transform.position;
        thornObj.layer = 8;
        thornObj.tag = "bullet";
        float angle = Mathf.Atan2(transform.position.y - closestEnemy.transform.position.y, transform.position.x - closestEnemy.transform.position.x) * Mathf.Rad2Deg + 90;
        thornObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
