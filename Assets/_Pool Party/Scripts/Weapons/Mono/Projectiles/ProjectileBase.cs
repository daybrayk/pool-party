using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField]
    protected int damage = 10;
    public float lifeTime = 1.5f;

    [SerializeField]
    GameObject visrep;
    [SerializeField]
    ParticleSystem destructionEffect;

    [SerializeField]
    protected new Rigidbody2D rigidbody;
    public CharacterRoot owner { get; protected set; }
    public Vector2 velocity
    {
        get { return rigidbody.velocity; }
        set 
        { 
            rigidbody.AddForce(value, ForceMode2D.Impulse); 
        }
    }

    float speed;

    public virtual void Init(float speed, Vector2 dir, CharacterRoot owner)
    {
        this.speed = speed;
        this.owner = owner;
        gameObject.SetActive(true);
        rigidbody.velocity = speed * dir;
    }

    public virtual void Init(Vector2 velocity, CharacterRoot owner)
    {
        this.owner = owner;
        gameObject.SetActive(true);
        rigidbody.velocity = velocity;
    }

    protected void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            BeginDestroy();
        }
    }

    protected void OnTriggerEnter2D(Collider2D c)
    {
        CharacterRoot root;
        CarSection car;

        if (c.TryGetComponent(out root))
        {
            if (!owner) root.combat.ApplyDamage(damage);
            else if (owner != root)
            {
                if ((GameModeBase.instance is TeamGameMode))
                {
                    if (owner.owningPlayer.teamId.Value == root.owningPlayer.teamId.Value)
                    {
                        BeginDestroy();
                        return;
                    }
                }

                root.combat.ApplyDamage(damage, owner.OwnerClientId);
            }
        }
        else if (c.TryGetComponent(out car))
        {
            if (owner == null) Debug.Log("Owner is null");
            else if (owner.owningPlayer == null) Debug.Log("OwningPlayer is null");
            car.ApplyDamage(damage, owner.owningPlayer.teamId.Value);
        }
        else if (c.CompareTag("Shield"))
        {
            if (c.TryGetComponent(out root)) owner = root;
            Vector2 reflectedVelocity = Vector2.Reflect(rigidbody.velocity, c.transform.up);
            rigidbody.velocity = reflectedVelocity;
            lifeTime = 1.5f;

            return;
        }

        BeginDestroy();
    }

    protected virtual void BeginDestroy()
    {
        visrep.SetActive(false);
        destructionEffect.gameObject.SetActive(true);
        rigidbody.velocity = Vector2.zero;
        StartCoroutine(DestructionHelper());
    }

    IEnumerator DestructionHelper()
    {
        yield return new WaitForSeconds(destructionEffect.main.duration);

        Destroy(gameObject);
    }
}