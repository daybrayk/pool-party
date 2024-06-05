using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
using Daybrayk.rpg;
public class CharacterCombatBasic : CharacterCombatBase
{
	[Header("Basic")]
    [Header("Water Trail")]
    [SerializeField]
    GameObject waterDropPrefab;
    [SerializeField]
    [MinMax]
    Vector2 dropRate;
    float dropTimer;

    [Header("Drying")]
    [SerializeField]
    float selfDryTickRate = 0.25f;
    [SerializeField]
    int selfDryTickAmount = 2;
    [SerializeField]
    StatModifier dryingSpeedModifier;

    Coroutine dryingCoroutine;

    private void Update()
    {
        if (currentDamage > maxDamage / 2)
        {
            var dropTime = Mathf.Lerp(dropRate.y, dropRate.x, (float)currentDamage / (float)maxDamage);

            if (dropTimer >= dropTime)
            {
                var drop = Instantiate(waterDropPrefab, transform.position, transform.rotation).GetComponent<WaterDrop>();
                drop.Init(0.5f, 1f);
                dropTimer = 0;
            }

            dropTimer += Time.deltaTime;
        }
    }

    public override void Soaked()
    {
        if (!isAlive) return;
        isAlive = false;
        //damageMeter.gameObject.SetActive(false);
        root.visualization.ToggleVisrep(false);
        root.weapon.StopShoot();
        root.collider.enabled = false;

        if (IsOwner)
        {
            CancelDry();

            StartCoroutine(RespawnHelper());
        }

        if (IsServer)
        {
            //GameModeBase.instance.AdjustScore(1, lastDamagingClient, OwnerClientId);

            var soakedEvt = new PlayerSoakedEvt(OwnerClientId, lastDamagingClient);
            GameEvents.instance.Raise<PlayerSoakedEvt>(soakedEvt);
            
            if (root.hasFlagHolder) root.flag.DropFlag();
        }
    }

    public override void BeginDry()
    {
        if (!isDrying && currentDamage > 0)
        {
            root.weapon.ChangeState(CharacterWeapon.WeaponStates.None);
            dryingCoroutine = StartCoroutine(DryHelper());
            root.movement.moveSpeedStat.AddModifier(dryingSpeedModifier);
            isDrying = true;
        }
    }

    public override void CancelDry()
    {
        if (!isDrying) return;

        isDrying = false;
        if (dryingCoroutine != null) StopCoroutine(dryingCoroutine);
        root.movement.moveSpeedStat.RemoveModifier(this);
        root.weapon.ExitCurrentState();
    }

    IEnumerator DryHelper()
    {
        while (currentDamage > 0)
        {
            yield return new WaitForSeconds(selfDryTickRate);
            if (IsServer) RemoveDamage(selfDryTickAmount);
            else
            {
                RemoveDamageServerRpc(selfDryTickAmount);
            }
        }

        if (root.weapon.currentState == CharacterWeapon.WeaponStates.None) root.weapon.ExitCurrentState();
        else
        {
            isDrying = false;
            root.movement.moveSpeedStat.RemoveModifier(this);

        }
    }

    IEnumerator RespawnHelper()
    {
        //root.ResetToSpawnPosition();
        GameModeBase.instance.RequestSpawnServerRpc(OwnerClientId);
        yield return new WaitWhile(() => !root.isResetToSpawn);
        yield return new WaitForSeconds(1f);
        root.isResetToSpawn = false;

        root.ResetCharacter();

        root.ResetCharacterServerRpc();
    }

    public override void ResetForSpawn()
    {
        base.ResetForSpawn();
        isAlive = true;
    }
}