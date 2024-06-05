using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class TeamDeathmatchGameMode : TeamGameMode
{
    private void OnEnable()
    {
        GameEvents.instance.AddListener<CharacterCombatBase.PlayerSoakedEvt>(OnPlayerSoaked);
    }

    private void OnDisable()
    {
        GameEvents.instance.RemoveListener<CharacterCombatBase.PlayerSoakedEvt>(OnPlayerSoaked);
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
        AdjustScore(1, e.damagingClientId, e.owningClientId);
    }

    //public override void AdjustScore(int value, ulong damagingClientId, ulong damagedClientId)
    //{
    //    if (persistentPlayerRuntimeCollection.TryGetPlayer(damagingClientId, out PersistentPlayer damaging) && persistentPlayerRuntimeCollection.TryGetPlayer(damagedClientId, out PersistentPlayer damaged))
    //    {
    //        if (damaging.clientId == damagedClientId || damaging.teamId == damaged.teamId)
    //        {
    //            scores[(int)damaging.teamId.Value] = Mathf.Max(scores[(int)damaging.teamId.Value] - value, 0);
    //        }
    //        else
    //        {
    //            scores[(int)damaging.teamId.Value] += value;
    //        }
    //
    //        if (localPlayer.teamId == damaging.teamId) scoreText.text = scores[(int)damaging.teamId.Value].ToString();
    //
    //        if (scores[(int)damaging.teamId.Value] >= scoreLimit) GameOver(damaging.teamId.Value);
    //    }
    //}
}