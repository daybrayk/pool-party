using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;

public abstract class LocalSimWeapon : WeaponBase
{
    [ServerRpc]
    protected virtual void ShootServerRpc(Vector2 velocity)
    {
        ProjectileBase p = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<ProjectileBase>();
        p.Init(velocity, owner);

        ShootClientRpc(velocity);
    }

    [ClientRpc]
    protected virtual void ShootClientRpc(Vector2 velocity)
    {
        //Because the host is also considered a client we need to early out from the ClientRpc call
        if (IsOwner || IsServer) return;

        ProjectileBase p = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<ProjectileBase>();
        p.Init(velocity, owner);
    }
}