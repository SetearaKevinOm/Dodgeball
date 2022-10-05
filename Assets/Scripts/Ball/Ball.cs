using System;
using System.Collections;
using System.Collections.Generic;
using Kevin;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kevin
{
    public class Ball : NetworkBehaviour
    {
        public int speed;
        public int maxSpeed;
        
        public Rigidbody rb;
        public float randomX;
        public float randomZ;

        public bool gameStarted = false;
        /*public void Start()
        {
            RollServerRpc();
        }*/

        /*public void OnEnable()
        {
            RollServerRpc();
        }*/

        public void FixedUpdate()
        {
            if (gameStarted && IsServer)
            {
                RollServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RollServerRpc()
        {
           
            rb = GetComponent<Rigidbody>();
            randomX = Random.Range(-10f, 10f);
            randomZ = Random.Range(-10f, 10f);
            //rb.AddForce(Vector3.forward,ForceMode.Impulse);
            rb.velocity = new Vector3(randomX, 0,randomZ).normalized* speed;
            gameStarted = false; 
        }
        
    }
}

