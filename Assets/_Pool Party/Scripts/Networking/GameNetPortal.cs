using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.PhotonRealtime;
public enum ConnectStatus
{
    Undefined,
    Success,
    ServerFull,
    ConnectedAgain,
    UserRequestedDisconnect,
    GenericDisconnect,
}

[System.Serializable]
public class ConnectionPayload
{
    public string clientGUID;
    public int clientScene = -1;
    public string playerName;
}

public class GameNetPortal : MonoBehaviour
{
    public enum LobbyNetwork
    {
        Old,
        New,
    }

    [SerializeField]
    NetworkManager _networkManager;
    public NetworkManager networkManager => _networkManager;

    [SerializeField]
    PhotonRealtimeTransport transport;

    public static GameNetPortal instance;
    private ClientNetPortal clientPortal;
    ServerNetPortal serverPortal;

    public LobbyNetwork networkType = LobbyNetwork.Old;
    public string playerName;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        clientPortal = GetComponent<ClientNetPortal>();
        serverPortal = GetComponent<ServerNetPortal>();

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        networkManager.OnServerStarted += OnNetworkReady;
        networkManager.OnClientConnectedCallback += ClientNetworkReady;
    }

    public void StartHost(string roomName, CancellationToken token)
    {
        transport.RoomName = roomName;
        if (!token.IsCancellationRequested) networkManager.StartHost();
    }

    private void OnNetworkReady()
    {
        if (networkManager.IsHost)
        {
            clientPortal.OnConnectFinished(ConnectStatus.Success);
        }

        clientPortal.OnNetworkReady();
        serverPortal.OnNetworkReady();
    }

    private void ClientNetworkReady(ulong clientId)
    {
        if (clientId == networkManager.LocalClientId)
        {
            OnNetworkReady();
            if (networkManager.IsServer)
            {
                networkManager.SceneManager.OnSceneEvent += OnSceneEvent;
            }
        }
    }

    private void OnSceneEvent(SceneEvent e)
    {
        if (e.SceneEventType != SceneEventType.LoadComplete) return;


    }

    private void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.OnServerStarted -= OnNetworkReady;
            networkManager.OnClientConnectedCallback -= ClientNetworkReady;
        }

        if (instance == this)
        {
            instance = null;
        }
    }
}