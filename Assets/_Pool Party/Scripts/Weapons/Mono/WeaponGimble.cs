using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponGimble : NetworkBehaviour
{
    [SerializeField]
    CharacterWeapon characterWeapon;
    [SerializeField]
    Transform _weaponHandle;
    public Transform weaponHandle => _weaponHandle;

    public void UpdateRotation(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        //float dot = Vector2.Dot(direction, Vector2.right);

        //if (dot > 0)
        //{
        //    characterWeapon.current.rightFacingVisrep.SetActive(true);
        //    characterWeapon.current.leftFacingVisrep.SetActive(false);
        //}
        //else if (dot < 0)
        //{
        //    characterWeapon.current.rightFacingVisrep.SetActive(false);
        //    characterWeapon.current.leftFacingVisrep.SetActive(true);
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateRotationServerRpc(Vector2 direction)
    {
        UpdateRotation(direction);
        UpdateRotationClientRpc(direction);
    }

    [ClientRpc]
    public void UpdateRotationClientRpc(Vector2 direction)
    {
        if (IsHost || IsOwner) return;

        UpdateRotation(direction);
    }
}