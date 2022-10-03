using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallSpawnerManager : NetworkBehaviour
{
    public float timer;
    public int amountOfBalls;
    public int maxAmountOfBalls;

    public GameObject ballPrefab;
    
    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnABallServerRpc();
    }

    [ServerRpc]
    private void SpawnABallServerRpc()
    {
        if (amountOfBalls < maxAmountOfBalls)
        {
            Instantiate(ballPrefab, transform.position, Quaternion.identity);
            amountOfBalls++;
            StartCoroutine(SpawnBall());
        }
    }

    [ServerRpc]
    public void SpawnAnotherBallServerRpc()
    {
        Instantiate(ballPrefab, transform.position, Quaternion.identity);
        amountOfBalls++;
        if (amountOfBalls < maxAmountOfBalls)
        {
            StartCoroutine(SpawnBall());
        }
    }
    
    private IEnumerator SpawnBall()
    {
        yield return new WaitForSeconds(timer);
        SpawnAnotherBallServerRpc();
    }

    
}
