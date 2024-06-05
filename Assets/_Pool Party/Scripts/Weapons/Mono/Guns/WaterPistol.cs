using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class WaterPistol : LocalSimWeapon
{
    public override void StartShoot()
    {
        if (owner.weapon.currentWaterLevel <= 0 || shotTimer < fireRate) return;

        startShoot = true;
        stopShoot = false;

        ProjectileBase p = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<ProjectileBase>();

        CalculateVelocity(out Vector2 velocity);
        p.Init(velocity, owner);
        
        if (!IsServer) ShootServerRpc(velocity);
        else
        {
            ShootClientRpc(velocity);
        }

        AdjustPressure(-1);
        owner.weapon.AdjustWaterLevel(-1);
        shotTimer = 0;
    }

    public override void StopShoot()
    {
        startShoot = false;
        stopShoot = true;
    }
}