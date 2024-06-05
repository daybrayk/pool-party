using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Daybrayk
{
    public class PriorityStateStack<T>
    {
        SortedList<int, T> entries;

        T defaultValue;

        public T value
        {
            get
            {
                if (entries.Count <= 0) return defaultValue;

                return entries.Values[entries.Count-1];
            }
        }

        public PriorityStateStack() : this(default) { }
        public PriorityStateStack(T defaultValue)
        {
            entries = new SortedList<int, T>();
            this.defaultValue = defaultValue;
        }

        public void Add(T state, int priority)
        {
            if (entries.ContainsKey(priority)) return;

            entries.Add(priority, state);
        }

        public void Remove(int priority)
        {
            if (!entries.ContainsKey(priority)) return;

            entries.Remove(priority);
        }

        public void RemoveCurrent()
        {
            if(entries.Count > 0)
            {
                entries.RemoveAt(entries.Count - 1);
            }
        }
    }
}