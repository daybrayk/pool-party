using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Netcode;
using Daybrayk;
public class VipGameMode : TeamGameMode
{
    [Header("VIP Mode")]
    [SerializeField]
    GameObject[] vipPrefabs;
    [SerializeField]
    AssetReference[] vipAssets;

    new private void Awake()
    {
        base.Awake();

        GameEvents.instance.AddListener<CharacterCombatBase.PlayerSoakedEvt>(OnPlayerSoaked);
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            scores.Add(0);
            scores.Add(0);
        }
    }

    void OnPlayerSoaked(CharacterCombatBase.PlayerSoakedEvt e)
    {
        if (IsServer)
        {
            if (persistentPlayerRuntimeCollection.TryGetPlayer(e.damagingClientId, out var p))
            {
                GameOver(p.teamId.Value);
            }
        }
    }

    protected override IEnumerator SpawnPlayerAvatars()
    {
        bool asyncProcessComplete = false;
        var waitAsyncProcess = new WaitUntil(() => asyncProcessComplete);

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {

            if (persistentPlayerRuntimeCollection.TryGetPlayer(client.Value.ClientId, out var p))
            {
                Debug.Log($"playerId: {p.clientId} teamId: {p.teamId.Value}");
                GameObject avatar = null;

                asyncProcessComplete = false;

                if (p.isVip.Value)
                {
                    AddressablesManager.instance.InstantiateAsync(vipAssets[p.teamId.Value], (gameObject) =>
                    {
                        asyncProcessComplete = true;
                        avatar = gameObject;
                    });
                    //avatar = Instantiate(vipPrefabs[p.teamId.Value]);
                }
                else
                {
                    AddressablesManager.instance.InstantiateAsync(avatarAssets[p.teamId.Value], (gameObject) =>
                    {
                        asyncProcessComplete = true;
                        avatar = gameObject;
                    });
                    //avatar = Instantiate(teamAvatarPrefabs[p.teamId.Value]);
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

    protected override void SetupScores()
    {
        foreach (var client in persistentPlayerRuntimeCollection.items)
        {
            if (client.isVip.Value) hud.AddPlayerScore(client.teamId.Value, client.displayName);
        }
    }

    public override void AdjustScore(int value, ulong damagingClientId, ulong damagedClientId)
    {
        if (persistentPlayerRuntimeCollection.TryGetPlayer(damagedClientId, out var p))
        {
            scores[(int)p.teamId.Value] = Mathf.Min(_scoreLimit, scores[(int)p.teamId.Value] + value);
            hud.SetScoreBarProgress(p.teamId.Value, (float)scores[(int)p.teamId.Value] / (float)_scoreLimit);
        }
    }

    protected override void BeginSetupState()
    {
        if (IsServer)
        {
            var team1 = persistentPlayerRuntimeCollection.items.Where((x) => x.teamId.Value == 0);
            var team2 = persistentPlayerRuntimeCollection.items.Where((x) => x.teamId.Value == 1);

            var ran = Random.Range(0, team1.Count());
            var ran1 = Random.Range(0, team2.Count());
            team1.ElementAt(ran).isVip.Value = true;
            team2.ElementAt(ran1).isVip.Value = true;
        }

        base.BeginSetupState();
    }
}