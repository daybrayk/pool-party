using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class SceneTransitionHandler : NetworkBehaviour
{
    public enum SceneStates
    { 
        Init,
        Start,
        Lobby,
        InGame,
    }
    static public SceneTransitionHandler instance { get; internal set; }

    public delegate void ClientLoadedSceneDelegateHandler(ulong clientId);
    public event ClientLoadedSceneDelegateHandler OnClientLoadedScene;

    public delegate void SceneStateChangedDelegateHandler(SceneStates newState);
    public event SceneStateChangedDelegateHandler OnSceneStateChanged;

    private int numberOfClientLoaded;

    private SceneStates sceneState;
    public SceneStates currentState => sceneState;

    private void Awake()
    {
        if(instance != this && instance != null) Destroy(instance.gameObject);

        instance = this;
        SetSceneState(SceneStates.Init);
    }

    private void Start()
    {
        if (sceneState == SceneStates.Init) SceneManager.LoadScene("MainMenu");
    }

    public void SetSceneState(SceneStates state)
    {
        sceneState = state;
        if (OnSceneStateChanged != null) OnSceneStateChanged.Invoke(sceneState);
    }

    public void SwitchScene(string sceneName)
    {
        numberOfClientLoaded = 0;
        if(NetworkManager.Singleton.IsListening)
        {
            Debug.Log("Network Manager loading scene");
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Scene Manager loading scene");
            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    void OnSceneEvent(SceneEvent e)
    {
        if (e.SceneEventType != SceneEventType.LoadComplete) return;

        OnClientLoadedScene?.Invoke(e.ClientId);

        numberOfClientLoaded += 1;
        if(numberOfClientLoaded == NetworkManager.Singleton.ConnectedClients.Count) NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
    }

    public bool AllClientsAreLoaded()
    {
        //Debug.Log("Number Of Clients: " + numberOfClientLoaded + " Connected Clients: " + NetworkManager.Singleton.ConnectedClients.Count);
        return numberOfClientLoaded == NetworkManager.Singleton.ConnectedClients.Count;
    }

    public void ExitAndInitialize()
    {
        OnClientLoadedScene = null;
        SetSceneState(SceneStates.Init);
        SceneManager.LoadSceneAsync(0);
    }

    public void ExitAndLoadStartMenu()
    {
        OnClientLoadedScene = null;
        SetSceneState(SceneStates.Start);
        SceneManager.LoadSceneAsync(1);
    }
}