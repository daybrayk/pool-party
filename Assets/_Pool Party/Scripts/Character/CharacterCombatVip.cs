using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daybrayk;
public class CharacterCombatVip : CharacterCombatBase
{
    new private void Awake()
    {
        base.Awake();

        _maxDamage = GameModeBase.instance.scoreLimit;
    }

    public override void ApplyDamage(int value, ulong damagingClientId)
    {
        base.ApplyDamage(value, damagingClientId);

        GameModeBase.instance.AdjustScore(value, damagingClientId, OwnerClientId);
    }

    public override void Soaked()
    {
        if (!isAlive) return;
        isAlive = false;
        //damageMeter.gameObject.SetActive(false);
        root.visualization.ToggleVisrep(false);
        root.weapon.StopShoot();
        root.collider.enabled = false;

        if (IsServer)
        {
            var soakedEvt = new PlayerSoakedEvt(OwnerClientId, lastDamagingClient);
            GameEvents.instance.Raise<PlayerSoakedEvt>(soakedEvt);
        }
    }
}