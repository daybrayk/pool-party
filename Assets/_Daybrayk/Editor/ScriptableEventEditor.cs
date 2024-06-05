#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Daybrayk
{
    [CustomEditor(typeof(ScriptableEvent), true)]
    public class ScriptableEventEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            ScriptableEvent scriptableEvent = (ScriptableEvent)target;

            if(GUILayout.Button("Find Listeners in Scene"))
            {
                IEnumerable<MonoBehaviour> listeners = FindObjectsOfType<MonoBehaviour>().Where(x => x is IScriptableEventListener);
                for(int i = 0; i < listeners.Count(); i++)
                {
                    IScriptableEventListener listener = (IScriptableEventListener)listeners.ElementAt(i);
                    if(listener.Event == scriptableEvent)
                    {
                        Debug.Log("GameObject: " + listeners.ElementAt(i).gameObject, listeners.ElementAt(i).gameObject);
                    }
                }
            }

        }
    }
}
#endif