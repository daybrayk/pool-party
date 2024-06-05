using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ResponseModal : MonoBehaviour
{
	[SerializeField]
	TMP_InputField playerName;
	[SerializeField]
	TMP_InputField roomName;

	System.Action<string, string> confirmCallback;
	public void Confirm()
    {
		confirmCallback?.Invoke(playerName.text, roomName.text);
		gameObject.SetActive(false);
    }

	public void SetupCallback(System.Action<string, string> action)
    {
		confirmCallback = action;
    }
}