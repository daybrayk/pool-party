using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daybrayk;
[CreateAssetMenu(menuName = "")]
public class GameModeData : ScriptableObject
{
    [SerializeField]
    int playerCount;
    [SerializeField]
	int teamCount;
    [SerializeField]
    float gameDuration;
    [SerializeField]
    int scoreLimit;
}