using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk.PersistentData
{
    public class SaveableEntity : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        string _id;
        public string id => _id;

        [ContextMenu("Generate GUId")]
        private void GenerateId() => _id = System.Guid.NewGuid().ToString();

	    public object GetState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (var saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.GetState();
            }

            return state;
        }

        public void SetState(object state)
        {
            var stateDictionary = (Dictionary<string, object>)state;

            foreach (var saveable in GetComponents<ISaveable>())
            {
                if(stateDictionary.TryGetValue(saveable.GetType().ToString(), out object value))
                {
                    saveable.SetState(value);
                }
            }
        }

        private void OnValidate()
        {
            if (gameObject.IsPrefabInstance()) _id = "";
            else if(string.IsNullOrEmpty(_id)) GenerateId();
        }
    }
}