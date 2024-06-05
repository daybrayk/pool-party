using UnityEngine;
using UnityEngine.Events;

namespace Daybrayk
{
    public static class UnityEventExtensions
    {
        public static void TryInvoke(this UnityEvent evt)
        {
            if (evt != null) evt.Invoke();
        }

        public static void TryInvoke(this UnityEventInt evt, int value)
        {
            if (evt != null) evt.Invoke(value);
        }

        public static void TryInvoke(this UnityEventFloat evt, float value)
        {
            if (evt != null) evt.Invoke(value);
        }

        public static void TryInvoke(this UnityEventString evt, string value)
        {
            if (evt != null) evt.Invoke(value);
        }

        public static void TryInvoke(this UnityEventBool evt, bool value)
        {
            if (evt != null) evt.Invoke(value);
        }

        public static void TryInvoke(this UnityEventGameObject evt, GameObject value)
        {
            if (evt != null) evt.Invoke(value);
        }

        public static void TryInvoke(this UnityEventTransform evt, Transform value)
        {
            if (evt != null) evt.Invoke(value);
        }
    }

    [System.Serializable]
    public class UnityEventInt : UnityEvent<int> { }

    [System.Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [System.Serializable]
    public class UnityEventString : UnityEvent<string> { }

    [System.Serializable]
    public class UnityEventBool : UnityEvent<bool> { }

    [System.Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject> { }

    [System.Serializable]
    public class UnityEventTransform : UnityEvent<Transform> { }
}
