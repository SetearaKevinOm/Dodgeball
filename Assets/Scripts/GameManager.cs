using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kevin;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager singleton;
    public GameObject playerPrefab;
    public GameObject ballSpawner;
    public Transform spawnPoint;
    
    public event Action OnGameStart;
    public event Action OnGameEnd;

    public float timer;
    public int amountOfBalls = 0;
    public int maxAmountOfBalls;
    public int currentNozzleIndex;
    public int playersInGame;
    public AudioSource cannonSound;
    public AudioSource wallSound;
    public GameObject ballPrefab;

    public List<GameObject> cannonNozzles;
    public void Awake()
    {
        singleton = this;
        OnGameStart += GameStart;
        OnGameEnd += GameEnd;
    }

    private void Start()
    {
	    NetworkManager.Singleton.OnServerStarted += SubscribeToSceneEvent;
    }

    public void CreateNozzleList()
    {
	    foreach(Nozzle nozzle in FindObjectsOfType<Nozzle>())
	    {
		    cannonNozzles.Add(nozzle.gameObject);
	    }
    }

    public void OnStartGame()
    {
	    OnGameStart?.Invoke();
	    CreateNozzleList();
	    StartCoroutine(Pause());
    }

    private IEnumerator Pause()
    {
	    yield return new WaitForSeconds(3f);
	    SpawnABall();
    }
    
    #region BallSpawner
    
	    private void SpawnABall()
	    {
		    if (IsServer && amountOfBalls < maxAmountOfBalls)
		    {
			    currentNozzleIndex = UnityEngine.Random.Range(0, cannonNozzles.Count);
			    NetworkInstantiate(ballPrefab, cannonNozzles[currentNozzleIndex].transform.position, Quaternion.identity);
			    cannonSound.Play();
			    //GameObject go = Instantiate(ballPrefab, transform.position, Quaternion.identity);
			    //go.GetComponent<NetworkObject>().Spawn();
			    amountOfBalls++;
			    StartCoroutine(SpawnCooldown());
		    }
	    }
	
	    private IEnumerator SpawnCooldown()
	    {
		    yield return new WaitForSeconds(timer);
		    SpawnABall();
	    }
	    
	    #endregion
    public void GameStart()
    {
	    if (!IsServer) return;

	    for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
	    {
		    GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
		    NetworkObject no = player.GetComponent<NetworkObject>();
		    player.GetComponent<NetworkObject>()
				    .SpawnWithOwnership(NetworkManager.Singleton.ConnectedClientsList[i].ClientId);
		    NetworkObject clientEntity = NetworkManager.Singleton.ConnectedClients.ElementAt(i).Value.PlayerObject;
		    clientEntity.GetComponent<ClientEntity>().ControlledPlayer = player;
		    player.GetComponent<Player>().SetName(clientEntity.GetComponent<ClientInfo>().ClientName.Value.ToString());
	    }

	    playersInGame = NetworkManager.Singleton.ConnectedClients.Count; 
    }

    public void GameEnd()
    {
	    if (playersInGame == 0)
	    {
		    Debug.Log("No more players! The End!");
		    SceneManager.LoadScene("EndGameScene", LoadSceneMode.Single);
	    }
    }
    
    public GameObject NetworkInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
	    if (!IsServer) return null;
	    if (prefab.GetComponent<NetworkObject>() == null) return null;
	    GameObject go = Instantiate(prefab, position, rotation);
	    go.GetComponent<NetworkObject>().Spawn();
	    return go;
    }
    
    private void SubscribeToSceneEvent()
    {
	    //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetupScene;
		
		
	    //OLLIE HACK: Subscribed SetupScene to the LobbyUIManager instead of the above, commented out line
	    //Means SetupScene only occurs when the Lobby's Start Game button is pressed
	    //Allows Lobby to load scenes in, call their Perlin spawn so level preview can exist
	    //On Start Game, lobby unloads everything but Base scene, then loads the new scene FULLY
	    //then SetupScene runs
	    LobbyManager.instance.LobbyGameStartEvent += SetupScene;
    }
    
    private void SetupScene()
    {
	    if (!IsServer) return;

	    for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
	    {
		    Transform spawnPoint = null;
		    foreach (SpawnPoint spawnPoints in FindObjectsOfType<SpawnPoint>())
		    {
			    spawnPoint = spawnPoints.transform;
		    }
		    /*if (spawnTransform != null) // No Spawns found
		    {
			    GameObject avatar = Instantiate(avatarPrefab, spawnTransform.position, spawnTransform.rotation);
			    NetworkObject no = avatar.GetComponent<NetworkObject>();
			    avatar.GetComponent<NetworkObject>()
				    .SpawnWithOwnership(NetworkManager.Singleton.ConnectedClientsList[i].ClientId);
			    NetworkObject clientEntity = NetworkManager.Singleton.ConnectedClients.ElementAt(i).Value.PlayerObject;
			    clientEntity.GetComponent<ClientEntity>().ControlledPlayer = avatar;
			    avatar.GetComponent<Avatar>().SetName(clientEntity.GetComponent<ClientInfo>().ClientName
				    .Value.ToString());
		    }*/
		    if (spawnPoint != null) // No Spawns found
		    {
			    GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
			    NetworkObject no = player.GetComponent<NetworkObject>();
			    player.GetComponent<NetworkObject>()
				    .SpawnWithOwnership(NetworkManager.Singleton.ConnectedClientsList[i].ClientId);
			    NetworkObject clientEntity = NetworkManager.Singleton.ConnectedClients.ElementAt(i).Value.PlayerObject;
			    clientEntity.GetComponent<ClientEntity>().ControlledPlayer = player;
			    player.GetComponent<Player>().SetName(clientEntity.GetComponent<ClientInfo>().ClientName
				    .Value.ToString());
		    }
	    }
    }
    
   
}
