using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class CarSection : NetworkBehaviour
{
    [SerializeField]
    Color dirtyColor;
    [SerializeField]
    Color cleanColor;
    [SerializeField]
    SpriteRenderer sprite;

    CarController carController;
    public int health { get; private set; }

    [SerializeField]
    [ReadOnly]
    NetworkVariable<int> _currentHealth;
    public int currentHealth => _currentHealth.Value;
    public bool isClean { get; private set; } = false;

    public override void OnNetworkSpawn()
    {
        _currentHealth.OnValueChanged += OnHealthUpdate;
    }

    public void Init(int health, CarController car)
    {
        carController = car;
        this.health = health;
        if(IsServer) _currentHealth.Value = health;
    }

    public void ApplyDamage(int value, ulong damagingTeamId)
    {
        if (!IsServer) return;
        if (isClean) return;
        if (damagingTeamId == carController.teamId) return;

        _currentHealth.Value -= value;
        var newColor = Color.Lerp(cleanColor, dirtyColor, (float)_currentHealth.Value / (float)health);
        sprite.color = newColor;

        if (_currentHealth.Value <= 0)
        {
            isClean = true;
            GameModeBase.instance.AdjustScore(1, damagingTeamId);
        }
    }

    void OnHealthUpdate(int prevValue, int newValue)
    {
        sprite.color = Color.Lerp(cleanColor,  dirtyColor, (float)newValue / (float)health);
    }
}