using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
namespace Daybrayk
{
	public class NetworkScriptableEventListener : NetworkBehaviour, IScriptableEventListener
    {
        public UnityEvent onEventRaised;

        [SerializeField]
        protected ScriptableEvent scriptableEvent = null;
        [SerializeField]
        protected bool removeListenerOnDisable = true;
        public ScriptableEvent Event => scriptableEvent;

        protected void OnEnable()
        {
            if (!IsOwner) enabled = false;

            scriptableEvent.AddListener(this);
        }

        protected void OnDisable()
        {
            if(removeListenerOnDisable) scriptableEvent.RemoveListener(this);
        }

        new  protected void OnDestroy()
        {
            scriptableEvent.RemoveListener(this);
            base.OnDestroy();
        }

        public virtual void OnEventRaised(Object raiser)
        {
            Debug.Log($"[ScriptableEventListener.OnEventRaised] {scriptableEvent.name}");
            if (onEventRaised != null) onEventRaised.TryInvoke();
        }
    }
}