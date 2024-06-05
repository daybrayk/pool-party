using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionHelperClasses
{
    #region Canvas Fading
    public class FadeGeneric<T> where T : Component
    {
        struct FadeData
        {
            Coroutine coroutine;
            System.Action OnStartCallback;
            System.Action OnEndCallback;
        }

        Dictionary<T, FadeData> dict = new Dictionary<T, FadeData>();

        public void FadeTo()
        {

        }
    }
    #endregion
}