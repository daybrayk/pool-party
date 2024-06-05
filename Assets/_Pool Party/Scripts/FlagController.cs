using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FlagController : NetworkBehaviour
{
    public enum FlagStatus
    {
        Returned,
        Taken,
        Dropped,
    }

    [SerializeField]
    DroppedFlag flag;
	[SerializeField]
	ulong teamId;
    [SerializeField]
    GameObject flagVisrep;
    NetworkVariable<FlagStatus> flagStatus = new NetworkVariable<FlagStatus>();

    private void Awake()
    {
        flag.flagId = teamId;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsServer) flagStatus.OnValueChanged += OnFlagStatusUpdated;
    }

    public void SetFlagStatus(FlagStatus newStatus)
    {
        flagStatus.Value = newStatus;
        switch (newStatus)
        {
            case FlagStatus.Returned:
                flagVisrep.SetActive(true);
                flag.HideFlag();
                break;
            case FlagStatus.Taken:
                flagVisrep.SetActive(false);
                flag.HideFlag();
                break;
            case FlagStatus.Dropped:
                flag.ShowFlag();
                break;
        }
    }

    void OnFlagStatusUpdated(FlagStatus oldValue, FlagStatus newValue)
    {
        switch (newValue)
        {
            case FlagStatus.Returned:
                flagVisrep.SetActive(true);
                flag.HideFlag();
                break;
            case FlagStatus.Taken:
                flagVisrep.SetActive(false);
                flag.HideFlag();
                break;
            case FlagStatus.Dropped:
                flag.ShowFlag();
                break;
        }
    }

    public void SetFlagDropPosition(Vector2 pos)
    {
        Debug.Log($"Setting drop position to {pos}");
        flag.transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (!IsServer) return;

        if (c.CompareTag("Player"))
        {
            if (c.TryGetComponent(out CharacterRoot root))
            {
                var mode = GameModeBase.instance as CaptureFlagMode;

                if (root.owningPlayer.teamId.Value == teamId && root.flag.hasFlag && flagStatus.Value == FlagStatus.Returned)
                {
                    mode.FlagCaptured(root.flag.heldFlagId, teamId);
                    root.flag.RemoveFlag();
                }
                else if (root.owningPlayer.teamId.Value != teamId)
                {
                    mode.FlagTaken(teamId, root.owningPlayer.clientId);
                    root.flag.TakeFlag(flag);
                }
            }
        }
    }
}