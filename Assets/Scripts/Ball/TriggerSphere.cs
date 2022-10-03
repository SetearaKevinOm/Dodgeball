using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Kevin
{
    public class TriggerSphere : MonoBehaviour
    {
        public Ball ball;
        public void Start()
        {
            ball = GetComponentInParent<Ball>();
        }

        public void OnTriggerEnter(Collider other)
        {
            Interfaces.IPlayer otherPlayer = other.GetComponent<Interfaces.IPlayer>();
            if (otherPlayer != null)
            {
                Debug.Log("Hit Player!");
                //other.gameObject.SetActive(false);
            }
            
            if (other.GameObject().layer == 3)
            {
                Debug.Log("Hit Wall!");
                if (ball.speed != ball.maxSpeed)
                {
                    ball.speed ++;
                }
                ball.gameStarted = true;
            }
            
        }
    }

}
