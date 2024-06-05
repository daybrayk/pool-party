using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class NetworkDisable : NetworkBehaviour
{
    public enum NetworkStatus
    {
        Server,
        NotServer,
    }
    [Tooltip("Disable the gameObject if it's Client/Server status matches this value")]
    public NetworkStatus networkStatus;


    public override void OnNetworkSpawn()
    {
        switch (networkStatus)
        {
            case NetworkStatus.Server:
                if (IsServer) gameObject.SetActive(false);
                break;
            case NetworkStatus.NotServer:
                if (!IsServer) gameObject.SetActive(false);
                break;
        }
    }
}
