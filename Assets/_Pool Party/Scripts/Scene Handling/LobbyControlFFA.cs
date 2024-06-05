using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
/// <summary>
/// Lobby controller for Free For All matches
/// </summary>
public class LobbyControlFFA : LobbyControl
{

    new protected void Awake()
    {
        base.Awake();

        if (NetworkManager.Singleton.IsListening && IsServer)
        {
            var c = Instantiate(clientPrefab, clientList.transform).GetComponent<ClientUIController>();

            c.SetClientName("Player " + NetworkManager.Singleton.LocalClientId);

            clientsInLobby.Add(NetworkManager.Singleton.LocalClientId, c);

            allPlayersInLobby = false;
        }
    }

    protected override void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            if (!clientsInLobby.ContainsKey(clientId))
            {
                var c = Instantiate(clientPrefab, clientList.transform).GetComponent<ClientUIController>();
                c.SetClientName("Player " + clientId);
                Debug.Log("adding player with clientId: " + clientId);
                clientsInLobby.Add(clientId, c);
            }
            //GenerateUserLobbyStatus();

            UpdateAndCheckPlayersInLobby();
        }
    }

    //protected override void OnClientDisconnected(ulong clientId)
    //{
    //    Debug.LogFormat("LobbyControl.OnClientDisconnected({0})", clientId);
    //    if (IsServer)
    //    {
    //        var disconnectedClient = clientsInLobby[clientId];
    //        clientsInLobby.Remove(clientId);
    //        Destroy(disconnectedClient.gameObject);
    //    }
    //    else
    //    {
    //        SceneTransitionHandler.instance.ExitAndLoadStartMenu();
    //    }
    //}

    protected override void ClientLoadedScene(ulong clientId)
    {
        if (IsServer)
        {
            if (!clientsInLobby.ContainsKey(clientId))
            {
                var c = Instantiate(clientPrefab, clientList.transform).GetComponent<ClientUIController>();
                c.SetClientName("Player " + clientId);
                Debug.Log("adding player with clientId: " + clientId);
                clientsInLobby.Add(clientId, c);
                //GenerateUserLobbyStatus();
            }

            UpdateAndCheckPlayersInLobby();
        }
    }

    [ClientRpc]
    protected override void SendClientReadyStatusUpdatesClientRpc(ulong clientId, string name, bool isReady)
    {
        if (!IsServer)
        {
            if (!clientsInLobby.ContainsKey(clientId))
            {
                Debug.Log("adding player with clientId: " + clientId);
                var c = Instantiate(clientPrefab, clientList.transform).GetComponent<ClientUIController>();
                c.SetClientName("Player " + clientId);
                c.SetClientStatus(isReady);

                clientsInLobby.Add(clientId, c);
            }
            else
            {
                clientsInLobby[clientId].SetClientStatus(isReady);
            }
        }
    }
}
