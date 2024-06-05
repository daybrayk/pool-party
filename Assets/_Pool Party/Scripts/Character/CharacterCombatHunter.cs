using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daybrayk;
using Daybrayk.rpg;
public class CharacterCombatHunter : CharacterCombatBase
{
    [Header("Hunter")]
    [SerializeField]
    ScriptableEventFloat screenOverlayAlpha;
    [SerializeField]
    StatModifier soakedSpeedModifier;
    [SerializeField]
    float delay = 1f;
    float timer;
    bool isStunned = false;

    public override int currentDamage
    {
        get { return base.currentDamage; }
        protected set
        {
            if (!IsServer) return;

            base.currentDamage = value;
            if(IsOwner) screenOverlayAlpha.Value = currentDamage / maxDamage;
        }
    }

    private void Update()
    {
        if (!isStunned)
        {
            if (timer <= 0) screenOverlayAlpha.Value = 0;
            timer -= Time.deltaTime;
        }
    }

    public override void Soaked()
    {
        if (!isStunned && IsOwner)
        {
            Debug.Log($"Hunter soaked", this);
            isStunned = true;
            root.movement.moveSpeedStat.AddModifier(soakedSpeedModifier);
            StartCoroutine(StunnedHelper());
        }
    }

    IEnumerator StunnedHelper()
    {
        yield return new WaitForSeconds(2f);
        isStunned = false;
        root.movement.moveSpeedStat.RemoveModifier(soakedSpeedModifier);
        screenOverlayAlpha.Value = 0;

        if (IsServer) RemoveDamage(maxDamage);
        else RemoveDamageServerRpc(maxDamage);
    }

    public override void ApplyDamage(int value)
    {
        if (isStunned) return;

        base.ApplyDamage(value);
        timer = delay;
    }

    public override void ApplyDamage(int value, ulong damagingClientId)
    {
        if (isStunned) return;

        base.ApplyDamage(value, damagingClientId);
        timer = delay;
    }

    protected override void OnDamageUpdated(int previousVal, int currentVal)
    {
        base.OnDamageUpdated(previousVal, currentVal);

        if (IsOwner) screenOverlayAlpha.Value = (float)currentDamage / (float)maxDamage;
    }
}