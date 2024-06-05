using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PersistentSOBootstrapper : SOBootstrapper
{
    public static PersistentSOBootstrapper instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(Init());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Init()
    {
        var waitProcessComplete = new WaitUntil(() => asyncProcessComplete);

        for (int i = 0; i < referenceList.Count; i++)
        {
            asyncProcessComplete = false;

            var op = Addressables.LoadAssetAsync<ScriptableObject>(referenceList[i]);
            asyncOperationHandles.Add(op);

            op.Completed += (operation) =>
            {
                soDict.Add(referenceList[i], operation.Result);
            };

            yield return waitProcessComplete;
        }

        isSetup = true;
    }

    public bool TryGetSOFromReferenc(AssetReference assetRef, out ScriptableObject outValue)
    {
        if (soDict.ContainsKey(assetRef))
        {
            outValue = soDict[assetRef];
            return true;
        }

        outValue = null;

        return false;
    }

    private void OnDestroy()
    {
        ReleaseAssets();   
    }

    void ReleaseAssets()
    {
        for (int i = 0; i < asyncOperationHandles.Count; i++)
        {
            Addressables.Release(asyncOperationHandles[i]);
        }

        soDict.Clear();
    }
}