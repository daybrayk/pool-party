using UnityEngine;
using UnityEngine.Events;

namespace EquilibriumSystems.EventArchitecture
{
    [System.Serializable]
    public class IntGameEvent : UnityEvent<int> { }

    [System.Serializable]
    public class StringGameEvent : UnityEvent<string> { }

    public class GameEventListener : MonoBehaviour
    {
        public GameEvent Event;

        [Tooltip("The higher this value, the later it will be called")]
        public int Priority = 0;
        public UnityEvent Response;

        // TODO: Create a custom drawer like with variable so can choose which paramter to use (i.e zero, 1 int, 1 float, etc....)
        public IntGameEvent ResponseInt;

        public StringGameEvent ResponseString;

        private void OnEnable()
        {
            Event.RegisterListener(this, Priority);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response?.Invoke();
        }

        public void OnEventRaised(int val)
        {
            ResponseInt?.Invoke(val);
        }

        public void OnEventRaised(string val)
        {
            ResponseString?.Invoke(val);
        }
    }
}