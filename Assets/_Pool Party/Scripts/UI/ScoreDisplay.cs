using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreDisplay : MonoBehaviour
{
	[SerializeField]
	TMP_Text _displayName;
	public TMP_Text displayName => _displayName;
    [SerializeField]
    Slider _fillBar;
	public Slider fillBar => _fillBar;
    [SerializeField]
	TMP_Text _score;
	public TMP_Text score => _score;
}