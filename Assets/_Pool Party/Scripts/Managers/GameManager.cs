using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	GameManager _instance;
	public GameManager instance => _instance;

	List<CharacterRoot> players;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnSceneSetup()
    {

    }

    void OnSceneTeardown()
    {

    }
}