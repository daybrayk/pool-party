using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Netcode.Transports.PhotonRealtime;
public class ClientNetPortal : MonoBehaviour
{
    const int TimeoutDuration = 10;

    GameNetPortal portal;
    
    public DisconnectReason disconnectReason { get; private set; } = new DisconnectReason();

    public event System.Action<ConnectStatus> ConnectFinishedEvt;
    public event System.Action NetworkTimeOutEvt;
    public static ClientNetPortal instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        portal = GetComponent<GameNetPortal>();
        NetworkManager.Singleton.OnClientDisconnectCallback += OnUserDisconnectRequest;
    }

    public static bool StartClientRelayMode(GameNetPortal portal, string roomKey, out string failMessage, CancellationToken cancellationToken)
    {
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as PhotonRealtimeTransport;

        transport.RoomName = roomKey;

        if (!cancellationToken.IsCancellationRequested) ConnectClient(portal);

        failMessage = string.Empty;
        return true;
    }

    private static void ConnectClient(GameNetPortal portal)
    {
        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            clientGUID = System.Guid.NewGuid().ToString(),
            clientScene = SceneManager.GetActiveScene().buildIndex,
            playerName = portal.playerName,
        });

        var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        portal.networkManager.NetworkConfig.ConnectionData = payloadBytes;
        portal.networkManager.NetworkConfig.ClientConnectionBufferTimeout = TimeoutDuration;

        portal.networkManager.StartClient();

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(ReceiveServerToClientConnectResult_CustomMessage), ReceiveServerToClientConnectResult_CustomMessage);
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(nameof(ReceiveServerToClientSetDisconnectReason_CustomMessage), ReceiveServerToClientSetDisconnectReason_CustomMessage);
    }

    public static void ReceiveServerToClientConnectResult_CustomMessage(ulong clientID, FastBufferReader reader)
    {
        reader.ReadValueSafe(out ConnectStatus status);
        instance.OnConnectFinished(status);
    }

    public static void ReceiveServerToClientSetDisconnectReason_CustomMessage(ulong clientID, FastBufferReader reader)
    {
        reader.ReadValueSafe(out ConnectStatus status);
        instance.OnDisconnectReceived(status);
    }

    public void OnNetworkReady()
    {
        if (!portal.networkManager.IsClient) enabled = false;
    }

    public void OnUserDisconnectRequest(ulong clientId)
    {
        if (portal.networkManager.IsClient) disconnectReason.SetDisconnectReason(ConnectStatus.UserRequestedDisconnect);
    }

    public void OnConnectFinished(ConnectStatus status)
    {
        if (status != ConnectStatus.Success) disconnectReason.SetDisconnectReason(status);

        ConnectFinishedEvt?.Invoke(status);
    }

    void OnDisconnectReceived(ConnectStatus status)
    {
        disconnectReason.SetDisconnectReason(status);
    }

    void OnClientDisconnectOrTimeout(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            //If active scene is the Main Menu then we "timed out" rather than disconnected
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                NetworkManager.Singleton.Shutdown();
                if (!disconnectReason.hasTransitionReason) disconnectReason.SetDisconnectReason(ConnectStatus.GenericDisconnect);

                SceneManager.LoadScene("MainMenu");
            }
            else if (disconnectReason.reason == ConnectStatus.GenericDisconnect || disconnectReason.reason == ConnectStatus.Undefined)
            {
                NetworkTimeOutEvt?.Invoke();
            }
        }
    }
}