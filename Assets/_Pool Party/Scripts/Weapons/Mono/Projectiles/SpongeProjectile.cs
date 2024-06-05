using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpongeProjectile : ThrowableProjectile
{
    [SerializeField]
    float dotDuration = 5f;

    float dotTimer;
    int tickDamage;
    CharacterRoot target;

    new protected void Awake()
    {
        base.Awake();
        tickDamage = Mathf.RoundToInt(damage / dotDuration);
    }

    protected override void OnTriggerEnter2D(Collider2D c)
    {
        if (!IsServer) return;

        if(c.TryGetComponent(out target) && (canHarmOwner || target != owner))
        {
            rigidbody.isKinematic = true;
            visrep.SetActive(false);
            DestructionClientRpc();
            StartCoroutine(DestructionHelper());
        }
    }

    [ClientRpc]
    void DestructionClientRpc()
    {
        rigidbody.isKinematic = true;
        visrep.SetActive(false);
    }

    protected override IEnumerator DestructionHelper()
    {
        WaitForSeconds tick = new WaitForSeconds(1);

        while (dotTimer <= dotDuration)
        {
            target.combat.ApplyDamage(tickDamage, owner.OwnerClientId);
            dotTimer += 1;
            yield return tick;
        }

        Destroy(gameObject);
    }
}