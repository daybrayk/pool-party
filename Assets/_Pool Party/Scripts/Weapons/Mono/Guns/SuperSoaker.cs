using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class SuperSoaker : LocalSimWeapon
{
    bool continueChain = true;

    LinkedProjectile lastFired;

    new void Update()
    {
        if (!IsOwner) return;

        base.Update();

        if(startShoot && !stopShoot)
        {
            if ((!useWater ||owner.weapon.currentWaterLevel > 0) && shotTimer > fireRate)
            {
                LinkedProjectile p = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<LinkedProjectile>();
                Debug.Assert(p != null);
                //Add player velocity to projectile initial velocity
                if (continueChain)
                {
                    if (lastFired != null)
                    {
                        lastFired.nextProjectile = p;
                    }
                    p.previousProjectile = lastFired;
                }
                else
                {
                    p.lineRenderer.positionCount = 1;
                    continueChain = true;
                }

                CalculateVelocity(out Vector2 velocity);
                p.Init(velocity, owner);

                if (!IsServer && IsOwner) ShootServerRpc(velocity);
                else
                {
                    ShootClientRpc(velocity);
                }

                if (useWater)
                {
                    AdjustPressure(-1);
                    owner.weapon.AdjustWaterLevel(-1);
                }

                lastFired = p;
                shotTimer = 0;
            }
            
        }

    }
    
    [ServerRpc]
    protected override void ShootServerRpc(Vector2 velocity)
    {
        LinkedProjectile p = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<LinkedProjectile>();
        Debug.Assert(p != null);
        //p.velocity = transform.up * Mathf.Lerp(projectileSpeed.x, projectileSpeed.y, (float)currentPressure / (float)maxPressure);

        if (continueChain)
        {
            if (lastFired != null)
            {
                lastFired.nextProjectile = p;
            }
            p.previousProjectile = lastFired;
        }
        else
        {
            p.lineRenderer.positionCount = 1;
            continueChain = true;
        }
        p.Init(velocity, owner);
        //p.Init(Mathf.Lerp(projectileSpeed.x, projectileSpeed.y, (float)currentPressure / (float)maxPressure), transform.up, owner);

        lastFired = p;
        shotTimer = 0;

        ShootClientRpc(velocity);
    }

    [ClientRpc]
    protected override void ShootClientRpc(Vector2 velocity)
    {
        if (IsHost || IsOwner) return;

        LinkedProjectile p = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<LinkedProjectile>();
        Debug.Assert(p != null);
        p.Init(velocity, owner);
        //p.velocity = transform.up * Mathf.Lerp(projectileSpeed.x, projectileSpeed.y, (float)currentPressure / (float)maxPressure);

        if (continueChain)
        {
            if (lastFired != null)
            {
                lastFired.nextProjectile = p;
            }
            p.previousProjectile = lastFired;
        }
        else
        {
            p.lineRenderer.positionCount = 1;
            continueChain = true;
        }

        lastFired = p;
        shotTimer = 0;
    }

    public override void StartShoot()
    {
        CancelCharge();
        startShoot = true;
        stopShoot = false;
    }

    public override void StopShoot()
    {
        startShoot = false;
        stopShoot = true;
        continueChain = false;
    }
}