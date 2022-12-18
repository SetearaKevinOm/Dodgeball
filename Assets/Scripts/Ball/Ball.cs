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

        public Vector3 currentVelocity;
        
        public bool gameStarted;
        /*public void Start()
        {
            RollServerRpc();
        }*/

        /*public void OnEnable()
        {
            RollServerRpc();
        }*/
        public void OnEnable()
        {
            if (gameStarted)
            {
                Roll();
            }
        }

        //[ServerRpc (RequireOwnership = false)]
        public void Roll()
        {
            //if (!IsServer) return;
            rb = GetComponent<Rigidbody>();
            randomX = Random.Range(-10f, 10f);
            randomZ = Random.Range(-10f, 10f);
            rb.AddForce(new Vector3(randomX, 0,randomZ).normalized* speed,ForceMode.Impulse);
            //rb.velocity = new Vector3(10, 0,0).normalized* speed;
            //rb.velocity = new Vector3(GameManager.singleton.cannonNozzles[GameManager.singleton.currentNozzleIndex].transform.localEulerAngles.x, 0, 0).normalized * speed;
            gameStarted = false; 
        }

        public void Update()
        {
            if (IsServer)
            {
                currentVelocity = rb.velocity;
                StartCoroutine(UpdateClientVelocity());
            }
        }

        private IEnumerator UpdateClientVelocity()
        {
            yield return new WaitForSeconds(0.5f);
            UpdateVelocityClientRpc();
        }
        
        [ClientRpc]
        private void UpdateVelocityClientRpc()
        {
            if (IsClient)
            {
                rb.velocity = currentVelocity;
            }
        }
    }
}

