using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public interface ISessionPlayerData
{
    bool isConnected { get; set; }
    ulong clientId { get; set; }
    void Reinitialize();
}

public class SessionManager<T> where T: struct, ISessionPlayerData
{
    const string HostGuid = "host_guid";

    NetworkManager networkManager;

    /// <summary>
    /// Maps a client guid to the data for a given player
    /// </summary>
    Dictionary<string, T> clientData;

    /// <summary>
    /// Maps a clientId to the corredsponding guid
    /// </summary>
    Dictionary<ulong, string> clientIdToGuid;

    public static SessionManager<T> _instance;
    public static SessionManager<T> instance => _instance ??= new SessionManager<T>();
    protected SessionManager()
    {
        networkManager = NetworkManager.Singleton;
        if (networkManager) networkManager.OnServerStarted += ServerStartedHandler;

        clientData = new Dictionary<string, T>();
        clientIdToGuid = new Dictionary<ulong, string>();
    }

    ~SessionManager()
    {
        if (networkManager) networkManager.OnServerStarted -= ServerStartedHandler;
    }

    void ServerStartedHandler()
    {
        if (networkManager.IsServer) networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }


    public void AddHostData(T sessionPlayerData)
    {
        if (sessionPlayerData.clientId == NetworkManager.ServerClientId)
        {
            clientData[HostGuid] = sessionPlayerData;
            clientIdToGuid[sessionPlayerData.clientId] =  HostGuid;
        }
        else
        {
            Debug.LogError($"Invalid clientId for host. Got {sessionPlayerData.clientId}, but NetworkManager.ServerClientId is {NetworkManager.ServerClientId}");
        }
    }

    void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToGuid.TryGetValue(clientId, out var guid))
        {
            if (TryGetPlayerData(guid, out T data) && data.clientId == clientId)
            {
                data.isConnected = false;
                clientData[guid] = data;
            }
        }

        if (clientId == networkManager.LocalClientId)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    public bool SetupConnectingPlayerSessionData(ulong clientId, string clientGuid, T sessionPlayerData)
    {
        bool success = true;

        if (clientData.ContainsKey(clientGuid))
        {
            bool isReconnecting = false;
            if (clientData[clientGuid].isConnected)
            {
                success = false;
            }
            else
            {
                isReconnecting = true;
            }

            if (isReconnecting)
            {
                sessionPlayerData = clientData[clientGuid];
                sessionPlayerData.clientId = clientId;
                sessionPlayerData.isConnected = true;
            }

        }
        
        if (success)
        {
            clientData[clientGuid] = sessionPlayerData;
            clientIdToGuid[clientId] = clientGuid;
        }

        return success;
    }

    public bool TryGetPlayerData(string guid, out T data)
    {
        if (clientData.TryGetValue(guid, out data)) return true;

        Debug.LogError($"No PlayerData for guid: {guid} was found");
        return false;
    }

    public bool TryGetPlayerData(ulong clientId, out T Data)
    {
        Data = default(T);
        if (!clientIdToGuid.ContainsKey(clientId))
        {
            Debug.Log($"ClientId to Guid list does not contain id {clientId}");
            return false;
        }
        if (TryGetPlayerData(clientIdToGuid[clientId], out Data)) return true;

        return false;
    }

    public void SetPlayerData(ulong clientId, T sessionPlayerData)
    {
        if (clientIdToGuid.TryGetValue(clientId, out string clientGuid)) clientData[clientGuid] = sessionPlayerData;
        else
        {
            Debug.LogError($"No client guid found corresponding to given clientId: {clientId}");
        }
    }

    public void OnSessionStarted()
    {
        ClearDisconnectedPlayersData();
    }

    public void OnSessionEnded()
    {
        ClearDisconnectedPlayersData();
        List<ulong> connectedClientIds = new List<ulong>(networkManager.ConnectedClientsIds);

        foreach (var id in clientIdToGuid.Keys)
        {
            if (connectedClientIds.Contains(id))
            {
                var guid = clientIdToGuid[id];
                var sessionPlayerData = clientData[guid];
                sessionPlayerData.Reinitialize();
                clientData[guid] = sessionPlayerData;
            }
        }
    }

    void ClearDisconnectedPlayersData()
    {
        List<ulong> idsToClear = new List<ulong>();
        List<ulong> connectedClientIds = new List<ulong>(networkManager.ConnectedClientsIds);

        foreach (var id in clientIdToGuid.Keys)
        {
            if (!connectedClientIds.Contains(id)) idsToClear.Add(id);
            else
            {
                string guid = clientIdToGuid[id];
                T sessionPlayerData = clientData[guid];
                sessionPlayerData.Reinitialize();
                clientData[guid] = sessionPlayerData;
            }
        }

        foreach (var id in idsToClear)
        {
            string guid = clientIdToGuid[id];
            
            //I don't think all of this is necessary but it is what they do in boss room.
            //Temporarily simplifying to see if it still works
            //if (TryGetPlayerData(guid, out T data) && data.clientId == id) clientData.Remove(guid);
            
            clientData.Remove(guid);

            clientIdToGuid.Remove(id);
        }
    }

    public void OnUserDisconnectRequest()
    {
        Clear();
    }

    void Clear()
    {
        clientData.Clear();
        clientIdToGuid.Clear();
    }
}