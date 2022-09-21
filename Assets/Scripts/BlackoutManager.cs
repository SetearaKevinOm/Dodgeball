using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackoutManager : NetworkBehaviour
{
   public float timer = 10f;
   public GameObject planeObject;
   public bool state;
   public Renderer renderer;

   public void Awake()
   {
      renderer = GetComponent<Renderer>();
   }

   public void Update()
   {
      if (Input.GetKeyDown(KeyCode.Space))
      {
         KeyPressedServerRpc();
      }
   }
   public void FixedUpdate()
   {
      if (Random.Range(0, 120) == 0)
      {
         state = !state;
         BlackOutClientRpc(state);
      }
   }

   #region Server

   [ServerRpc(RequireOwnership = false)]
   private void KeyPressedServerRpc()
   {
      KeyPressedClientRpc();
   }

   #endregion
   
   #region Client

   [ClientRpc]
   private void BlackOutClientRpc(bool status)
   {
      state = status;
      FadeOutClientRpc();
   }

   [ClientRpc]
   private void FadeOutClientRpc()
   {
      if (state)
      {
         renderer.material.SetFloat("_Alpha",0f);
      }
      else if (state == false)
      {
         renderer.material.SetFloat("_Alpha",1f);
      }
   }
   
   [ClientRpc]
   private void KeyPressedClientRpc()
   {
      Debug.Log("Key Pressed!");
   }

   #endregion
}
