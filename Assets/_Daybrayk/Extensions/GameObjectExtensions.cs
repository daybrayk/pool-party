using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Daybrayk
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component)
        {
            component = obj.GetComponentInChildren<T>();

            return component != null;
        }

        public static bool IsPrefabInstance(this GameObject obj)
        {
            return (obj.scene.rootCount == 0 || obj.scene.name == null);
        }
    }
}