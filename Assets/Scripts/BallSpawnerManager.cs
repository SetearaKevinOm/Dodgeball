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

    public void OnAwake()
    {
        StartCoroutine(GameStarted());
    }

    private IEnumerator GameStarted()
    {
        if (IsServer)
        {
            yield return new WaitForSeconds(2f);
            SpawnABallServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnABallServerRpc()
    {
        if (amountOfBalls < maxAmountOfBalls)
        {
            Instantiate(ballPrefab, transform.position, Quaternion.identity);
            amountOfBalls++;
            StartCoroutine(SpawnCooldown());
        }
    }

    [ServerRpc]
    public void SpawnAnotherBallServerRpc()
    {
        Instantiate(ballPrefab, transform.position, Quaternion.identity);
        amountOfBalls++;
        if (amountOfBalls < maxAmountOfBalls)
        {
            StartCoroutine(SpawnCooldown());
        }
    }
    
    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(timer);
        SpawnAnotherBallServerRpc();
    }

    
}
