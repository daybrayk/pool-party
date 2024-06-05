using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeCollection<T> : ScriptableObject
{
	public List<T> items = new List<T>();

	public event System.Action<T> onItemAdded;
	public event System.Action<T> onItemRemoved;

	public void Add(T item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            onItemAdded?.Invoke(item);
        }

    }

    public void Remove(T item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            onItemRemoved?.Invoke(item);
        }
    }

    public void Clean()
    {
        for (int i = items.Count-1; i >= 0; i--)
        {
            if (items[i] == null) items.RemoveAt(i);
        }
    }

    public void Clear()
    {
        items.Clear();
    }
}