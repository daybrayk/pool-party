using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
public enum TriggerStyle
{
    SemiAuto,
    Automatic,
}

public abstract class WeaponBase : NetworkBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField]
    protected TriggerStyle _triggerStyle;
    public TriggerStyle triggerStyle => _triggerStyle;
    [SerializeField]
    protected float fireRate = 0.5f;
    [SerializeField]
    protected float rechargeTime = 1.0f;
    [SerializeField]
    protected int _maxPressure = 10;
    public int maxPressure => _maxPressure;
    [SerializeField]
    [Range(0, 1f)]
    protected float _maxPressureThreshold = 0.75f;
    public float maxPressureThreshold => _maxPressureThreshold;
    [SerializeField]
    protected bool useWater = true;
    [SerializeField]
    protected bool addPlayerMovement = true;

    [Header("Projectile")]
    [SerializeField]
    protected Transform spawnPoint;
    [SerializeField]
    protected GameObject projectilePrefab;
    [SerializeField]
    protected float timeToGround = 1f;
    [SerializeField]
    [MinMax]
    protected Vector2Int projectileSpeed;

    [Header("Visrep")]
    [SerializeField]
    protected GameObject visrep;

    [Header("UI Tracking")]
    [SerializeField]
    protected ScriptableEventFloat pressureTracker;

    [Header("Debug")]
    [SerializeField]
    [ReadOnly]
    protected int _currentPressure;
    public int currentPressure => _currentPressure;

    protected float shotTimer = 0;
    public bool isCharging { get; protected set; } = false;
    protected bool startShoot = false;
    protected bool stopShoot = true;
    protected Coroutine chargeCoroutine;
    public CharacterRoot owner { get; protected set; }
    
    protected void Awake()
    {
        owner = GetComponentInParent<CharacterRoot>();
        if (owner == null)
        {
            addPlayerMovement = false;
        }

        shotTimer = fireRate;
        _currentPressure = maxPressure;
    }

    protected void Update()
    {
        shotTimer += Time.deltaTime;
    }

    public virtual void Equip() 
    {
        gameObject.SetActive(true);
        if (IsOwner) pressureTracker.Value = (float)currentPressure / (float)maxPressure;
        isCharging = false;
    }

    public virtual void UnEquip() 
    {
        gameObject.SetActive(false);
        startShoot = false;
        stopShoot = true;
    }

    public virtual bool CheckEquipRequirements() { return true; }

    public void CancelCharge()
    {
        if (isCharging)
        {
            Debug.Log("Canceling charge");
            StopCoroutine(chargeCoroutine);
            isCharging = false;
        }
    }

    public void AdjustPressure(int value)
    {
        _currentPressure = Mathf.Min(Mathf.Max(currentPressure + value, 0), maxPressure);

        if (IsOwner) pressureTracker.Value = (float)currentPressure / (float)maxPressure;
    }

    public virtual void StartShoot() { }

    public virtual void StopShoot() { }

    public virtual void Charge() 
    {
        startShoot = false;
        stopShoot = true;

        if (!isCharging) chargeCoroutine = StartCoroutine(ChargeHelper());
    }

    IEnumerator ChargeHelper()
    {
        WaitForSeconds delay = new WaitForSeconds(rechargeTime / maxPressure);
        isCharging = true;

        while (currentPressure < maxPressure)
        {
            yield return delay;

            AdjustPressure(1);
        }

        isCharging = false;
    }

    protected void OnValidate()
    {
        _maxPressure = Mathf.Max(_maxPressure, 1);
        fireRate = Mathf.Max(fireRate, 0);
        rechargeTime = Mathf.Max(rechargeTime, 0.001f);
    }

    protected void CalculateVelocity(out Vector2 velocity)
    {
        CalculateVelocity(transform.up, out velocity);
    }

    protected virtual void CalculateVelocity(Vector2 trajectory, out Vector2 velocity)
    {
        //'Vector2 velocityAdjustment = Vector2.zero;
        velocity = trajectory;

        //Braydon 4/13/2022: Currently there are some non-character prefabs that use weapons to shoot, in order to save some dev time.
        //To prevent null ref errors I am wrapping the movement velocity addition in a bool that is set to false if there is no
        //CharacterRoot component found on the object
        //if (addPlayerMovement)
        //{
        //    float xAdjustment = Vector2.Dot(transform.up.With(y: 0), owner.movement.currentVelocity);
        //    float yAdjustment = Vector2.Dot(transform.up.With(x: 0), owner.movement.currentVelocity);
        //
        //    velocityAdjustment.x = xAdjustment > 0 ? owner.movement.currentVelocity.x : 0;
        //    velocityAdjustment.y = yAdjustment > 0 ? owner.movement.currentVelocity.y : 0;
        //}

        if (currentPressure > maxPressureThreshold * maxPressure)
        {
            velocity *= projectileSpeed.y;
            //velocity += velocityAdjustment;
        }
        else
        {
            velocity = (trajectory * Mathf.Lerp(projectileSpeed.x, projectileSpeed.y, (float)currentPressure / ((float)maxPressure * maxPressureThreshold)));
        }
    }

    #region Debug
    public void SetMaxBulletSpeed(string value)
    {
        if (!IsOwner) return;
        int temp;
        if (int.TryParse(value, out temp)) projectileSpeed.y = temp;
    }

    public void SetMinBulletSpeed(string value)
    {
        if (!IsOwner) return;

        int temp;
        if (int.TryParse(value, out temp))
        {
            projectileSpeed.x = temp;
            if (projectileSpeed.x > projectileSpeed.y) projectileSpeed.y = projectileSpeed.x;
        }
    }
    #endregion
}