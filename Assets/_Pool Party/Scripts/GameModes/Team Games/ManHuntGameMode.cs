using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Netcode;
using Daybrayk;
public class ManHuntGameMode : GameModeBase
{
    [Header("Man Hunt")]
    [SerializeField]
    GameObject hunterSpawn;
    [SerializeField]
    GameObject[] huntedSpawns;
    [SerializeField]
    GameObject hunterPrefab;
    [SerializeField]
    AssetReference hunterAsset;

    [SerializeField]
    GameObject hunteePrefab;
    [SerializeField]
    AssetReference huntedAsset;

    private void OnEnable()
    {
        GameEvents.instance.AddListener<CharacterCombatBase.PlayerSoakedEvt>(OnPlayerSoaked);
    }

    private void OnDisable()
    {
        GameEvents.instance.RemoveListener<CharacterCombatBase.PlayerSoakedEvt>(OnPlayerSoaked);
    }

    void OnPlayerSoaked(CharacterCombatBase.PlayerSoakedEvt e)
    {
        Debug.Log($"Damaging client: {e.damagingClientId}");
        AdjustScore(1, e.damagingClientId, e.owningClientId);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        localTeamNameText.text = "Players Hunted ";

        _scoreLimit = persistentPlayerRuntimeCollection.items.Count - 1;

        if (IsServer)
        {
            scores.Add(0);
            scoreText.text = scores[0].ToString() + "/" + _scoreLimit.ToString();
        }

        hud.AddPlayerScore(1, $"Hunter");

    }

    protected override IEnumerator SpawnPlayerAvatars()
    {
        bool asyncProcessComplete = false;
        var waitAsyncProcess = new WaitUntil(() => asyncProcessComplete);

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (persistentPlayerRuntimeCollection.TryGetPlayer(client.Value.ClientId, out PersistentPlayer p))
            {
                Debug.Log($"playerId: {p.clientId} teamId: {p.teamId.Value}");
                GameObject avatar = null;

                asyncProcessComplete = false;

                if (p.teamId.Value == 1)
                {
                    AddressablesManager.instance.InstantiateAsync(hunterAsset, (gameObject) =>
                    {
                        asyncProcessComplete = true;
                        avatar = gameObject;
                    });
                    //avatar = Instantiate(hunterPrefab);
                }
                else
                {
                    AddressablesManager.instance.InstantiateAsync(huntedAsset, (gameObject) =>
                    {
                        asyncProcessComplete = true;
                        avatar = gameObject;
                    });
                    //avatar = Instantiate(hunteePrefab);
                }

                yield return waitAsyncProcess;

                avatar.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.ClientId, true);
            }
            else
            {
                Debug.LogError("No Persistent Player found with Id: " + client.Value.ClientId);
            }
        }
    }

    protected override void PlacePlayerAvatars()
    {
        var playerObjects = NetworkManager.Singleton.ConnectedClientsList;
        int j = 0;
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (persistentPlayerRuntimeCollection.TryGetPlayer(playerObjects[i].ClientId, out PersistentPlayer persistent))
            {
                if (persistent.teamId.Value == 1)
                {
                    playerObjects[i].PlayerObject.GetComponent<CharacterRoot>().ResetToSpawnPositionClientRpc(hunterSpawn.transform.position);
                }
                else
                {
                    playerObjects[i].PlayerObject.GetComponent<CharacterRoot>().ResetToSpawnPositionClientRpc(huntedSpawns[j].transform.position);
                    j++;
                }
            }
        }
    }

    protected override void Scores_OnListChanged(NetworkListEvent<int> changeEvent)
    {
        scoreText.text = changeEvent.Value.ToString() + "/" + _scoreLimit;
        scoreSlider.value = changeEvent.Value / _scoreLimit;
    }

    //public override bool TryFindAvailableSpawnPoint(PersistentPlayer p, out PlayerSpawn spawnPoint)
    //{
    //    for (int i = 0; i < spawnPoints.Length; i++)
    //    {
    //        if (!spawnPoints[i].hasAssignedClient && spawnPoints[i].teamId == p.teamId.Value)
    //        {
    //            spawnPoint = spawnPoints[i];
    //            Debug.Log($"Spawn point has Team Id: {spawnPoint.teamId}");
    //            return true;
    //        }
    //    }
    //
    //    spawnPoint = null;
    //    return false;
    //}

    public override void AdjustScore(int value, ulong damagingClientId, ulong damagedClientId)
    {
        if (damagingClientId == damagedClientId) return;

        scores[0]++;
        scoreText.text = scores[0].ToString() + "/" + _scoreLimit.ToString();

        if (scores[0] >= _scoreLimit) GameOver(1);
    }

    public override void AdjustScore(int value, ulong damagingTeamId) { }

    protected override void BeginSetupState()
    {
        if(IsServer)
        {
            var ran = Random.Range(0, persistentPlayerRuntimeCollection.items.Count);
            persistentPlayerRuntimeCollection.items[ran].teamId.Value = (ulong)1;
            _winner.Value = 0;
        }

        base.BeginSetupState();

    }

    protected override void BeginStartingState()
    {
        base.BeginStartingState();

        if (localPlayer.teamId.Value == 1)
        {
            hud.ShowMessage("Players are hiding", startDelay - 1);
        }
        else
        {
            hud.ShowMessage("Hide", startDelay - 1);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<CharacterInputHandler>().EnableInput();
        }
    }

    protected override void BeginStartedState()
    {
        if (localPlayer.teamId.Value == 1)
        {
            hud.ShowMessage("HUNT!", 3);
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<CharacterInputHandler>().EnableInput();
        }
        else
        {
            hud.ShowMessage("The hunt begins!", 3);
        }
    }

    protected override void BeginEndingState()
    {
        if (IsServer)
        {
            for (int i = 0; i < persistentPlayerRuntimeCollection.items.Count; i++)
            {
                persistentPlayerRuntimeCollection.items[i].teamId.Value = 0;
            }
        }

        if (gameOverUI == null) return;

        gameOverUI.ShowUI();

        if(winner == 0) gameOverUI.SetWinnerText("Hunted");
        else
        {
            gameOverUI.SetWinnerText("Hunter");
        }

        timer = endDelay;
        delayTimerText.gameObject.SetActive(true);
        onGameOver.Invoke();
    }
}