using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterVisualization : NetworkBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    GameObject visrep;
    [SerializeField]
    GameObject gunVisrep;
    [SerializeField]
    GameObject shieldVisrep;
    [SerializeField]
    GameObject throwableVisrep;

    CharacterRoot root;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
    }

    public void SetAnimParam(string name, int value)
    {
        animator.SetInteger(name, value);
    }

	public void SetAnimParam(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    #region Visrep
    public void ToggleVisrep(bool show)
    {
        visrep.SetActive(show);
    }

    [ServerRpc]
    public void ToggleVisrepServerRpc(bool show)
    {
        visrep.SetActive(show);
    }

    [ClientRpc]
    public void ToggleVisrepClientRpc(bool show)
    {
        visrep.SetActive(show);
    }
    #endregion

    #region WeaponVisrep
    public void ToggleWeapon(bool show)
    {
        gunVisrep.SetActive(show);
    }

    [ServerRpc]
    public void ToggleWeaponServerRpc(bool show)
    {
        gunVisrep.SetActive(show);

        ToggleWeaponClientRpc(show);
    }

    [ClientRpc]
    public void ToggleWeaponClientRpc(bool show)
    {
        if (IsServer || IsOwner) return;

        gunVisrep.SetActive(show);
    }
    #endregion

    #region Shield Visrep
    public void ToggleShield(bool show)
    {
        shieldVisrep.SetActive(show);
    }

    [ServerRpc]
    public void ToggleShieldServerRpc(bool show)
    {
        shieldVisrep.SetActive(show);

    }

    [ClientRpc]
    public void ToggleShieldClientRpc(bool show)
    {
        shieldVisrep.SetActive(show);
    }
    #endregion

    public void ShowThrowable()
    {
        throwableVisrep.SetActive(true);
    }

    public void HideThrowable()
    {
        throwableVisrep.SetActive(false);
    }

    public void ResetForSpawn()
    {
        ToggleVisrep(true);
        ToggleWeapon(true);
    }
}