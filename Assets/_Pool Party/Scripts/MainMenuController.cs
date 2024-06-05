using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Photon.Realtime;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    SceneReference ffaLobbyScene;
    [SerializeField]
    SceneReference tdmLobbyScene;

    [SerializeField]
    ResponseModal responsePopup;
    

	public void HostFFA()
    {
        LobbyControl.isHosting = true;
        SceneTransitionHandler.instance.SwitchScene(ffaLobbyScene);
    }

    public void JoinFFA()
    {
        LobbyControl.isHosting = false;
        SceneTransitionHandler.instance.SwitchScene(ffaLobbyScene);
    }

    public void HostTDM()
    {
        switch (GameNetPortal.instance.networkType)
        {
            case GameNetPortal.LobbyNetwork.Old:
                LobbyControl.isHosting = true;
                SceneTransitionHandler.instance.SwitchScene("New Lobby");
                break;
            case GameNetPortal.LobbyNetwork.New:
                responsePopup.SetupCallback((playerName, roomName) => ServerModalCallback(playerName, roomName));
                responsePopup.gameObject.SetActive(true);
                break;
        }
    }

    public void JoinTDM()
    {
        switch (GameNetPortal.instance.networkType)
        {
            case GameNetPortal.LobbyNetwork.Old:
                LobbyControl.isHosting = false;
                SceneTransitionHandler.instance.SwitchScene("New Lobby");
                break;
            case GameNetPortal.LobbyNetwork.New:
                responsePopup.SetupCallback((playerName, roomName) => ClientModalCallback(playerName, roomName));
                responsePopup.gameObject.SetActive(true);
                break;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ServerModalCallback(string playerName, string roomName)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        GameNetPortal.instance.playerName = playerName;
        GameNetPortal.instance.StartHost(roomName, cancellationTokenSource.Token);
    }

    public void ClientModalCallback(string playerName, string roomName)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        GameNetPortal.instance.playerName = playerName;
        if (!ClientNetPortal.StartClientRelayMode(GameNetPortal.instance, roomName, out string failMessage, cancellationTokenSource.Token))
        {
            Debug.LogError($"Connection failed: {failMessage}");
        }
    }
}