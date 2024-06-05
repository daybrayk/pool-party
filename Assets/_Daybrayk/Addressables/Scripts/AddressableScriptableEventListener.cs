using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Daybrayk
{
    public class AddressableScriptableEventListener : MonoBehaviour, IScriptableEventListener
    {
        public UnityEvent onEventRaised;

        [SerializeField]
        protected AssetReferenceT<ScriptableEvent> scriptableEventAsset = null;

        ScriptableEvent scriptableEvent;
        public ScriptableEvent Event => scriptableEvent;

        bool isSetup = false;

        AsyncOperationHandle<ScriptableEvent> handle;

        protected void OnEnable()
        {
            if (!isSetup)
            {
                handle = Addressables.LoadAssetAsync<ScriptableEvent>(scriptableEventAsset);

                handle.Completed += (operation) =>
                {
                    isSetup = true;
                    scriptableEvent = operation.Result;
                    scriptableEvent.AddListener(this);
                };

                return;
            }

            scriptableEvent.AddListener(this);
        }

        protected void OnDestroy()
        {
            if (isSetup)
            {
                scriptableEvent.RemoveListener(this);

                Addressables.Release(handle);
            }
        }

        public virtual void OnEventRaised(Object raiser)
        {
            onEventRaised.TryInvoke();
        }
    }
}