using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class CharacterFlagHolder : NetworkBehaviour
{
	[SerializeField]
	GameObject flagVisrep;

	[SerializeField]
	[ReadOnly]
	NetworkVariable<bool> _hasFlag;
	public bool hasFlag => _hasFlag.Value;
	public ulong heldFlagId => heldFlag.flagId;

	DroppedFlag heldFlag;
	CharacterRoot root;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
    }

    public override void OnNetworkSpawn()
    {
		_hasFlag.OnValueChanged += OnHasFlagUpdated;
    }

    public void TakeFlag(DroppedFlag flag)
    {
		_hasFlag.Value = true;
		heldFlag = flag;
		flagVisrep.SetActive(true);
    }

    void OnHasFlagUpdated(bool oldValue, bool newValue)
    {
		flagVisrep.SetActive(newValue);
    }

	public void RemoveFlag()
    {
		_hasFlag.Value = false;
        heldFlag = null;
		flagVisrep.SetActive(false);
    }

    public void DropFlag()
    {
		if (!IsServer || !hasFlag) return;
		Debug.Log($"Player {OwnerClientId} dropping flag");
		
		var mode = GameModeBase.instance as CaptureFlagMode;
		mode.FlagDropped(heldFlag.flagId, transform.position);

		RemoveFlag();
    }
}