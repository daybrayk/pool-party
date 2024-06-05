using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UnloadAssetOnDestroy : MonoBehaviour
{
    public event System.Action<AssetReference, UnloadAssetOnDestroy> Destroyed;
    public AssetReference assetReference { get; set; }
    private void OnDestroy()
    {
        Destroyed?.Invoke(assetReference, this);
    }
}