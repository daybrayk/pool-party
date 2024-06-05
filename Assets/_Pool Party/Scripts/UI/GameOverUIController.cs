using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameOverUIController : MonoBehaviour
{
	[SerializeField]
	CanvasGroup canvasGroup;
	[SerializeField]
	TMP_Text winnerText;

	public void ShowUI()
    {
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
    }

	public void HideUI()
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	public void SetWinnerText(string value)
    {
		Debug.Log($"Setting winner text to: {value}");
		winnerText.text = value;
    }
}