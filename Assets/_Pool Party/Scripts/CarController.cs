using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Daybrayk;

public class CarController : NetworkBehaviour
{
    [SerializeField]
    CarSection[] sections;


    [ReadOnly]
    public ulong teamId;
    [SerializeField]
    int totalHealth = 3000;

    [SerializeField]
    [ReadOnly]
    NetworkVariable<int> _currentDamage = new NetworkVariable<int>();
    public int currentDamage => _currentDamage.Value;

    public void Init(ulong teamId, int scoreLimit)
    {
        this.teamId = teamId;
        for (int i = 0; i < sections.Length; i++)
        {
            sections[i].Init(totalHealth / sections.Length, this);
        }
    }

    
}
