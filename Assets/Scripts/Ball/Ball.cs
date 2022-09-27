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
        public float speed;
        public Rigidbody rb;
        public float random;
        
        public void Start()
        {
            rb = GetComponent<Rigidbody>();
            random = Random.Range(0f, 10f);
            RollServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void RollServerRpc()
        {
            //rb.AddForce(new Vector3(random, 0,random).normalized* speed,ForceMode.Impulse);
            rb.velocity = new Vector3(random, 0,random).normalized* speed;
        }
        
    }
}

