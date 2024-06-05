using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarWashGameMode : TeamGameMode
{
    [Header("Car Wash")]
    [SerializeField]
    CarController[] cars;

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
        scores[(int)damagingTeamId] += value;

        hud.SetScoreText(damagingTeamId, $"{scores[(int) damagingTeamId]}/{_scoreLimit}");

        if (scores[(int)damagingTeamId] >= _scoreLimit) GameOver(damagingTeamId);
    }

    protected override void Scores_OnListChanged(NetworkListEvent<int> changeEvent)
    {
        hud.SetScoreText((ulong) changeEvent.Index, $"{scores[changeEvent.Value]}/{_scoreLimit}");
    }

    protected override void BeginSetupState()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].Init((ulong)i, _scoreLimit);
        }

        base.BeginSetupState();
    }


}
