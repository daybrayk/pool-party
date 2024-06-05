using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EquilibriumSystems.EventArchitecture
{

    [CreateAssetMenu(menuName = "Equilibrium Systems/Event Architecture/Game Event")]
    public class GameEvent : ScriptableObject
    {
        struct Listeners
        {
            public GameEventListener listener;
            public int priority;
        };

        private List<Listeners> m_listeners = new List<Listeners>();

        //[Sirenix.OdinInspector.Button("Raise")]
        public void Raise()
        {
            Debug.Log("Game Event has been Raised: " + this.ToString());
            for (int i = 0; i < m_listeners.Count; ++i)
            {
                m_listeners[i].listener.OnEventRaised();
            }
        }

        public void Raise(int val)
        {
            Debug.Log("Game Event with int parameter " + val.ToString() + " has been Raised: " + this.ToString());
            for (int i = 0; i < m_listeners.Count; ++i)
            {
                m_listeners[i].listener.OnEventRaised(val);
            }
        }

        public void Raise(string val)
        {
            Debug.Log("Game Event with string parameter " + val + " has been Raised: " + this.ToString());
            for (int i = 0; i < m_listeners.Count; ++i)
            {
                m_listeners[i].listener.OnEventRaised(val);
            }
        }

        // marking internal so doesn't show up in inspector method name dropdowns
        internal void RegisterListener(GameEventListener listener, int Priority)
        {
            Listeners list = new Listeners();
            list.listener = listener;
            list.priority = Priority;

            bool bAdded = false;
            for (int i = 0; i < m_listeners.Count; ++i)
            {
                if (m_listeners[i].priority <= Priority)
                {
                    continue;
                }

                m_listeners.Insert(i, list);
                bAdded = true;
                break;
            }

            // Add to end
            if (bAdded == false)
            {
                m_listeners.Add(list);
            }
        }

        // marking internal so doesn't show up in inspector method name dropdowns
        internal void UnregisterListener(GameEventListener listener)
        {
            for (int i = 0; i < m_listeners.Count; ++i)
            {
                if (m_listeners[i].listener == listener)
                {
                    m_listeners.RemoveAt(i);
                }
            }
        }
    }
}