using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public class Shotgun : LocalSimWeapon
{
    [SerializeField]
    float coneAngle;
    
    [SerializeField]
    [MinMax]
    Vector2Int projectileCountRange = new Vector2Int(3, 6);

    public override void StartShoot()
    {
        if (owner.weapon.currentWaterLevel <= 0 || shotTimer < fireRate) return;

        if (IsServer) ShootClientRpc(currentPressure);
        else
        {
            ShootServerRpc(currentPressure);
        }
        SpawnProjectiles(Mathf.CeilToInt(Mathf.Lerp(projectileCountRange.x, projectileCountRange.y, (float)currentPressure / (float)maxPressure)));


        _currentPressure = 0;
        startShoot = true;
        stopShoot = false;
        
    }

    public override void StopShoot()
    {
        startShoot = false;
        stopShoot = true;
    }

    [ServerRpc]
    protected void ShootServerRpc(int pressure)
    {
        SpawnProjectiles(Mathf.CeilToInt(Mathf.Lerp(projectileCountRange.x, projectileCountRange.y, (float)pressure / (float)maxPressure)));
        ShootClientRpc(pressure);
    }

    [ClientRpc]
    protected void ShootClientRpc(int pressure)
    {
        if (IsOwner || IsServer) return;

        SpawnProjectiles(Mathf.CeilToInt(Mathf.Lerp(projectileCountRange.x, projectileCountRange.y, (float)pressure / (float)maxPressure)));
    }

    void SpawnProjectiles(int projectileCount)
    {
        var straightVector = transform.up;
        var angleInRad = (coneAngle / 2) * Mathf.PI / 180;
        Vector2 startVector = new Vector2((Mathf.Cos(-angleInRad) * straightVector.x) - (Mathf.Sin(-angleInRad) * straightVector.y),
            Mathf.Sin(-angleInRad) * straightVector.x + Mathf.Cos(-angleInRad) * straightVector.y);

        Vector2 endVector = new Vector2(Mathf.Cos(angleInRad) * straightVector.x - Mathf.Sin(angleInRad) * straightVector.y,
            Mathf.Sin(angleInRad) * straightVector.x + Mathf.Cos(angleInRad) * straightVector.y);

        for (int i = 0; i < projectileCount; i++)
        {
            Vector2 vector = Vector2.Lerp(startVector, endVector, (float)i / (float)(projectileCount-1));
            CalculateVelocity(vector, out Vector2 velocity);
            ProjectileBase p = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.LookRotation(Vector3.forward, vector)).GetComponent<ProjectileBase>();
            
            p.Init(velocity, owner);
        }
    }
}