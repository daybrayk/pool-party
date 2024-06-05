using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
    [CreateAssetMenu(menuName = "Daybrayk/Scriptable Event")]
    public class ScriptableEvent : ScriptableObject
    {
        private List<IScriptableEventListener> listeners = new List<IScriptableEventListener>();

        public void Raise()
        {
            Raise(null);
            
        }

        public void Raise(Object raiser)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnEventRaised(raiser);
            }
        }

        public void AddListener(IScriptableEventListener e)
        {
            if (listeners.Contains(e)) return;
            
            listeners.Add(e);
        }

        public void RemoveListener(IScriptableEventListener e)
        {
            if (listeners.Contains(e)) listeners.Remove(e);
        }
    }


    public interface IScriptableEventListener
    {
        ScriptableEvent Event { get; }
        void OnEventRaised(Object raiser);
    }
}
