using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Daybrayk
{
    public class OnEnableEvent : MonoBehaviour
    {
        public UnityEvent onEnable;

        private void OnEnable()
        {
            onEnable.TryInvoke();
        }
    }
}