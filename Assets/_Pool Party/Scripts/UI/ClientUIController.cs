using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Daybrayk;

public class ClientUIController : MonoBehaviour
{
    [SerializeField]
    Color[] teamColors;

    [SerializeField]
    TMP_Text clientName;
    [SerializeField]
    TMP_Text clientStatus;
    [SerializeField]
    GameObject nextTeamBtn;
    [SerializeField]
    GameObject prevTeamBtn;
    [SerializeField]
    Image backgroundImage;
    
    [SerializeField]
    [ReadOnly]
    int team;

    public bool isReady { get; private set; } = false;
    public string displayName => clientName.text;

    private void Start()
    {
        if (!isReady) clientStatus.text = "Not Ready";
        else
        {
            clientStatus.text = "Ready";
        }
    }

    public void SetClientName(string value)
    {
        clientName.text = value;
    }

    public void SetClientStatus(bool value)
    {
        if (value)
        {
            Debug.Log($"Setting Client isReady: {value}");
            clientStatus.text = "Ready";
            isReady = true;
        }
        else
        {
            Debug.Log($"Setting Client isReady: {value}");
            clientStatus.text = "Not Ready";
            isReady = false;
        }
    }

    public void SetTeam(int value)
    {
        team = value;
        backgroundImage.color = teamColors[team];
    }

    public void ChangeTeam(int value)
    {
        team = (team + value) % 3;
        backgroundImage.color = teamColors[value];
        Debug.Log($"Changing to team {teamColors[team]}");
    }

    public void EnableTeamSelect()
    {
        nextTeamBtn.SetActive(true);
        prevTeamBtn.SetActive(true);
    }

    public void DisableTeamSelect()
    {
        nextTeamBtn.SetActive(false);
        prevTeamBtn.SetActive(false);
    }
}