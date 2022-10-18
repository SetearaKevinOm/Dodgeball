using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallSpawnerManager : NetworkBehaviour
{
    public float timer;
    public int amountOfBalls = 0;
    public int maxAmountOfBalls;

    public GameObject ballPrefab;

    //Network Manager
    private NetworkManager _networkManager; 
    public void Awake()
    {
        _networkManager = NetworkManager.Singleton;
        _networkManager.OnServerStarted += GameStarted;
        //StartCoroutine(GameStarted());
    }

    private void GameStarted()
    {
        if (IsServer)
        {
            SpawnABall();
        }
    }

    
    private void SpawnABall()
    {
        if (IsServer && amountOfBalls < maxAmountOfBalls)
        {
            GameObject go = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
            amountOfBalls++;
            StartCoroutine(SpawnCooldown());
        }
    }

    
    /*public void SpawnAnotherBall()
    {
        Instantiate(ballPrefab, transform.position, Quaternion.identity);
        amountOfBalls++;
        if (amountOfBalls < maxAmountOfBalls)
        {
            StartCoroutine(SpawnCooldown());
        }
    }*/
    
    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(timer);
        SpawnABall();
    }

    
}
