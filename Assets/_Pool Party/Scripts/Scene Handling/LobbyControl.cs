using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.Netcode;
using Daybrayk;
using TMPro;

public class LobbyControl : NetworkBehaviour
{
    public enum GameModeType
    {
        [Description("Free For All")]
        FreeForAll,
        [Description("Team Death Match")]
        TeamDeathMatch,
        [Description("Man Hunt")]
        ManHunt,
        [Description("Car Wash")]
        CarWash,
        [Description("Capture the Flag")]
        CaptureTheFlag,
        [Description("VIP")]
        VIP,
    }

    public static bool isHosting;

    [Header("Client Stuff")]
    [SerializeField]
    protected PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection;
    //[SerializeField]
    //protected AssetReferenceT<PersistentPlayerRuntimeCollection> persistentPlayerRuntimeAsset;
    [SerializeField]
    protected GameObject clientPrefab;
    [SerializeField]
    protected AssetReference clientAsset;
    [SerializeField]
    protected int minimumPlayerCount = 2;

    [Header("UI")]
    public TMP_Text gameTypeLabel;
    [SerializeField]
    protected GameObject readyButton;
    [SerializeField]
    protected GameObject startButton;
    [SerializeField]
    TMP_Dropdown modeSelect;
    [SerializeField]
    [Daybrayk.ReadOnly]
    protected NetworkVariable<GameModeType> gameType;

    [SerializeField]
    protected GameObject clientList;

    protected PersistentPlayer localPersistentPlayer;
	protected bool allPlayersInLobby;
	protected Dictionary<ulong, ClientUIController> clientsInLobby;

    //AsyncOperationHandle<PersistentPlayerRuntimeCollection> handle;

    protected void Awake()
    {
        //handle = Addressables.LoadAssetAsync<PersistentPlayerRuntimeCollection>(persistentPlayerRuntimeAsset);
        //
        //handle.Completed += (operation) =>
        //{
        //    persistentPlayerRuntimeCollection = operation.Result;
        //};

        clientsInLobby = new Dictionary<ulong, ClientUIController>();
        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();

        foreach (var mode in (GameModeType[])System.Enum.GetValues(typeof(GameModeType)))
        {
            optionData.Add(new TMP_Dropdown.OptionData(mode.GetDescription()));
        }
        modeSelect.AddOptions(optionData);

        switch(GameNetPortal.instance.networkType)
        {
            case GameNetPortal.LobbyNetwork.Old:
                if (isHosting && !IsServer)
                {
                    if (NetworkManager.Singleton.StartHost()) Debug.Log("Starting Host");
                    else
                    {
                        Debug.LogError("Could not start Host");
                    }
                }
                else if(!IsClient)
                {
                    Debug.Log("Starting Client");
                    NetworkManager.Singleton.StartClient();
                }
                break;
        }

        SceneTransitionHandler.instance.SetSceneState(SceneTransitionHandler.SceneStates.Lobby);
    } 

    public override void OnNetworkSpawn()
    {
        var sm = SessionManager<SessionPlayerData>.instance;
        persistentPlayerRuntimeCollection.TryGetPlayer(NetworkManager.Singleton.LocalClientId, out localPersistentPlayer);
        
        if (IsServer)
        {
            //var c = Instantiate(clientPrefab, clientList.transform).GetComponent<ClientUIController>();

            AddressablesManager.instance.InstantiateAsync(clientAsset, clientList.transform, (gameObject) =>
            {
                var c = gameObject.GetComponent<ClientUIController>();

                switch (gameType.Value)
                {
                    case GameModeType.FreeForAll:
                        break;
                    case GameModeType.ManHunt:
                        break;
                    default:
                        c.EnableTeamSelect();
                        break;

                }

                if (sm.TryGetPlayerData(NetworkManager.Singleton.LocalClientId, out var playerData))
                {
                    Debug.Log($"[LobbyControl.OnNetworkSpawn] Updating player display name. Name: { playerData.playerName}");
                    localPersistentPlayer.displayName = playerData.playerName;
                    c.SetClientName(playerData.playerName);
                }
                else
                {
                    c.SetClientName($"Player {NetworkManager.Singleton.LocalClientId} (You)");
                }

                clientsInLobby.Add(NetworkManager.Singleton.LocalClientId, c);
                c.SetClientStatus(true);
                c.SetTeam((int)localPersistentPlayer.teamId.Value);

                allPlayersInLobby = false;

                readyButton.SetActive(false);

                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                SceneTransitionHandler.instance.OnClientLoadedScene += ClientLoadedScene;
                UpdateAndCheckPlayersInLobby();
            });
        }
        else
        {
            gameType.OnValueChanged += OnGameTypeChanged;
            startButton.SetActive(false);

            gameTypeLabel.text = gameType.Value.GetDescription();
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        SceneTransitionHandler.instance.OnClientLoadedScene -= ClientLoadedScene;
        gameType.OnValueChanged -= OnGameTypeChanged;
    }

    protected virtual void UpdateAndCheckPlayersInLobby()
    {
        Debug.Log("Updating players in lobby");
        allPlayersInLobby = clientsInLobby.Count >= minimumPlayerCount;

        foreach (var client in clientsInLobby)
        {
            Debug.Log($"SendingClientReadyStatusUpdates for client {client.Key}");
            //Check if value is null and remove client from list on connected clients
            SendClientReadyStatusUpdatesClientRpc(client.Key, client.Value.displayName, client.Value.isReady);
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(client.Key)) allPlayersInLobby = false;
        }

        CheckAllPlayersReady();
    }

    protected virtual void OnClientConnected(ulong clientId) 
    {
        if (IsServer)
        {
            Debug.Log($"Client connected with clientId: {clientId}");
            if (!clientsInLobby.ContainsKey(clientId))
            {
                if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer persistentPlayer))
                {
                    AddressablesManager.instance.InstantiateAsync(clientAsset, clientList.transform, (gameObject) =>
                    {
                        Debug.Log($"[LobbyControl.OnClientConnected] Updating player display name. Name: {persistentPlayer.displayName}");
                        var c = gameObject.GetComponent<ClientUIController>();

                        var sm = SessionManager<SessionPlayerData>.instance;
                        if (sm.TryGetPlayerData(clientId, out var playerData))
                        {
                            persistentPlayer.displayName = playerData.playerName;
                            c.SetClientName(playerData.playerName);
                        }
                        else
                        {
                            c.SetClientName($"Player {clientId} (You)");
                        }

                        c.SetTeam((int)persistentPlayer.teamId.Value);
                        clientsInLobby.Add(clientId, c);
                        
                        UpdateAndCheckPlayersInLobby();
                    });


                    //SetGameTypeClientRpc((int)gameType);
                }
            }
            else
            {
                UpdateAndCheckPlayersInLobby();
            }

        }
    }
    protected virtual void OnClientDisconnected(ulong clientId) 
    {
        Debug.LogFormat("LobbyControl.OnClientDisconnected({0})", clientId);
        if (IsServer && clientsInLobby.ContainsKey(clientId))
        {
            var disconnectedClient = clientsInLobby[clientId];
            clientsInLobby.Remove(clientId);
            Destroy(disconnectedClient.gameObject);
            ClientDisconnectedClientRpc(clientId);
        }
        else
        {
            SceneTransitionHandler.instance.ExitAndLoadStartMenu();
        }
    }

    protected virtual void ClientLoadedScene(ulong clientId) 
    {
        Debug.Log($"Client {clientId} loaded scene");
        if (!clientsInLobby.ContainsKey(clientId))
        {
            if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer persistentPlayer))
            {
                AddressablesManager.instance.InstantiateAsync(clientAsset, (gameObject) =>
                {
                    Debug.Log($"[LobbyControl.ClientLoadedScene] Updating player display name. Name: {persistentPlayer.displayName}");
                    var c = gameObject.GetComponent<ClientUIController>();

                    var sm = SessionManager<SessionPlayerData>.instance;
                    if (sm.TryGetPlayerData(clientId, out var playerData))
                    {
                        persistentPlayer.displayName = playerData.playerName;
                        c.SetClientName(playerData.playerName);
                    }
                    else
                    {
                        c.SetClientName($"Player {clientId} (You)");
                    }

                    c.SetTeam((int)persistentPlayer.teamId.Value);
                    Debug.Log($"[ClientLoadedScene] Adding client {clientId} to list");
                    clientsInLobby.Add(clientId, c);

                    UpdateAndCheckPlayersInLobby();
                });

                //SetGameTypeClientRpc((int)gameType);
            }
        }
        else
        {
            UpdateAndCheckPlayersInLobby();
        }

    }

    [ClientRpc]
    protected virtual void ClientDisconnectedClientRpc(ulong clientId)
    {
        if (IsOwner || IsHost) return;

        var disconnectedClient = clientsInLobby[clientId];
        clientsInLobby.Remove(clientId);
        Destroy(disconnectedClient.gameObject);
    }

    [ClientRpc]
    protected virtual void SendClientReadyStatusUpdatesClientRpc(ulong clientId, string displayName, bool isReady) 
    { 
        if (IsServer) return;
        persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer tempPlayer);
        Debug.Log($"[ClientRpc] is client {clientId} in list {clientsInLobby.ContainsKey(clientId)}");
        if (!clientsInLobby.ContainsKey(clientId))
        {

            //var c = Instantiate(clientPrefab, clientList.transform).GetComponent<ClientUIController>();
            AddressablesManager.instance.InstantiateAsync(clientAsset, clientList.transform, (gameObject) =>
            {
                var c = gameObject.GetComponent<ClientUIController>();

                Debug.Log($"[LobbyControl.SendClientReadyStatusUpdatesClientRpc] Updating player display name. Name: {tempPlayer.displayName}");
                tempPlayer.displayName = displayName;
                c.SetClientName(displayName);
                c.SetClientName(tempPlayer.displayName);

                if (clientId == NetworkManager.Singleton.LocalClientId)
                {
                    localPersistentPlayer = tempPlayer;


                    switch (gameType.Value)
                    {
                        case GameModeType.FreeForAll:
                            break;
                        case GameModeType.ManHunt:
                            break;
                        default:
                            c.EnableTeamSelect();
                            break;
                    }
                }

                Debug.Log($"[ClientRpc] Adding client {clientId} to list");
                clientsInLobby.Add(clientId, c);

                clientsInLobby[clientId].SetTeam((int)tempPlayer.teamId.Value);
                clientsInLobby[clientId].SetClientStatus(isReady);
            });
        }
        else
        {
            clientsInLobby[clientId].SetTeam((int)tempPlayer.teamId.Value);
            clientsInLobby[clientId].SetClientStatus(isReady);
        }
        
    }

    public virtual void StartGame() 
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

        SceneTransitionHandler.instance.OnClientLoadedScene -= ClientLoadedScene;
        switch (gameType.Value)
        {
            case GameModeType.CarWash:
                SceneTransitionHandler.instance.SwitchScene("Carwash Game");
                break;
            case GameModeType.TeamDeathMatch:
                SceneTransitionHandler.instance.SwitchScene("TDM Game");
                break;
            case GameModeType.ManHunt:
                SceneTransitionHandler.instance.SwitchScene("Man Hunt Game");
                break;
            case GameModeType.FreeForAll:
                SceneTransitionHandler.instance.SwitchScene("FFA Game");
                break;
            case GameModeType.CaptureTheFlag:
                SceneTransitionHandler.instance.SwitchScene("Capture the Flag Game");
                break;
            case GameModeType.VIP:
                SceneTransitionHandler.instance.SwitchScene("VIP Game");
                break;
        }
    }

    protected virtual void CheckAllPlayersReady()
    {
        if (allPlayersInLobby)
        {
            var allPlayersReady = true;

            foreach (var client in clientsInLobby)
            {
                if (!client.Value.isReady) allPlayersReady = false;
            }

            startButton.GetComponentInChildren<Button>().interactable = allPlayersReady;
        }
        else
        {
            startButton.GetComponentInChildren<Button>().interactable = false;
        }
    }

    public void TogglePlayerReady()
    {
        if(!IsServer)
        {
            var client = clientsInLobby[NetworkManager.Singleton.LocalClientId];

            //client.SetClientStatus(!client.isReady);
            ClientIsReadyServerRpc(NetworkManager.Singleton.LocalClientId, !client.isReady);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected void ClientIsReadyServerRpc(ulong clientId, bool isReady)
    {
        Debug.Log("ClientIsReadyServerRpc clientId = " + clientId);
        if (clientsInLobby.ContainsKey(clientId))
        {
            Debug.Log("Updating client ready status: " + isReady);
            clientsInLobby[clientId].SetClientStatus(isReady);
            UpdateAndCheckPlayersInLobby();
        }
    }

    public void RequestDisconnect()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        Debug.Log("Requesting disconnect from clientId " + clientId + ". IsHost: " + IsHost);
        if (!IsHost) Debug.Log("Host is clientId: " + NetworkManager.ServerClientId);
        if (IsServer)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
            SceneTransitionHandler.instance.ExitAndInitialize();
        }
        else
        {
            clientId = NetworkManager.Singleton.LocalClientId;
            RequestDisconnectServerRpc(clientId);
            //SceneTransitionHandler.instance.ExitAndLoadStartMenu();
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDisconnectServerRpc(ulong clientId)
    {
        Debug.Log("Calling NetworkManager.DisconnectClient()");
        NetworkManager.Singleton.DisconnectClient(clientId);
    }

    public void NextTeam()
    {
        ChangeTeam(1);
    }

    public void PrevTeam()
    {
        ChangeTeam(-1);
    }

    public void ChangeTeam(int value)
    {
        var id = NetworkManager.Singleton.LocalClientId;
        var uiClient = clientsInLobby[id];

        if (IsServer)
        {
            if (persistentPlayerRuntimeCollection.TryGetPlayer(id, out PersistentPlayer player))
            {
                if (value == 0)
                {
                    player.teamId.Value = 0;
                    uiClient.SetTeam(0);
                }
                else
                {
                    int team = (int)player.teamId.Value + value;
                    if (team >= ConstantValues.Max_Team_Count) team = 0;
                    if (team < 0) team = ConstantValues.Max_Team_Count - 1;
                    player.teamId.Value = (ulong)team;
                    uiClient.SetTeam((int)player.teamId.Value);
                }
                ChangeTeamClientRpc(id, (int)player.teamId.Value);
            }
        }
        else
        {
            ChangeTeamServerRpc(id, value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected void ChangeTeamServerRpc(ulong clientId, int value)
    {
        Debug.Log("Changing teams");
        var uiClient = clientsInLobby[clientId];

        if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer player))
        {
            if (value == 0)
            {
                player.teamId.Value = 0;
                uiClient.SetTeam(0);
            }
            else
            {
                int team = (int)player.teamId.Value + value;
                if (team >= ConstantValues.Max_Team_Count) team = 0;
                if (team < 0) team = ConstantValues.Max_Team_Count - 1;
                player.teamId.Value = (ulong)team;
                uiClient.SetTeam((int)player.teamId.Value);
            }
            ChangeTeamClientRpc(clientId, (int)player.teamId.Value);
        }

    }

    [ClientRpc]
    protected void ChangeTeamClientRpc(ulong clientId, int value)
    {
        if (IsServer) return;

        var uiClient = clientsInLobby[clientId];

        if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer player))
        {
            uiClient.SetTeam(value);
        }
    }


    IEnumerator ShutdownHelper()
    {
        yield return new WaitWhile(() => NetworkManager.Singleton.ShutdownInProgress);
        SceneTransitionHandler.instance.ExitAndLoadStartMenu();

    }

    public void SetGameType(int value)
    {
        if (!IsOwnedByServer) return;

        gameType.Value = (GameModeType)value;

        UpdateUIForGameMode();
    }

    void OnGameTypeChanged(GameModeType prevValue, GameModeType newValue)
    {
        if (IsServer) return;

        UpdateUIForGameMode();
    }

    void UpdateUIForGameMode()
    {
        var clientId = NetworkManager.Singleton.LocalClientId;
        
        if(!IsServer) ClientIsReadyServerRpc(clientId, false);

        Debug.Log($"ClientId: {clientId}");
        var c = clientsInLobby[clientId];
        gameTypeLabel.text = gameType.Value.GetDescription();

        switch (gameType.Value)
        {
            case GameModeType.CarWash:
                c.EnableTeamSelect();
                break;
            case GameModeType.TeamDeathMatch:
                c.EnableTeamSelect();
                break;
            case GameModeType.ManHunt:
                if (IsServer) ChangeTeam(0);
                else ChangeTeamServerRpc(clientId, 0);
                c.DisableTeamSelect();
                break;
            case GameModeType.FreeForAll:
                if (IsServer) ChangeTeam(0);
                else ChangeTeamServerRpc(clientId, 0);
                c.DisableTeamSelect();
                break;
            case GameModeType.CaptureTheFlag:
                c.EnableTeamSelect();
                break;
            case GameModeType.VIP:
                c.EnableTeamSelect();
                break;
        }
    }

    //private new void OnDestroy()
    //{
    //    base.OnDestroy();
    //    Addressables.Release(handle);
    //}
}
