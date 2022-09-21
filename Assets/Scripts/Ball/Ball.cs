using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;
    public float random;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        random = Random.Range(0f, 100f);
    }

    void Update()
    {
        rb.velocity = new Vector3(random, 0,random).normalized* speed;
    }
}
