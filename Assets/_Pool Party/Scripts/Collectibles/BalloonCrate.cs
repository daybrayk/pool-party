using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BalloonCrate : CollectibleBase
{
    [Header("Balloon Crate")]
    [SerializeField]
    int balloonCount;

    [SerializeField]
    float respawnTime = 30;

    [SerializeField]
    GameObject visrep;

    new Collider2D collider;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    protected override void OnTriggerEnter2D(Collider2D c)
    {
        if (c.TryGetComponent(out CharacterRoot root))
        {
            if (root.weapon.balloonCount < root.weapon.maxWaterBalloons)
            {
                Collect(root);
            }
        }
    }

    protected override void Collect(CharacterRoot root)
    {
        root.weapon.AdjustBalloonCount(balloonCount);
        visrep.SetActive(false);
        collider.enabled = false;
        StartCoroutine(Respawn());

        if (!IsServer) CollectServerRpc();
        else
        {
            CollectClientRpc();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void CollectServerRpc()
    {
        visrep.SetActive(false);
        collider.enabled = false;
        StartCoroutine(Respawn());
        CollectClientRpc();
    }

    [ClientRpc]
    void CollectClientRpc()
    {
        if (IsOwner || IsServer) return;

        visrep.SetActive(false);
        collider.enabled = false;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        visrep.SetActive(true);
        collider.enabled = true;
    }
}