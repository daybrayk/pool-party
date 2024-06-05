using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Netcode;
using Daybrayk;
public class FreeForAllGameMode : GameModeBase
{
    [SerializeField]
    protected GameObject playerAvatarPrefab;

    [SerializeField]
    protected AssetReference playerAvatarRef;

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
        AdjustScore(1, e.damagingClientId, e.owningClientId);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        localTeamNameText.text = localPlayer.displayName;
        if (IsServer)
        {
            var clientCount = NetworkManager.Singleton.ConnectedClientsList.Count;
            for (int i = 0; i < clientCount; i++)
            {
                scores.Add(0);
            }
        }
    }

    protected override IEnumerator SpawnPlayerAvatars()
    {
        bool asyncProcessComplete = false;
        var waitAsyncProcess = new WaitUntil(() => asyncProcessComplete);

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            PersistentPlayer p;
            persistentPlayerRuntimeCollection.TryGetPlayer(client.Value.ClientId, out p);
            GameObject avatar = null;
            
            asyncProcessComplete = false;

            AddressablesManager.instance.InstantiateAsync(playerAvatarRef, (gameObject) =>
            {
                asyncProcessComplete = true;
                avatar = gameObject;
            });

            yield return waitAsyncProcess;

            //GameObject avatar = Instantiate(playerAvatarPrefab);
            avatar.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.ClientId, true);

            avatar.GetComponent<CharacterInputHandler>().DisableInput();
        }
    }

    protected override void PlacePlayerAvatars()
    {
        var playerObjects = NetworkManager.Singleton.ConnectedClientsList;
        int j = 0;
        for (int i = 0; i < playerObjects.Count; i++, j++)
        {
            playerObjects[i].PlayerObject.GetComponent<CharacterRoot>().ResetToSpawnPositionClientRpc(spawnPoints[j].transform.position);
        }
    }

    protected override void SetupScores()
    {
        foreach (var client in persistentPlayerRuntimeCollection.items)
        {

            hud.AddPlayerScore(client.clientId, client.displayName);
        }
    }

    public override void AdjustScore(int value, ulong damagingClient, ulong damagedClient)
    {
        if (damagingClient == damagedClient) scores[(int)damagingClient] = Mathf.Max( scores[(int)damagingClient] - value, 0);
        else
        {
            scores[(int)damagingClient] += value;
        }

        hud.SetScoreText(damagingClient, scores[(int)damagingClient].ToString());

        if (scores[(int)damagingClient] >= _scoreLimit) GameOver(damagingClient);
    }
}