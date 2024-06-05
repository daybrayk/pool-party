using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;

public class PlayerSpawn : NetworkBehaviour
{
	[SerializeField]
	ulong _teamId;
	public ulong teamId => _teamId;

	[SerializeField]
	[ReadOnly]
	NetworkVariable<ulong> clientId = new NetworkVariable<ulong>();
	public ulong assignedClient => clientId.Value;
	public bool hasAssignedClient { get; private set; } = false;
    
	public override void OnNetworkSpawn()
	{
		clientId.OnValueChanged += OnClientIdUpdated;
	}

	public void AssignClient(ulong id)
    {
		clientId.Value = id;
		hasAssignedClient = true;
    }

	[ServerRpc(RequireOwnership = false)]
	public void AssignClientServerRpc(ulong id)
    {
		AssignClient(id);
    }

	public void RemoveAssignedClient()
    {
		hasAssignedClient = false;
    }

	[ServerRpc]
	public void RemoveAssignedClientServerRpc()
    {
		RemoveAssignedClient();
    }

    void OnClientIdUpdated(ulong oldValue, ulong newValue)
    {
		hasAssignedClient = true;
    }

    private void OnValidate()
    {
		_teamId = (ulong)Mathf.Max(_teamId, 0);
    }
}