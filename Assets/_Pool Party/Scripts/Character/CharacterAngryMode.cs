using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Daybrayk.rpg;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterAngryMode : NetworkBehaviour
{
	CharacterRoot root;

    [SerializeField]
    StatModifier moveSpeedModifier;
    [Header("Debug")]
    [SerializeField]
    bool _isActive;
    public bool isActive => _isActive;
    [SerializeField]
    float duration = 5f;
    [SerializeField]
    float cooldown = 30f;

    float timer;
    float cooldownTimer;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
        moveSpeedModifier.source = this;
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.isPressed) Activate();

        if (isActive)
        {
            if (timer <= 0) Deactivate();
            timer -= Time.deltaTime;
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void Activate()
    {
        if (cooldownTimer > 0 || isActive) return;

        _isActive = true;
        root.combat.RemoveDamageServerRpc(root.combat.maxDamage);
        root.movement.moveSpeedStat.AddModifier(moveSpeedModifier);
        timer = duration;
    }

    public void Deactivate()
    {
        _isActive = false;
        root.movement.moveSpeedStat.RemoveModifier(this);
        cooldownTimer = cooldown;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CharacterAngryMode))]
public class CharacterAngryModeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CharacterAngryMode t = target as CharacterAngryMode;
        if (GUILayout.Button("Activate"))
        {
            if (!t.isActive) t.Activate();
            else
            {
                t.Deactivate();
            }
        }
    }
}

#endif