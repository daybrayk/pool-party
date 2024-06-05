using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Collections;
public class ServerNetPortal : MonoBehaviour
{
	const int MaxConnectPayload = 1024;
	
	GameNetPortal portal;

	Dictionary<ulong, int> clientSceneMap = new Dictionary<ulong, int>();

	public int serverScene => SceneManager.GetActiveScene().buildIndex;

    private void Start()
    {
		portal = GetComponent<GameNetPortal>();

		portal.networkManager.ConnectionApprovalCallback += ApprovalCheck;
		portal.networkManager.OnServerStarted += ServerStartedHandler;
    }

    private void OnDestroy()
    {
        if (portal != null)
        {
            if (portal.networkManager != null)
            {
                portal.networkManager.ConnectionApprovalCallback -= ApprovalCheck;
                portal.networkManager.OnServerStarted -= ServerStartedHandler;
            }
        }
    }

    public void OnNetworkReady()
    {
        if (!portal.networkManager.IsServer) enabled = false;
        else
        {
            portal.networkManager.OnClientDisconnectCallback += OnClientDisconnect;

            NetworkManager.Singleton.SceneManager.LoadScene("New Lobby", LoadSceneMode.Single);

            if (portal.networkManager.IsHost) clientSceneMap[portal.networkManager.LocalClientId] = serverScene;
        }
    }

    void OnClientDisconnect(ulong clientId)
    {
        clientSceneMap.Remove(clientId);

        if (clientId == portal.networkManager.LocalClientId) portal.networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    public void OnClientSceneChanged(ulong clientId, int sceneIndex)
    {
        clientSceneMap[clientId] = sceneIndex;
    }

    public void OnUserDisconnectRequest()
    {
        Clear();
    }

    void Clear()
    {
        clientSceneMap.Clear();
    }

    public bool AllClientsInServerScene()
    {
        foreach (var kvp in clientSceneMap)
        {
            if (kvp.Value != serverScene) return false;

            return true;
        }

        return false;
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Running approval check");
        var clientId = request.ClientNetworkId;
        var connectionData = request.Payload;

        if (connectionData.Length > MaxConnectPayload)
        {
            Debug.LogError("ConnectionData length exceeded MaxConnectedPayload size.");
            //response(false, 0, false, null, null);
            return;
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Adding Host Player Data");
            SessionManager<SessionPlayerData>.instance.AddHostData(new SessionPlayerData(clientId, portal.playerName));

            //response(true, null, true, null, null);
            return;
        }

        ConnectStatus gameReturnStatus = ConnectStatus.Undefined;

        if (portal.networkManager.ConnectedClientsIds.Count > 10/*TODO: create max player count*/)
        {
            gameReturnStatus = ConnectStatus.ServerFull;

            SendServerToClientConnectResult(clientId, gameReturnStatus);
            SendServerToClientSetDisconnectReason(clientId, gameReturnStatus);
            //Possibly needs a small delay before disconnecting
            portal.networkManager.DisconnectClient(clientId);
        }

        string payload = System.Text.Encoding.UTF8.GetString(connectionData);
        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

        int clientScene = connectionPayload.clientScene;
        Debug.Log($"Host Approval Check: connection client GUID: {connectionPayload.clientGUID}, client Name: {connectionPayload.playerName}");
        Debug.Log($"Host Id: {NetworkManager.Singleton.LocalClientId}, Connection Client Id: {clientId}");
        //check session manager if player has previously been connected

        gameReturnStatus = SessionManager<SessionPlayerData>.instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.clientGUID,
            new SessionPlayerData(clientId, connectionPayload.playerName, true)) ? ConnectStatus.Success : ConnectStatus.ConnectedAgain;

        if (gameReturnStatus == ConnectStatus.ConnectedAgain)
        {
            Debug.Log("Client connected again!");
            ulong oldClientId = 1;
            SendServerToClientSetDisconnectReason(oldClientId, ConnectStatus.ConnectedAgain);
            portal.networkManager.DisconnectClient(clientId);

            return;
        }

        if (gameReturnStatus == ConnectStatus.Success)
        {
            Debug.Log("Connection Success!");
            SendServerToClientConnectResult(clientId, gameReturnStatus);
            clientSceneMap[clientId] = clientScene;
            //response(true, null, true, Vector3.zero, Quaternion.identity);
        }
    }

    void ServerStartedHandler()
    {

    }

    public void SendServerToClientSetDisconnectReason(ulong clientID, ConnectStatus status)
    {
        var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(nameof(ClientNetPortal.ReceiveServerToClientSetDisconnectReason_CustomMessage), clientID, writer);
    }

    public void SendServerToClientConnectResult(ulong clientID, ConnectStatus status)
    {
        var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(nameof(ClientNetPortal.ReceiveServerToClientConnectResult_CustomMessage), clientID, writer);
    }

}