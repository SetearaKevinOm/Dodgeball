using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
public class MainMenu : NetworkBehaviour
{
    public void StartGame()
    {
        Debug.Log("I clicked");
        UnityEngine.SceneManagement.SceneManager.LoadScene("NetworkManagerScene");
        //NetworkSceneManager.SceneManager.LoadScene("NetworkManagerScene", LoadSceneMode.Additive);
    }
}
