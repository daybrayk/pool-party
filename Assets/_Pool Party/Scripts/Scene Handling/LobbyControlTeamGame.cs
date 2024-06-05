using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Daybrayk;
public class LobbyControlTeamGame : LobbyControl
{
    ////public enum GameModeType
    ////{
    ////    CarWash,
    ////    TeamDeathMatch,
    ////    ManHunt,
    ////    FreeForAll,
    ////}
    //
    //[Header("Team Mode")]
    //[SerializeField]
    //GameObject[] teamLists;
    //new protected void Awake()
    //{
    //    base.Awake();
    //
    //    if (NetworkManager.Singleton.IsListening && IsServer)
    //    {
    //        
    //        var c = Instantiate(clientPrefab, teamLists[0].transform).GetComponent<ClientUIController>();
    //
    //        switch (gameType)
    //        {
    //            case GameModeType.FreeForAll:
    //                break;
    //            default:
    //                c.EnableTeamSelect();
    //                break;
    //
    //        }
    //
    //        c.SetClientName("You");
    //
    //        clientsInLobby.Add(NetworkManager.Singleton.LocalClientId, c);
    //
    //        allPlayersInLobby = false;
    //
    //        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    //        SceneTransitionHandler.instance.OnClientLoadedScene += ClientLoadedScene;
    //    }
    //}
    //
    //protected override void UpdateAndCheckPlayersInLobby()
    //{
    //    bool lobbyCount = clientsInLobby.Count >= minimumPlayerCount;
    //    //Check if there are enough players on each team
    //    //bool teamCount = teamLists[0].transform.childCount > 0 && teamLists[1].transform.childCount > 0;
    //    allPlayersInLobby = lobbyCount/* && teamCount*/;
    //
    //    foreach (var client in clientsInLobby)
    //    {
    //        //Check if value is null and remove client from list on connected clients
    //        SendClientReadyStatusUpdatesClientRpc(client.Key, client.Value.isReady);
    //        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(client.Key)) allPlayersInLobby = false;
    //    }
    //
    //    CheckAllPlayersReady();
    //}
    //
    //public override void StartGame()
    //{
    //    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    //
    //    SceneTransitionHandler.instance.OnClientLoadedScene -= ClientLoadedScene;
    //    switch (gameType)
    //    {
    //        case GameModeType.CarWash:
    //            SceneTransitionHandler.instance.SwitchScene("Carwash Game");
    //            break;
    //        case GameModeType.TeamDeathMatch:
    //            SceneTransitionHandler.instance.SwitchScene("TDM Game");
    //            break;
    //        case GameModeType.ManHunt:
    //            SceneTransitionHandler.instance.SwitchScene("Man Hunt Game");
    //            break;
    //    }
    //}
    //
    //protected override void CheckAllPlayersReady()
    //{
    //    if (allPlayersInLobby)
    //    {
    //        var allPlayersReady = true;
    //
    //        foreach (var client in clientsInLobby)
    //        {
    //            if (!client.Value.isReady) allPlayersReady = false;
    //        }
    //
    //        startButton.GetComponentInChildren<Button>().interactable = allPlayersReady;
    //    }
    //    else
    //    {
    //        startButton.GetComponentInChildren<Button>().interactable = false;
    //    }
    //}
    //
    //protected override void OnClientConnected(ulong clientId)
    //{
    //    if (IsServer)
    //    {
    //        if (!clientsInLobby.ContainsKey(clientId))
    //        {
    //            if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer persistentPlayer))
    //            {
    //                SetGameTypeClientRpc((int)gameType);
    //                
    //                var c = Instantiate(clientPrefab, teamLists[persistentPlayer.teamId].transform).GetComponent<ClientUIController>();
    //                
    //                switch (gameType)
    //                {
    //                    case GameModeType.FreeForAll:
    //                        break;
    //                    default:
    //                        persistentPlayer.teamId = 0;
    //                        break;
    //                }
    //                
    //                c.SetClientName("Player " + clientId);
    //                clientsInLobby.Add(clientId, c);
    //            }
    //        }
    //
    //        UpdateAndCheckPlayersInLobby();
    //    }
    //}
    //
    //protected override void ClientLoadedScene(ulong clientId)
    //{
    //    if (IsServer)
    //    {
    //        if (!clientsInLobby.ContainsKey(clientId))
    //        {
    //            persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer persistentPlayer);
    //            persistentPlayer.teamId = (ulong)(teamLists[1].transform.childCount < teamLists[0].transform.childCount ? 1 : 0);
    //            
    //            var c = Instantiate(clientPrefab, teamLists[persistentPlayer.teamId].transform).GetComponent<ClientUIController>();
    //            c.SetClientName("Player " + clientId);
    //            clientsInLobby.Add(clientId, c);
    //        }
    //
    //        UpdateAndCheckPlayersInLobby();
    //    }
    //}
    //
    //[ClientRpc]
    //protected override void SendClientReadyStatusUpdatesClientRpc(ulong clientId, bool isReady)
    //{
    //    if (IsServer) return;
    //
    //    if (!clientsInLobby.ContainsKey(clientId))
    //    {
    //        PersistentPlayer tempPlayer;
    //        persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out tempPlayer);
    //        tempPlayer.teamId = (ulong)(teamLists[1].transform.childCount < teamLists[0].transform.childCount ? 1 : 0);
    //
    //        var c = Instantiate(clientPrefab, teamLists[tempPlayer.teamId].transform).GetComponent<ClientUIController>();
    //
    //        if (clientId == NetworkManager.Singleton.LocalClientId)
    //        {
    //            localPersistentPlayer = tempPlayer;
    //            c.SetClientName("You");
    //
    //            switch(gameType)
    //            {
    //                case GameModeType.FreeForAll:
    //                    break;
    //                default:
    //                    c.EnableTeamSelect();
    //                    break;
    //            }
    //        }
    //        else
    //        {
    //            c.SetClientName("Player " + clientId);
    //        }
    //
    //        clientsInLobby.Add(clientId, c);
    //    }
    //    
    //    clientsInLobby[clientId].SetClientStatus(isReady);
    //}
    //
    //public void SwapTeam()
    //{
    //    var id = NetworkManager.Singleton.LocalClientId;
    //    var uiClient = clientsInLobby[id];
    //    if (IsServer)
    //    {
    //        if(persistentPlayerRuntimeCollection.TryGetPlayer(id, out PersistentPlayer persistentPlayer))
    //        {
    //            if(gameType == GameModeType.ManHunt && persistentPlayer.teamId > 0)
    //            {
    //                if (teamLists[0].transform.childCount > 0) return;
    //            }
    //
    //            persistentPlayer.teamId = (persistentPlayer.teamId + 1) % (ulong)teamLists.Length;
    //            uiClient.transform.parent = teamLists[persistentPlayer.teamId].transform;
    //
    //            SwapTeamClientRpc(id, persistentPlayer.teamId);
    //        }
    //    }
    //    else
    //    {
    //        if (persistentPlayerRuntimeCollection.TryGetPlayer(id, out PersistentPlayer persistentPlayer))
    //        {
    //            if (gameType == GameModeType.ManHunt && persistentPlayer.teamId > 0)
    //            {
    //                if (teamLists[0].transform.childCount > 0) return;
    //            }
    //
    //            var teamId = (persistentPlayer.teamId + 1) % (ulong)teamLists.Length;
    //
    //            SwapTeamServerRpc(id, teamId);
    //        }
    //    }
    //}
    //
    //[ServerRpc(RequireOwnership = false)]
    //protected void SwapTeamServerRpc(ulong clientId, ulong teamId)
    //{
    //    var uiClient = clientsInLobby[clientId];
    //    if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer persistentPlayer))
    //    {
    //        persistentPlayer.teamId = teamId;
    //        uiClient.transform.parent = teamLists[persistentPlayer.teamId].transform;
    //    }
    //
    //    SwapTeamClientRpc(clientId, teamId);
    //}
    //
    //[ClientRpc]
    //protected void SwapTeamClientRpc(ulong clientId, ulong teamId)
    //{
    //    if (IsHost) return;
    //
    //    Debug.Log("SwapTeamClientRpc");
    //    var uiClient = clientsInLobby[clientId];
    //
    //    if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out PersistentPlayer persistentPlayer))
    //    {
    //        Debug.Log("Found Persistent Player");
    //        persistentPlayer.teamId = teamId;
    //        uiClient.transform.parent = teamLists[persistentPlayer.teamId].transform;
    //    }
    //}
    //
    ////public override void SetGameType(int value)
    ////{
    ////    if (!IsOwnedByServer) return;
    ////
    ////    gameType = (GameModeType)value;
    ////    SetGameTypeClientRpc(value);
    ////
    ////    if (gameType == GameModeType.ManHunt)
    ////    {
    ////        for (int i = 1; i < teamLists[0].transform.childCount; i++)
    ////        {
    ////            PersistentPlayer p = persistentPlayerRuntimeCollection.items[i];
    ////            var uiClient = clientsInLobby[p.clientId];
    ////            if (uiClient.transform.parent == teamLists[0].transform)
    ////            {
    ////                SwapTeamServerRpc(p.clientId, 1);
    ////            }
    ////        }
    ////    }
    ////}
    //
    //[ClientRpc]
    //void SetGameTypeClientRpc(int value)
    //{
    //    if (IsServer) return;
    //    gameType = (GameModeType)value;
    //
    //    switch (gameType)
    //    {
    //        case GameModeType.CarWash:
    //            gameTypeLabel.text = "Car Wash";
    //            break;
    //        case GameModeType.TeamDeathMatch:
    //            gameTypeLabel.text = "Team Deathmatch";
    //            break;
    //        case GameModeType.ManHunt:
    //            gameTypeLabel.text = "Man Hunt";
    //            break;
    //    }
    //}
}
