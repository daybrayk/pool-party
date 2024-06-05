using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
    public class ScriptableEventProperty<T> : ScriptableEvent
    {
        [SerializeField]
        protected T value;
        public T Value { get { return value; } set { this.value = value; Raise(this); } }
    }
}
