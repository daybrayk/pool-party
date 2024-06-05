using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class AddressablesManager : MonoBehaviour
{
	public static AddressablesManager instance { get; private set; }

    readonly Dictionary<AssetReference, List<GameObject>> spawnedGameObjects = new Dictionary<AssetReference, List<GameObject>>();
    readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> asyncOperationHandles = new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public void LoadAssetAsync(AssetReference assetRef, System.Action callback)
    {
        StartCoroutine(CR_LoadAsset(assetRef, callback));
    }

    IEnumerator CR_LoadAsset(AssetReference assetRef, System.Action callback)
    {
        bool asyncProcessComplete = false;
        var op = Addressables.LoadAssetAsync<GameObject>(assetRef);
        asyncOperationHandles[assetRef] = op;

        op.Completed += (operation) =>
        {
            asyncProcessComplete = true;
        };

        //Wait for asset loaded
        yield return new WaitUntil(() => asyncProcessComplete);

        callback.Invoke();
    }

    public void ReleaseAsset(AssetReference assetRef, System.Action callback)
    {
        if (spawnedGameObjects[assetRef].Count != 0)
        {
            var spawnedObjects = spawnedGameObjects[assetRef];
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                Addressables.ReleaseInstance(spawnedObjects[i].gameObject);
            }
        }

        if (asyncOperationHandles[assetRef].IsValid())
        {
            Debug.Log($"Operation Handle is valid, releasing asset from memory");
            Addressables.Release(asyncOperationHandles[assetRef]);
        }

        asyncOperationHandles.Remove(assetRef);
    }

    void ReleaseInstantiatedObj(AssetReference asset, UnloadAssetOnDestroy obj)
    {
        Debug.Log($"Releasing asset: {obj.name}");
        
        Addressables.ReleaseInstance(obj.gameObject);

        if (!spawnedGameObjects[asset].Contains(obj.gameObject))
        {
            Debug.LogError($"Object: \"{obj.gameObject.name}\" was not associated with the provided AssetReference: \"{asset}\"");
            return;
        }

        spawnedGameObjects[asset].Remove(obj.gameObject);

        if (spawnedGameObjects[asset].Count == 0)
        {
            Debug.Log($"Removed all {obj.name}. RuntimeKey: {asset.RuntimeKey}");

            if (asyncOperationHandles[asset].IsValid())
            {
                Debug.Log($"Operation Handle is valid, releasing asset from memory");
                Addressables.Release(asyncOperationHandles[asset]);
            }

            asyncOperationHandles.Remove(asset);
        }
    }

    public class TransformSpawnInfo
    {
        public Transform transform;
        public System.Action<GameObject> callback;

        public TransformSpawnInfo(Transform parent, System.Action<GameObject> action)
        {
            transform = parent;
            callback = action;
        }
    }

    public class PositionSpawnInfo
    {
        public Vector3 pos;
        public Quaternion rot;
        public System.Action<GameObject> callback;

        public PositionSpawnInfo(Vector3 position, Quaternion rotation, System.Action<GameObject> action)
        {
            pos = position;
            rot = rotation;
            callback = action;
        }
    }
}