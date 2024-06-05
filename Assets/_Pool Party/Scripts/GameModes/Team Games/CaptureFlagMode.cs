using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daybrayk;
using Unity.Netcode;
public class CaptureFlagMode : TeamGameMode
{
    [SerializeField]
    Transform[] flagOrigins;
    [SerializeField]
    FlagController[] flags;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            scores.Add(0);
            scores.Add(0);
        }
    }

    public override void AdjustScore(int value, ulong damagingClientId, ulong damagedClientId) { }

    public override void AdjustScore(int value, ulong damagingTeamId)
    {
        var newScore = (scores[(int)damagingTeamId] += value);

        hud.SetScoreText(damagingTeamId,newScore.ToString());

        if (newScore > _scoreLimit) GameOver(damagingTeamId);
    }

    public void FlagTaken(ulong flagId, ulong clientId)
    {
        if(!IsServer) return;

        int flagIndex = (int)flagId;
        if (flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was taken", 3f);
        else
        {
            hud.ShowMessage($"{((PersistentPlayer.TeamColors)flagId)} flag taken", 3f);
        }
        flags[flagIndex].SetFlagStatus(FlagController.FlagStatus.Taken);

        FlagTakenClientRpc(flagId);
    }

    [ClientRpc]
    void FlagTakenClientRpc(ulong flagId)
    {
        if (IsServer) return;

        int flagIndex = (int)flagId;
        if (flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was taken", 3f);
        else
        {
            hud.ShowMessage($"{((PersistentPlayer.TeamColors)flagId)} flag taken", 3f);
        }
    }

    public void FlagDropped(ulong flagId, Vector2 dropPosition)
    {
        if (!IsServer) return;

        int flagIndex = (int)flagId;
        if (flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was dropped", 3f);
        else
        {
            hud.ShowMessage($"{((PersistentPlayer.TeamColors)flagId)} flag dropped", 3f);
        }
        flags[flagIndex].SetFlagStatus(FlagController.FlagStatus.Dropped);

        flags[flagIndex].SetFlagDropPosition(dropPosition);

        FlagDroppedClientRpc(flagId);
    }

    [ClientRpc]
    void FlagDroppedClientRpc(ulong flagId)
    {
        if (IsServer) return;
        
        //int flagIndex = (int)flagId;
        if (flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was dropped", 3f);
        else
        {
            hud.ShowMessage($"{((PersistentPlayer.TeamColors)flagId)} flag dropped", 3f);
        }
    }

    public void FlagReturned(ulong flagId)
    {
        if(!IsServer) return;

        int flagIndex = (int)flagId;
        if(flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was returned", 3f);
        else
        {
            hud.ShowMessage($"{((PersistentPlayer.TeamColors)flagId)} flag returned", 3f);
        }
        flags[flagIndex].SetFlagStatus(FlagController.FlagStatus.Returned);
        FlagReturnedClientRpc(flagId);
    }

    [ClientRpc]
    void FlagReturnedClientRpc(ulong flagId)
    {
        if(IsServer) return;

        if (flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was returned", 3f);
        else
        {
            hud.ShowMessage($"{((PersistentPlayer.TeamColors)flagId)} flag returned", 3f);
        }
    }
    
    public void FlagCaptured(ulong flagId, ulong collectingTeamId)
    {
        if(!IsServer) return;

        int flagIndex = (int)flagId;
        if (flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was captured", 3f);
        else
        {
            hud.ShowMessage($"Your team captured the flag", 3f);
        }
        AdjustScore(1, collectingTeamId);
        flags[flagIndex].SetFlagStatus(FlagController.FlagStatus.Returned);
        FlagCapturedClientRpc(flagId, collectingTeamId);
    }

    [ClientRpc]
    void FlagCapturedClientRpc(ulong flagId, ulong collectingTeamId)
    {
        if (IsServer) return;

        int flagIndex = (int)flagId;
        if (flagId == localPlayer.teamId.Value) hud.ShowMessage($"Your flag was captured", 3f);
        else
        {
            hud.ShowMessage($"Your team captured the flag", 3f);
        }
    }

    //public class FlagEvent : GameEvent
    //{
    //    public enum FlagEventType
    //    {
    //        Taken,
    //        Dropped,
    //        Returned,
    //        Captured,
    //    }
    //
    //    public ulong flagId;
    //    public ulong collectingTeamId;
    //    public ulong carrierId;
    //
    //    public FlagEventType type;
    //
    //    public Vector2 dropPosition;
    //}
}