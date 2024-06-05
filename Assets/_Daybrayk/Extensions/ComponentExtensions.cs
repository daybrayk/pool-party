using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
	public static class ComponentExtensions
	{
        public static bool TryGetComponentInChildren<T>(this Component obj, out T component)
        {
            component = obj.GetComponentInChildren<T>();

            return component != null;
        }
    }
}
