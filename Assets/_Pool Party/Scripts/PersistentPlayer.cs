using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.Netcode;
using Daybrayk;

[RequireComponent(typeof(NetworkObject))]
public class PersistentPlayer : NetworkBehaviour
{
    public enum TeamColors
    {
        Blue,
        Green,
    }

    [SerializeField]
    PersistentPlayerRuntimeCollection persistentPlayerRuntimeCollection;
    //[SerializeField]
    //protected AssetReferenceT<PersistentPlayerRuntimeCollection> persistentPlayerRuntimeAsset;

    [SerializeField]
    [ReadOnly]
    public string displayName;

    [ReadOnly]
    public NetworkVariable<ulong> teamId;

    [ReadOnly]
    public NetworkVariable<bool> isVip;

    public string teamDisplayName => ((TeamColors)teamId.Value).ToString();
    public ulong clientId { get; private set; }

    //AsyncOperationHandle<PersistentPlayerRuntimeCollection> handle;

    private void Awake()
    {
        //handle = Addressables.LoadAssetAsync<PersistentPlayerRuntimeCollection>(persistentPlayerRuntimeAsset);
        //
        //handle.Completed += (operation) =>
        //{
        //    persistentPlayerRuntimeCollection = operation.Result;
        //};
        Debug.Log("Spawned persistent player");
        DontDestroyOnLoad(this);
    }

    public override void OnNetworkSpawn()
    {
        gameObject.name = "PersistentPlayer" + OwnerClientId;
        
        persistentPlayerRuntimeCollection.Add(this);
        clientId = GetComponent<NetworkObject>().OwnerClientId;
    }
    [ServerRpc]
    public void SetDisplayNameServerRpc(string name)
    {
        displayName = name;
    }

    [ClientRpc]
    public void SetDisplayNameClientRpc(string name)
    {
        if(IsServer) return;
        displayName = name;
    }
}
