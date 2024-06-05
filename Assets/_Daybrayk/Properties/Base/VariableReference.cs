using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk.Properties
{
    public abstract class VariableReference<T, R> where R : ScriptableVariable<T>
    {
        [SerializeField]
        protected bool useReference;
        [SerializeField]
        protected T value;
        [SerializeField]
        protected R reference;

        public T Value
        {
            get
            {
                if (useReference && reference != null) return reference.value;
                else
                {
                    return value;
                }
            }
            set
            {
                if (useReference && reference != null) reference.value = value;
                else
                {
                    this.value = value;
                }
            }
        }
    }
}
