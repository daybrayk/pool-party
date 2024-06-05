using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using Unity.Netcode;
using Daybrayk;
public partial class GameModeBase : NetworkBehaviour
{
    public enum GameModeStates
    {
        Default,
		PlayersConnecting,
		Setup,
        Starting,
		Started,
        Ending,
    }

    public System.Action onGameOver;

    [SerializeField]
    protected PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection;
    //[SerializeField]
    //protected AssetReferenceT<PersistentPlayerRuntimeCollection> persistentPlayerRuntimeAsset;
    [Header("Start Game")]
	[SerializeField]
	protected PlayerSpawn[] spawnPoints;
    [SerializeField]
    protected float startDelay = 5.0f;

    [Header("End Game")]
    [SerializeField]
    protected float endDelay = 5.0f;


    [Header("Scoring")]
    [SerializeField]
    protected bool useScoreLimit;
    [SerializeField]
    protected int _scoreLimit;
    public int scoreLimit => _scoreLimit;
    [SerializeField]
    protected Slider scoreSlider;
    [SerializeField]
    protected TMP_Text scoreText;
    [SerializeField]
    protected TMP_Text localTeamNameText;

    [Header("Game Timer")]
    [SerializeField]
    protected bool useTimer;
    [SerializeField]
    [Tooltip("In minutes")]
    protected float gameTime;

    [Header("UI")]
    [SerializeField]
    protected HudController hud;
    [SerializeField]
    protected GameOverUIController gameOverUI;
    [SerializeField]
    protected TMP_Text gameTimeText;
    [SerializeField]
    protected TextMeshProUGUI delayTimerText;

    [Header("Debug")]
    [SerializeField]
    [ReadOnly]
    protected List<GameObject> connectedAvatars = new List<GameObject>();
    [SerializeField]
    protected LayerMask spawnMask;
	
    protected NetworkList<int> scores;
    public GameModeStates clientCurrentState { get; private set; }
    protected NetworkVariable<GameModeStates> serverCurrentState = new NetworkVariable<GameModeStates>();
    public static GameModeBase instance { get; private set; }
    
    protected PersistentPlayer _localPlayer;
    public PersistentPlayer localPlayer => _localPlayer;

    protected float timer;
    protected NetworkVariable<int> _winner;
    public int winner => _winner.Value;

    AsyncOperationHandle<PersistentPlayerRuntimeCollection> handle;

    protected void Awake()
    {
        if (!instance)
        {
            instance = this;
            scores = new NetworkList<int>();
            _winner = new NetworkVariable<int>();

            //handle = Addressables.LoadAssetAsync<PersistentPlayerRuntimeCollection>(persistentPlayerRuntimeAsset);
            //
            //handle.Completed += (operation) =>
            //{
            //    persistentPlayerRuntimeCollection = operation.Result;
            //};
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public override void OnNetworkSpawn()
	{
        persistentPlayerRuntimeCollection.TryGetPlayer(NetworkManager.Singleton.LocalClientId, out _localPlayer);

        SetupStateMachine();
        if(!IsServer)
        {
            stateMachine.ChangeState(GameModeStates.PlayersConnecting);
            scores.OnListChanged += Scores_OnListChanged;
            serverCurrentState.OnValueChanged += ServerState_OnValueChanged;
        }
        else
        {
            stateMachine.ChangeState(GameModeStates.PlayersConnecting);
            stateMachine.stateChanged += OnStateChanged;
        }
        
        if (IsServer && SceneTransitionHandler.instance == null)
        {
            stateMachine.ChangeState(GameModeStates.Started);
        }
	}

    public override void OnNetworkDespawn()
    {
        //scores.OnListChanged -= Scores_OnListChanged;
    }

    //new private void OnDestroy()
    //{
    //    scores.Dispose();
    //    Addressables.Release(handle);
    //}

    protected void Update()
    {
        stateMachine.Update();
    }
    public void GameOver(ulong winner)
    {
        if (!IsServer) return;
        if (serverCurrentState.Value == GameModeStates.Ending) return;

        _winner.Value = (int)winner;
        onGameOver.Invoke();
        stateMachine.ChangeState(GameModeStates.Ending);
    }

    #region Virtual Methods
    public virtual void AdjustScore(int value, ulong damagingClientId, ulong damagedClientId) { }

    public virtual void AdjustScore(int value, ulong damagingTeamId) { }

    protected virtual IEnumerator SpawnPlayerAvatars() { yield break; }

    protected virtual void PlacePlayerAvatars() { }

    protected virtual void SetupScores() { }

    protected virtual void Scores_OnListChanged(NetworkListEvent<int> changeEvent)
    {
        hud.SetScoreText((ulong)changeEvent.Index, changeEvent.Value.ToString());
    }

    public virtual bool TryFindAvailableSpawnPoint(PersistentPlayer p, out PlayerSpawn spawnPoint)
    {
        float dist = 0;
        int index = 0;
        bool foundSpawn = false;
        var playerObjects = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var spawn = spawnPoints[i];

            Collider2D hit = Physics2D.OverlapCircle(spawn.transform.position, 3f, spawnMask);
            if (hit)
            {
                Debug.Log($"Object in radius of Spawn {hit.gameObject.name}", hit.gameObject);
                continue;
            }
            
            for (int j = 0; j < playerObjects.Count; j++)
            {
                var playerDist = Vector3.Distance(spawn.transform.position, playerObjects[j].PlayerObject.transform.position);
                
                if(playerDist > dist)
                {
                    dist = playerDist;
                    index = i;
                    foundSpawn = true;
                }
            }
        }

        if (foundSpawn)
        {
            spawnPoint = spawnPoints[index];
            return true;
        }

        Debug.Log("Didn't find a valid spawn point");
        spawnPoint = null;
        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void RequestSpawnServerRpc(ulong clientId)
    {
        if (persistentPlayerRuntimeCollection.TryGetPlayer(clientId, out var player))
        {
            if (TryFindAvailableSpawnPoint(player, out var spawn))
            {
                NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<CharacterRoot>().ResetToSpawnPositionClientRpc(spawn.transform.position);
            }
            else
            {
                Debug.LogError("Could not find a suitable spawn point");
            }
        }
    }
    #endregion

	//public void ChangeState(GameModeStates newState)
    //{
    //    if(IsServer)
    //    {
    //        Debug.Log("Changing game state to " + newState);
    //        ChangeStateClientRpc(newState);
    //
    //        EndState(serverCurrentState.Value);
    //        BeginState(newState);
    //        serverCurrentState.Value = newState;
    //    }
    //}
    //
    //[ClientRpc]
    //protected void ChangeStateClientRpc(GameModeStates newState)
    //{
    //    if (IsServer) return;
    //
    //    Debug.Log("Changing game state to " + newState);
    //    EndState(clientCurrentState);
    //    clientCurrentState = newState;
    //    BeginState(clientCurrentState);
    //}
    //
	//protected virtual void BeginState(GameModeStates state)
    //{
    //    switch (state)
    //    {
    //        case GameModeStates.PlayersConnecting:
    //            BeginConnectingState();
    //            break;
    //        case GameModeStates.Setup:
    //            BeginSetupState();
    //            break;
    //        case GameModeStates.Starting:
    //            BeginStartingState();
    //            break;
    //        case GameModeStates.Started:
    //            BeginStartedState();
    //            break;
    //        case GameModeStates.Ending:
    //            BeginEndingState();
    //            break;
    //    }
    //}
    //
    //protected virtual void EndState(GameModeStates state)
    //{
    //    switch (state)
    //    {
    //        case GameModeStates.PlayersConnecting:
    //            EndConnectingState();
    //            break;
    //        case GameModeStates.Setup:
    //            EndSetupState();
    //            break;
    //        case GameModeStates.Starting:
    //            EndStartingState();
    //            break;
    //        case GameModeStates.Started:
    //            EndStartedState();
    //            break;
    //        case GameModeStates.Ending:
    //            EndEndingState();
    //            break;
    //    }
    //}
}