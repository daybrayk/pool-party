using System.Collections;
using UnityEngine;

public static class CanvasGroupExtensions
{


	public static void Fade(this CanvasGroup group, float desiredAlpha, float duration)
    {
        
    }

    public static void StopFade(this CanvasGroup group)
    {

    }

    public static IEnumerator FadeHelper(CanvasGroup group, float desiredAlpha, float duration)
    {
        float accumulator = 0;
        while(group.alpha != desiredAlpha)
        {
            group.alpha = Mathf.Lerp(group.alpha, desiredAlpha, accumulator / duration);

            yield return new WaitForSeconds(Time.deltaTime);
            accumulator += Time.deltaTime;
        }    
    }
}