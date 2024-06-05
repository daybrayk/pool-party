using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WaterballoonProjectile : ThrowableProjectile
{

    protected Collider2D[] colliderBuffer = new Collider2D[10];

    public override void Init(Vector2 velocity, float scale, float timeToGround, CharacterRoot owner)
    {
        this.owner = owner;
        visrep.transform.localScale += Vector3.one * scale;
        trigger.radius = collider.radius = visrep.transform.localScale.x / 2;
        damage = Mathf.RoundToInt(damage * (1 + scale));
        damageRadius *= (1 + scale);
        explosionVisrep.transform.localScale += Vector3.one * scale;
        rigidbody.velocity = velocity;
        this.timeToGround = timeToGround;
    }

    protected override void OnTriggerEnter2D(Collider2D c)
    {
        if (!IsServer) return;

        CharacterRoot root;
        if (c.TryGetComponent(out root) && (canHarmOwner || root != owner))
        {
            if (GameModeBase.instance is ManHuntGameMode)
            {
                if (!canHarmTeam && (root.owningPlayer.teamId.Value == owner.owningPlayer.teamId.Value)) return;
            }
            else
            {
                Debug.Log("Not Man Hunt Mode");
            }

            DamagePlayersInRange();
            rigidbody.isKinematic = true;
            explosionVisrep.SetActive(true);
            visrep.SetActive(false);
            StartCoroutine(DestructionHelper());
        }
    }

    protected void DamagePlayersInRange()
    {
        int hits = Physics2D.OverlapCircleNonAlloc(transform.position, damageRadius, colliderBuffer);
        CharacterRoot root;
        for (int i = 0; i < hits; i++)
        {
            if (colliderBuffer[i].TryGetComponent(out root) && root.hasCombat) root.combat.ApplyDamage(damage);
        }
    }

}