using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daybrayk;
public class ScreenFader : MonoBehaviour
{
    public enum FadeState
    {
        Faded,
        NotFaded,
    }
	//[SerializeField]
    //[Tooltip("Prefab template for a default \"Black Screen\". Note: Must have a Canvas Group component attached")]
	//GameObject defaultBlackScreen;
    [SerializeField]
    [Tooltip("The default time it will take to fade to/from 1.0 alpha")]
    float fadeDuration = 1.0f;

    public FadeState currentState { get; private set; } = FadeState.Faded;
    Coroutine fadeCoroutine;
    [SerializeField]
    CanvasGroup canvasGroup;
    bool bCanDefaultFade = true;
    float step = 0.02f;

    private void Awake()
    {
        //Create overlay
        GameObject temp = new GameObject();
        GameEvents.instance.AddListener<ScreenFadeToBlackEvt>(FadeToBlack);
        GameEvents.instance.AddListener<ScreenFadeFromBlackEvt>(FadeFromBlack);
    }

    private void OnDisable()
    {
        GameEvents.instance.RemoveListener<ScreenFadeToBlackEvt>(FadeToBlack);
        GameEvents.instance.RemoveListener<ScreenFadeFromBlackEvt>(FadeFromBlack);
    }

    public void FadeToBlack(ScreenFadeToBlackEvt e)
    {
        Debug.Log("Fade to black");
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeHelper(1f));
    }

    public void FadeFromBlack(ScreenFadeFromBlackEvt e)
    {
        Debug.Log("Fade from black");
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeHelper(0));
    }

    IEnumerator FadeHelper(float alpha)
    {
        WaitForSeconds waitStep = new WaitForSeconds(step);
        float timer = 0;
        while (canvasGroup.alpha != alpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, alpha, timer/fadeDuration);

            yield return waitStep;

            timer += step;
        }
    }

    public class ScreenFadeToBlackEvt : GameEvent { }
    public class ScreenFadeFromBlackEvt : GameEvent { }
}