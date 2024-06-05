using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class ClientCharacterRoot : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if(!IsClient)
        {
            enabled = false;
            return;
        }
    }
}