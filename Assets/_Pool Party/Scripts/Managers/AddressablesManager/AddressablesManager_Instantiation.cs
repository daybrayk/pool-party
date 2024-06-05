using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class AddressablesManager
{
    readonly Dictionary<AssetReference, Queue<TransformSpawnInfo>> queuedTransformSpawnRequests = new Dictionary<AssetReference, Queue<TransformSpawnInfo>>();
    readonly Dictionary<AssetReference, Queue<PositionSpawnInfo>> queuedPositionSpawnRequests = new Dictionary<AssetReference, Queue<PositionSpawnInfo>>();

    public void InstantiateAsync(AssetReference assetRef, System.Action<GameObject> callback)
    {
        InstantiateAsync(assetRef, Vector3.zero, Quaternion.identity, callback);
    }

    public void InstantiateAsync(AssetReference assetRef, Vector3 pos, System.Action<GameObject> callback)
    {
        InstantiateAsync(assetRef, pos, Quaternion.identity, callback);
    }

    public void InstantiateAsync(AssetReference assetRef, Quaternion rot, System.Action<GameObject> callback)
    {
        InstantiateAsync(assetRef, Vector3.zero, rot, callback);
    }

    public void InstantiateAsync(AssetReference assetRef, Vector3 pos, Quaternion rot, System.Action<GameObject> callback)
    {
        StartCoroutine(CR_InstantiateAsync(assetRef, pos, rot, callback));
    }

    IEnumerator CR_InstantiateAsync(AssetReference assetRef, Vector3 pos, Quaternion rot, System.Action<GameObject> callback)
    {
        if (!assetRef.RuntimeKeyIsValid())
        {
            Debug.LogError($"Ivalid Key {assetRef.RuntimeKey}");
            yield break;
        }

        bool asyncProcessComplete = false;

        //Check if there is already a handle to the Asset Reference
        if (!asyncOperationHandles.ContainsKey(assetRef))
        {
            //var op = Addressables.LoadAssetAsync<GameObject>(assetRef);
            //asyncOperationHandles[assetRef] = op;
            //
            //op.Completed += (operation) =>
            //{
            //    asyncProcessComplete= true;
            //};
            //
            ////Wait for asset loaded
            //yield return new WaitUntil(() => asyncProcessComplete);

            AsyncOperationHandle<GameObject> handle = default;
            
            assetRef.InstantiateAsync(pos, rot).Completed += (asyncOperation) =>
            {
                handle = asyncOperation;
                asyncProcessComplete = true;
                
            };

            //Wait for object instantiated
            yield return new WaitUntil(() => asyncProcessComplete);

            asyncOperationHandles[assetRef] = handle;
            AddToSpawnedObjects(assetRef, handle.Result);

            callback.Invoke(handle.Result);
        }
        else if (asyncOperationHandles[assetRef].IsDone)
        {
            //AsyncOperationHandle<GameObject> handle = default;
            //
            //assetRef.InstantiateAsync(pos, rot).Completed += (asyncOperation) =>
            //{
            //    handle = asyncOperation;
            //    asyncProcessComplete = true;
            //};
            //
            ////Wait for object instantiated
            //yield return new WaitUntil(() => asyncProcessComplete);

            var handle = asyncOperationHandles[assetRef];

            var go = Instantiate(handle.Result, pos, rot);
            callback.Invoke(go);

            AddToSpawnedObjects(assetRef, go);
        }
        else
        {
            if (queuedPositionSpawnRequests.ContainsKey(assetRef) == false)
            {
                queuedPositionSpawnRequests[assetRef] = new Queue<PositionSpawnInfo>();
            }
            queuedPositionSpawnRequests[assetRef].Enqueue(new PositionSpawnInfo(pos, rot, callback));
        }
    }

    void AddToSpawnedObjects(AssetReference assetRef, GameObject go)
    {
        if (!spawnedGameObjects.ContainsKey(assetRef))
        {
            spawnedGameObjects[assetRef] = new List<GameObject>();
        }

        spawnedGameObjects[assetRef].Add(go);

        var notify = go.AddComponent<UnloadAssetOnDestroy>();
        notify.Destroyed += ReleaseInstantiatedObj;
        notify.assetReference = assetRef;
    }

    public void InstantiateAsync(AssetReference assetRef, Transform parent, System.Action<GameObject> callback)
    {
        if (!assetRef.RuntimeKeyIsValid())
        {
            Debug.LogError($"Ivalid Key {assetRef.RuntimeKey}");
            return;
        }

        //Check if there is already a handle to the Asset Reference
        if (!asyncOperationHandles.ContainsKey(assetRef))
        {
            var op = Addressables.LoadAssetAsync<GameObject>(assetRef);
            asyncOperationHandles[assetRef] = op;

            op.Completed += (operation) =>
            {
                Debug.Log($"Instantiating object: {assetRef.SubObjectName}");
                assetRef.InstantiateAsync(parent).Completed += (asyncOperation) =>
                {
                    if (!spawnedGameObjects.ContainsKey(assetRef))
                    {
                        spawnedGameObjects[assetRef] = new List<GameObject>();
                    }

                    spawnedGameObjects[assetRef].Add(asyncOperation.Result);

                    var notify = asyncOperation.Result.AddComponent<UnloadAssetOnDestroy>();
                    notify.Destroyed += ReleaseInstantiatedObj;
                    notify.assetReference = assetRef;

                    callback.Invoke(asyncOperation.Result);
                };

                if (queuedTransformSpawnRequests.ContainsKey(assetRef))
                {
                    while (queuedTransformSpawnRequests[assetRef]?.Count > 0)
                    {
                        var info = queuedTransformSpawnRequests[assetRef].Dequeue();
                        assetRef.InstantiateAsync(info.transform).Completed += (asyncOperation) =>
                        {
                            if (!spawnedGameObjects.ContainsKey(assetRef))
                            {
                                spawnedGameObjects[assetRef] = new List<GameObject>();
                            }

                            spawnedGameObjects[assetRef].Add(asyncOperation.Result);

                            var notify = asyncOperation.Result.AddComponent<UnloadAssetOnDestroy>();
                            notify.Destroyed += ReleaseInstantiatedObj;
                            notify.assetReference = assetRef;

                            info.callback.Invoke(asyncOperation.Result);
                        };
                    }
                }
            };
        }
        //If we have already loaded the asset into memory
        else if (asyncOperationHandles[assetRef].IsDone)
        {
            assetRef.InstantiateAsync(parent).Completed += (asyncOperation) =>
            {
                if (!spawnedGameObjects.ContainsKey(assetRef))
                {
                    spawnedGameObjects[assetRef] = new List<GameObject>();
                }

                spawnedGameObjects[assetRef].Add(asyncOperation.Result);

                var notify = asyncOperation.Result.AddComponent<UnloadAssetOnDestroy>();
                notify.Destroyed += ReleaseInstantiatedObj;
                notify.assetReference = assetRef;

                callback.Invoke(asyncOperation.Result);
            };
        }
        else
        {
            if (queuedTransformSpawnRequests.ContainsKey(assetRef) == false)
            {
                queuedTransformSpawnRequests[assetRef] = new Queue<TransformSpawnInfo>();
            }
            queuedTransformSpawnRequests[assetRef].Enqueue(new TransformSpawnInfo(parent, callback));
        }
    }
}
