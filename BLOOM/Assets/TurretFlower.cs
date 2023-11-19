using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFlower : MonoBehaviour
{
    public float radius;


    private void Start()
    {
        StartCoroutine(startFunction());
    }
    IEnumerator startFunction()
    {
        yield return new WaitForEndOfFrame();
        GeneralManager.instance.DrawLR(gameObject.GetComponentInChildren<LineRenderer>(), radius, 16, 0.1f);
    }
}
