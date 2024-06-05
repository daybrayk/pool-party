using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SOBootstrapper : MonoBehaviour
{
    [SerializeField]
    protected List<AssetReference> referenceList;

    protected static readonly Dictionary<AssetReference, ScriptableObject> soDict = new Dictionary<AssetReference, ScriptableObject>();
    protected readonly List<AsyncOperationHandle<ScriptableObject>> asyncOperationHandles = new List<AsyncOperationHandle<ScriptableObject>>();

    public bool isSetup { get; protected set; }

    protected bool asyncProcessComplete = false;
}