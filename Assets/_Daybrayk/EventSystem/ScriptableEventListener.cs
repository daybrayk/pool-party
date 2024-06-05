using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Daybrayk
{
    public class ScriptableEventListener : MonoBehaviour, IScriptableEventListener
    {
        public UnityEvent onEventRaised;

        [SerializeField]
        protected ScriptableEvent scriptableEvent = null;
        public ScriptableEvent Event => scriptableEvent;

        protected void OnEnable()
        {
            scriptableEvent.AddListener(this);
        }

        protected void OnDestroy()
        {
            scriptableEvent.RemoveListener(this);
        }

        public virtual void OnEventRaised(Object raiser)
        {
            onEventRaised.TryInvoke();
        }
    }
}
