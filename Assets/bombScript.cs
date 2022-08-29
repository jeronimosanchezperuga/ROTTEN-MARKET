﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombScript : MonoBehaviour
{
    private Transform Player;
    private Rigidbody rb;
    private Vector3 velocity = new Vector3(0,0,1);

    private float lifetime = 5;

    public GameObject explosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        transform.LookAt(Player);
        rb = GetComponent<Rigidbody>();
        velocity *= (Random.Range(6f, 12f));
        velocity += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        rb.AddForce(velocity, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0)
        {

            GameObject clon = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            Destroy(clon, 0.9f);
            Destroy(gameObject);

        }


    }
}
