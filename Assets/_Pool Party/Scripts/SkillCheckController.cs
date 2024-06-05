using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Daybrayk;
public class SkillCheckController : MonoBehaviour
{
	[SerializeField]
	Slider slider;
    [SerializeField]
    RectTransform completeArea;
    [SerializeField]
    ScriptableEventBool skillCheckEvent;
    bool active = false;
    Coroutine activeCoroutine;

    private void Update()
    {
        if (active && Keyboard.current.rKey.isPressed)
        {
            Debug.Log("R key pressed");
            StopCoroutine(activeCoroutine);
            active = false;

            Debug.Log("Handlrect parent width: " + slider.handleRect.GetComponentInParent<RectTransform>().rect.width);
            Debug.Log("Anchor Max: " + slider.handleRect.anchorMax.x);
            float handleMax = slider.handleRect.position.x + slider.handleRect.rect.xMax;
            float handleMin = slider.handleRect.position.x + slider.handleRect.rect.xMin;
            float areaMax = completeArea.position.x + completeArea.rect.xMax;
            float areaMin = completeArea.position.x + completeArea.rect.xMin;
            Debug.Log("Slider Max: " + handleMax + " Area Min: " + areaMin);
            Debug.Log("Slider Min: " + handleMin + " Area Max: " + areaMax);
            if (handleMax < areaMin || handleMin > areaMax)
            {
                Debug.Log("Skill check unsuccessful");
                skillCheckEvent.Value = false;
            }
            else
            {
                Debug.Log("Skill check successful");
                skillCheckEvent.Value = true;
            }

            completeArea.localPosition = Vector3.zero;
            slider.gameObject.SetActive(false);
        }
    }

    IEnumerator SkillCheckHelper()
	{
        yield return new WaitForSeconds(0.5f);
        active = true;
        WaitForFixedUpdate wfs = new WaitForFixedUpdate();
		while (slider.value < 1)
        {
            yield return wfs;
            slider.value += Time.fixedDeltaTime;
            
        }
        active = false;
        slider.gameObject.SetActive(false);
    }

    public void Activate()
    {
        Debug.Log("Activating skill check");

        if (!active)
        {
            slider.value = 0;

            completeArea.localPosition += Vector3.right * Random.Range(60, (((RectTransform)completeArea.parent).rect.width - 30));
            
            slider.gameObject.SetActive(true);
            activeCoroutine = StartCoroutine(SkillCheckHelper());
        }
    }

    public void Cancel()
    {
        Debug.Log("Canceling");
        active = false;
        completeArea.localPosition = Vector3.zero;
        slider.value = 0;
        slider.gameObject.SetActive(false);
        StopCoroutine(activeCoroutine);
    }
}

