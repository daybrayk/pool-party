using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Daybrayk;
public class HudController : MonoBehaviour
{
    [SerializeField]
	Transform scoreHolder;
	[SerializeField]
	GameObject scoreDisplayPrefab;
	[SerializeField]
	Dictionary<ulong, ScoreDisplay> scores = new Dictionary<ulong, ScoreDisplay>();
	[SerializeField]
	TMP_Text delayTimerText;
    [SerializeField]
    TMP_Text gameTimerText;
	[SerializeField]
	TMP_Text messageText;

	bool showingMessage;

    public void AddPlayerScore(ulong id, string displayName)
    {
		if (scores.ContainsKey(id)) return;

		GameObject newScore = Instantiate(scoreDisplayPrefab, scoreHolder);
		if (newScore.TryGetComponent(out ScoreDisplay component))
		{
			component.displayName.text = displayName;
			scores.Add(id, component);
		}
    }

	public void SetScoreText(ulong id, string text)
    {
		if (!scores.ContainsKey(id)) return;
		var s = scores[id];
		s.score.text = text;
    }

	public void SetScoreBarProgress(ulong id, float value)
    {
		if (!scores.ContainsKey(id)) return;

		var s = scores[id];
		s.fillBar.value = value;
    }

	public void SetDelayTimerText(string text)
    {
		delayTimerText.text = text;

    }

	public void SetGameTimerText(string text)
    {
		gameTimerText.text = text;
    }

	public void ShowMessage(string message, float duration = 5.0f)
    {
		if (showingMessage) return;

		messageText.text = message;

		StartCoroutine(MessageHelper(duration));
    }

	IEnumerator MessageHelper(float duration)
    {
		showingMessage = true;
		messageText.gameObject.SetActive(true);

		yield return new WaitForSeconds(duration);

		showingMessage = false;
		messageText.gameObject.SetActive(false);
    }
}