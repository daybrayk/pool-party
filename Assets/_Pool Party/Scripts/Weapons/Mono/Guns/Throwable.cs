using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class Throwable : WeaponBase
{
    [Header("Throwable")]
    [SerializeField]
    [MinMax]
    Vector2Int waterUsage;
    [SerializeField]
    int powerChargeSharpness;
    [SerializeField]
    [ReadOnly]
    float power;

    float maxPower = 1f;

    new protected void Update()
    {
        base.Update();

        if(startShoot)
        {
            power = Mathf.Lerp(power, maxPower, 1 - Mathf.Exp(-powerChargeSharpness * Time.deltaTime));
        }
    }

    public override void StartShoot()
    {
        if (shotTimer < fireRate || owner.weapon.balloonCount <= 0) return;
        Debug.Log("Water Balloon start shoot");
        CancelCharge();
        startShoot = true;
        stopShoot = false;
        power = 0;
        maxPower = Mathf.Lerp(1, 0.25f, (float)currentPressure / (float)maxPressure);
    }

    public override void StopShoot()
    {
        if (!startShoot) return;

        stopShoot = true;
        startShoot = false;
        Throw();
    }

    void Throw()
    {
        Vector2 velocity = Mathf.Lerp(projectileSpeed.x, projectileSpeed.y, power) * transform.up;
        var pressure = ((float)currentPressure / (float)maxPressure);
        
        if (IsServer)
        {
            ThrowableProjectile waterBalloon = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<ThrowableProjectile>();
            waterBalloon.Init(velocity, pressure, timeToGround, owner);
            waterBalloon.GetComponent<NetworkObject>().Spawn(true);
        }
        else
        {
            ThrowServerRpc(velocity, pressure);
        }

        AdjustPressure(-currentPressure);
        owner.weapon.AdjustWaterLevel(-Mathf.RoundToInt(Mathf.Lerp(waterUsage.x, waterUsage.y, power)));
        shotTimer = 0;
        owner.weapon.AdjustBalloonCount(-1);
    }

    [ServerRpc]
    void ThrowServerRpc(Vector2 velocity, float pressure)
    {
        ThrowableProjectile waterBalloon = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<ThrowableProjectile>();
        waterBalloon.Init(velocity, pressure, timeToGround, owner);
        waterBalloon.GetComponent<NetworkObject>().Spawn(true);
    }

    //[ClientRpc]
    //void ThrowClientRpc(Vector2 velocity, float pressure)
    //{
    //    if (IsServer) return;
    //
    //    ThrowableProjectile waterBalloon = Instantiate(projectilePrefab, spawnPoint.position, transform.rotation).GetComponent<ThrowableProjectile>();
    //    waterBalloon.Init(velocity, pressure, timeToGround, owner);
    //    //waterBalloon.GetComponent<NetworkObject>().Spawn(true);
    //}
}