using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Unity.Netcode.Transports.UTP;

[Serializable]
public class Level
{
    public Object level;
    public string levelNameOnUI;
}

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;

    ulong myLocalClientId;
    NetworkObject myLocalClient;
    string clientName;

    [Header("Level Manager")]
    public List<Level> levels;
    public GameObject levelPrefab;
    
    
    [Header("IP Scene")] 
   
    
    public GameObject ipCanvas;
    public GameObject lobbyCanvas;
    public GameObject startButton;
    public GameObject selectLevelGameObject;
    public TMP_InputField ipInputField;

    [Header("Lobby Scene")] 
    
    public GameObject clientLobbyNameUIPrefab;

    public GameObject firstCamera;
    public GameObject playerPanel;
    public TMP_InputField playerNameInputField;


    [Header("Level Selecter")] 
    
    public string selectedLevel;
    public string sceneToLoad;
    public Transform selectLevelPanelTransform;
    public TMP_Text selectedLevelText;
    
    
    public event Action LobbyGameStartEvent;
   
    public IEnumerator LobbyGameStartDelayCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        LobbyGameStartEvent?.Invoke();
    }
    public void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientJoin;
        ipInputField.text = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
    }
    
    #region IPScene

    public void HostButton()
    {
        if (ipInputField.text == "")
        {
            Debug.Log("IP address not entered!!!");
        }
        else
        {
            ipCanvas.SetActive(false);
            lobbyCanvas.SetActive(true);
            NetworkManager.Singleton.StartHost();
            foreach (Level level in levels)
            {
                GameObject levelObject = Instantiate(levelPrefab, selectLevelPanelTransform);
                levelObject.GetComponentInChildren<TMP_Text>().text = level.levelNameOnUI;
                levelObject.GetComponent<LevelButton>().currentLevel = level.level.name;
            }

            //Debug.Log(NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address);
        }
    }

    public void JoinButton()
    {
        if (ipInputField.text == "")
        {
            Debug.Log("IP address not entered!!!");
        }
        else
        {
            Debug.Log(NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address);
            ClientLobby();
            NetworkManager.Singleton.StartClient();
            //Debug.Log(NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address);
        }
    }
    
    void ClientLobby()
    {
        ipCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
        startButton.gameObject.SetActive(false);
        selectLevelGameObject.SetActive(false);
    }
    
    public void OnIpEntered()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ipInputField.text;
    }
    #endregion


    #region LobbyScene

    /*On the server you use NetworkDiscovery.StartAsServer() to broadcasting messages to clients. On the client, you 
        listen to the broadcast by calling NetworkDiscovery.StartAsClient() then implementing OnReceivedBroadcast(string fromAddress, string data) function 
        which is called when it finds a server. You can then use fromAddress IP value to connect to the server.*/
    public void OnStartButton()
    {
        if (selectedLevel == "")
        {
            Debug.Log("You must select a level!!!");
            return;
        }
        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManagerOnOnSceneEvent;
        
        try
        {
            NetworkManager.Singleton.SceneManager.LoadScene(selectedLevelText.text, LoadSceneMode.Additive);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
        firstCamera.SetActive(false);
        GameManager.singleton.OnStartGame();
    }
    
    private void SceneManagerOnOnSceneEvent(SceneEvent sceneEvent) 
    {
        BroadcastLobbyUIStateClientRpc(true);
    }

    [ClientRpc]
    private void BroadcastLobbyUIStateClientRpc(bool gameInProgress)
    {
        lobbyCanvas.SetActive(!gameInProgress);
    }
    
    private void OnClientJoin(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer || IsOwner)
        {
            NetworkClient client;
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out client))
            {
                ClientInfo clientInfo = client.PlayerObject.GetComponent<ClientInfo>();
                clientInfo.Init((ulong) NetworkManager.Singleton.ConnectedClients.Count);

                GameObject uiRef = Instantiate(clientLobbyNameUIPrefab, playerPanel.transform);
                clientInfo.lobbyUIRef = uiRef;
                uiRef.GetComponent<TMP_Text>().text = clientInfo.ClientName.Value.ToString();
            }
            HandleLocalClient(clientId);
        }
        //else RequestClientNamesLobbyUIServerRpc(clientId);
        else RequestClientUIUpdateServerRpc();

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            myLocalClientId = clientId;
        }

    }
    
    void HandleLocalClient(ulong clientId)
    {
        NetworkClient temporaryClient;
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out temporaryClient))
        {
            NetworkObject playerObject = temporaryClient.PlayerObject;
            if (playerObject.IsOwner)
            {
                myLocalClient = playerObject;
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    
    public void RequestClientUIUpdateServerRpc()
    {
        HandleClientNameChange();
    }

    private void HandleClientNameChange()
    {
        ClearLobbyNamesClientRpc();

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnClientLobbyUIClientRpc(client.PlayerObject.GetComponent<ClientInfo>().ClientName.Value.ToString());
        }
    }

    [ClientRpc]
    private void ClearLobbyNamesClientRpc()
    {
        foreach (Transform child in playerPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    [ClientRpc]
    private void SpawnClientLobbyUIClientRpc(string newName)
    {
        SpawnClientLobbyUI(newName);
    }

    private void SpawnClientLobbyUI(string clientName)
    {
        GameObject uiRef = Instantiate(clientLobbyNameUIPrefab, playerPanel.transform);
        uiRef.GetComponent<TMP_Text>().text = clientName;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientInfo>().lobbyUIRef = uiRef;
    }
    
    public void UpdateClientName()
    {
        if (IsServer)
        {
            if (myLocalClient != null)
            {
                myLocalClient.GetComponent<ClientInfo>().ClientName.Value = playerNameInputField.text;
                HandleClientNameChange();
            }
            else
            {
                print("No local client found");
            }
        }
        else
        {
            RequestClientNameChangeServerRpc(myLocalClientId, playerNameInputField.text);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestClientNameChangeServerRpc(ulong clientId, string name)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<ClientInfo>().ClientName
            .Value = name;
        HandleClientNameChange();
    }

    public void UpdateLevelText(string level)
    {
        selectedLevelText.text = level;
    }
    
    #endregion
}
