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
    
    public void OnEnable()
    {
        //SpawnABall();
        //GameManager.singleton.OnGameStart += GameStarted;
        //StartCoroutine(GameStarted());
        GameStarted();
    }

    private void GameStarted()
    {
        if (IsServer || IsHost)
        {
            SpawnABall();
        }
    }

    
    private void SpawnABall()
    {
        if (IsServer || IsHost && amountOfBalls < maxAmountOfBalls)
        {
            GameManager.singleton.NetworkInstantiate(ballPrefab, transform.position, Quaternion.identity);
            //GameObject go = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            //go.GetComponent<NetworkObject>().Spawn();
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
