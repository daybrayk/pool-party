using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Netcode;

public class TeamGameMode : GameModeBase
{
    [Header("Team Mode")]
    [SerializeField]
    protected GameObject[] teamAvatarPrefabs;
    [SerializeField]
    protected AssetReference[] avatarAssets;

    [SerializeField]
    protected List<TeamSpawnList> teamSpawns;

    protected override IEnumerator SpawnPlayerAvatars()
    {
        bool asyncProcessComplete = false;
        var waitAsyncProcess = new WaitUntil(() => asyncProcessComplete);

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            GameObject avatar = null;
            PersistentPlayer p;
            persistentPlayerRuntimeCollection.TryGetPlayer(client.Value.ClientId, out p);

            asyncProcessComplete = false;

            AddressablesManager.instance.InstantiateAsync(avatarAssets[p.teamId.Value], (gameObject) =>
            {
                asyncProcessComplete = true;
                avatar = gameObject;
            });

            yield return waitAsyncProcess;

            //GameObject avatar = Instantiate(teamAvatarPrefabs[p.teamId.Value]);
            avatar.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.ClientId, true);

            avatar.GetComponent<CharacterInputHandler>().DisableInput();
        }
    }

    protected override void PlacePlayerAvatars()
    {
        var playerObjects = NetworkManager.Singleton.ConnectedClientsList;
        int k = 0;

        for (int j = 0; j < teamSpawns.Count; j++)
        {
            k = 0;
            for (int i = 0; i < playerObjects.Count; i++)
            {
                if (persistentPlayerRuntimeCollection.TryGetPlayer(playerObjects[i].ClientId, out PersistentPlayer persistent))
                {
                    if (persistent.teamId.Value == (ulong)j)
                    {
                        playerObjects[i].PlayerObject.GetComponent<CharacterRoot>().ResetToSpawnPositionClientRpc(teamSpawns[j].spawnPoints[k].transform.position);
                        k++;
                    }
                }
            }
        }
    }

    protected override void SetupScores()
    {
        foreach (var client in persistentPlayerRuntimeCollection.items)
        {

            if (localPlayer.teamId.Value == client.teamId.Value) hud.AddPlayerScore(client.teamId.Value, client.teamDisplayName + "(You)");
            else
            {
                hud.AddPlayerScore(client.teamId.Value, client.teamDisplayName);
            }
        }
    }

    public override void AdjustScore(int value, ulong damagingClientId, ulong damagedClientId)
    {
        if (persistentPlayerRuntimeCollection.TryGetPlayer(damagingClientId, out PersistentPlayer damaging) && persistentPlayerRuntimeCollection.TryGetPlayer(damagedClientId, out PersistentPlayer damaged))
        {
            scores[(int)damaging.teamId.Value] += value;

            hud.SetScoreText(damaging.teamId.Value, scores[(int)damaging.teamId.Value].ToString());

            if (scores[(int)damaging.teamId.Value] >= _scoreLimit) GameOver(damaging.teamId.Value);
        }
    }

    public override bool TryFindAvailableSpawnPoint(PersistentPlayer p, out PlayerSpawn spawnPoint)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].hasAssignedClient && spawnPoints[i].assignedClient == p.clientId)
            {
                spawnPoint = spawnPoints[i];
                return true;
            }
            else if (!spawnPoints[i].hasAssignedClient && spawnPoints[i].teamId == p.teamId.Value)
            {
                spawnPoint = spawnPoints[i];
                spawnPoint.AssignClient(p.clientId);
                return true;
            }
        }

        spawnPoint = null;
        return false;
    }
    [System.Serializable]
    public struct TeamSpawnList
    {
        public List<PlayerSpawn> spawnPoints;
    }
}
