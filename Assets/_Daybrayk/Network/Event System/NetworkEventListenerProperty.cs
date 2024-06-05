using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
namespace Daybrayk
{
    [RequireComponent(typeof(NetworkObject))]
	public class NetworkEventListenerProperty<T, TEvent> : NetworkBehaviour, IScriptableEventListener where TEvent : UnityEvent<T>, new()
    {
        public TEvent onPropertyChanged;

        [SerializeField]
        ScriptableEvent _scriptableProperty;

        [SerializeField]
        protected bool executeOnNetworkSpawn = true;
        [SerializeField]
        protected bool removeListenerOnDisable = true;

        public ScriptableEvent Event => _scriptableProperty;

        public ScriptableEvent scriptableProperty
        {
            get { return _scriptableProperty; }

            set
            {
                if (scriptableProperty) scriptableProperty.RemoveListener(this);
                _scriptableProperty = value;
                if (scriptableProperty) scriptableProperty.AddListener(this);
            }
        }

        public void Raise()
        {
            OnEventRaised(scriptableProperty);
        }

        public void OnEventRaised(Object raiser)
        {
            if (onPropertyChanged != null) onPropertyChanged.Invoke((raiser as ScriptableEventProperty<T>).Value);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) enabled = false;

            scriptableProperty.AddListener(this);
            if (executeOnNetworkSpawn) Raise();
        }

        private void OnDisable()
        {
            if(removeListenerOnDisable) scriptableProperty.RemoveListener(this);
        }
    }
}