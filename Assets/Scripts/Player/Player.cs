using System;
using System.Collections;
using System.Collections.Generic;
using Kevin;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Kevin
{
    public class Player : MonoBehaviour, Interfaces.IPlayer
    {
        public TMP_Text nameText;
        
        public void SetName(string name)
        {
            nameText.text = name;
            SetNameClientRpc(nameText.text);
        }
    
        [ClientRpc]
        public void SetNameClientRpc(string name)
        {
            nameText.text = name;
        }
        
        /*public GameObject playerPrefab;

        public void Awake()
        {
            playerPrefab = this.GetComponent<GameObject>(); 
        }
        private void OnTriggerEnter(Collider other)
        {
            Interfaces.IBall ball = other.GetComponent<Interfaces.IBall>();
            if (ball != null)
            {
                playerPrefab.SetActive(false);
            }
        }*/
    } 
}

