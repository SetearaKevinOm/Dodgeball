using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackoutManager : NetworkBehaviour
{
   public float timer;
   public GameObject planeObject;
   public bool state;
   public Renderer renderer;
   public bool time;
   public void Awake()
   {
      renderer = GetComponent<Renderer>();
      renderer.material.SetFloat("_Alpha",0f);
      StartCoroutine(BlackoutDelay());
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
      if(time)
      {
         BlackOutClientRpc();
      }
   }

   IEnumerator BlackoutDelay()
   {
      yield return new WaitForSeconds(timer);
      time = true;
   }
   
   IEnumerator ClearScreen()
   {
      yield return new WaitForSeconds(1f);
      ClearScreenClientRpc();
   }
   
   #region Client

   [ClientRpc]
   private void BlackOutClientRpc()
   {
      //renderer.material.SetFloat("_Alpha",Mathf.Lerp(0f,1f,0.1f));
      /*for (float f = 1; f < 0; f -= 0.1f)
      {
         renderer.material.SetFloat("_Alpha",f);
         StartCoroutine(OneSec());
      }*/
      renderer.material.SetFloat("_Alpha", 1f);
      StartCoroutine(ClearScreen());
   }

   IEnumerator WaitForSec()
   {
      yield return new WaitForSeconds(5f);
   }
   
   [ClientRpc]
   private void ClearScreenClientRpc() 
   {
      time = false;
      //renderer.material.SetFloat("_Alpha",Mathf.Lerp(1f,0f,0.1f));
      renderer.material.SetFloat("_Alpha",0f);
      StartCoroutine(BlackoutDelay());
   }

   [ClientRpc]
   private void KeyPressedClientRpc()
   {
      Debug.Log("Key Pressed!");
   }

   #endregion
   
   #region Server

   [ServerRpc(RequireOwnership = false)]
   private void KeyPressedServerRpc()
   {
      KeyPressedClientRpc();
   }

   #endregion
}
