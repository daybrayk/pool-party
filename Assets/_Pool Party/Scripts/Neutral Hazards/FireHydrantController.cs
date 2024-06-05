using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class FireHydrantController : NetworkBehaviour
{
    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    SuperSoaker[] projectileLaunchers;

    private void Start()
    {
        for (int i = 0; i < projectileLaunchers.Length; i++)
        {
            projectileLaunchers[i].StartShoot();
        }
    }
}
