using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowableProjectile : NetworkBehaviour
{
    [SerializeField]
    protected int damage;
    [SerializeField]
    protected float damageRadius;
    [SerializeField]
    new protected CircleCollider2D collider;
    [SerializeField]
    protected CircleCollider2D trigger;
    [SerializeField]
    protected GameObject visrep;
    [SerializeField]
    protected GameObject explosionVisrep;
    [SerializeField]
    protected bool canHarmTeam = true;
    new protected Rigidbody2D rigidbody;

    protected CharacterRoot owner;

    protected float timeToGround = 1f;
    protected float timer;
    protected bool canHarmOwner = false;
    protected bool canHitShield = true;

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        explosionVisrep.transform.localScale = new Vector3(damageRadius, damageRadius, damageRadius);
    }

    private void Update()
    {
        if(timer < timeToGround)
        {
            timer += Time.deltaTime;
            return;
        }

        canHarmOwner = true;
        canHitShield = false;
        rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, Vector2.zero, 1 - Mathf.Exp(-5 * Time.deltaTime));
    }

    public virtual void Init(Vector2 velocity, float scale, float timeToGround, CharacterRoot owner)
    {
        this.owner = owner;
        damage = Mathf.RoundToInt(damage *(1 + scale));
        rigidbody.velocity = velocity;
        this.timeToGround = timeToGround;
    }

    protected virtual void OnTriggerEnter2D(Collider2D c) { }

    protected virtual IEnumerator DestructionHelper()
    {
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}