using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Daybrayk;
using Daybrayk.StateMachine;
[RequireComponent(typeof(CharacterRoot), typeof(NetworkObject))]
public partial class CharacterWeapon : NetworkBehaviour
{
    public enum WeaponStates
    {
        None,
        Gun,
        Shield,
        Throwable,
    }
    [Header("Water")]
    [SerializeField]
    int _maxWaterLevel;
    public int maxWaterLevel => _maxWaterLevel;
    [SerializeField]
    float reloadTime;
    [SerializeField]
    [ReadOnly]
    int _currentWaterLevel;
    public int currentWaterLevel => _currentWaterLevel;


    [Header("Weapon")]
    [SerializeField]
    WeaponGimble _weaponGimble;
    public WeaponGimble weaponGimble => _weaponGimble;
    [SerializeField]
    int defaultWeaponIndex;
    [SerializeField]
    WeaponStates startingState;

    [Header("Shield")]
    [SerializeField]
    int shieldWaterUsage = 1;
    [SerializeField]
    float tickTime = 0.5f;
    float shieldTickTimer;

    [Header("Throwables")]
    [SerializeField]
    int _maxWaterBalloons = 3;
    public int maxWaterBalloons => _maxWaterBalloons;
    [SerializeField]
    ScriptableEventInt waterBalloonCount;
    public int balloonCount => waterBalloonCount.Value;
    [SerializeField]
    Throwable waterBalloon;

    [Header("Events")]
    [SerializeField]
    ScriptableEventFloat waterLevelTracker;
    [SerializeField]
    ScriptableEvent reloadStart;
    [SerializeField]
    ScriptableEvent reloadCancel;

    public WeaponBase current
    {
        get { return _current; }
        private set
        {
            _current = value;
        }
    }
    public int currentWeaponIndex => _currentWeaponIndex;

    [SerializeField]
    GameObject shield;


    [SerializeField]
    List<WeaponBase> weapons = new List<WeaponBase>();

    [Header("Debug")]
    [ReadOnly]
    public bool inRefillZone = false;
    [SerializeField]
    [ReadOnly]
    WeaponBase _current;
    [SerializeField]
    [ReadOnly]
    int _currentWeaponIndex;

    bool isRefilling;
    Coroutine refillCoroutine;
    CharacterRoot root;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
        SetupStateMachine();
    }

    public override void OnNetworkSpawn()
    {
        gameObject.name = "Player " + OwnerClientId + " Avatar";
        waterBalloon.gameObject.SetActive(false);
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }

        _currentWeaponIndex = defaultWeaponIndex;
        
        waterBalloonCount.Value = _maxWaterBalloons;
        
        if(IsOwner) ChangeState(startingState);
        
        AdjustWaterLevel(maxWaterLevel);
    }

    private void Update()
    {
        if (!IsSpawned) return;

        var aimDirection = root.input.aimDirection;
        weaponGimble.UpdateRotation(aimDirection);
        if (!IsServer) weaponGimble.UpdateRotationServerRpc(aimDirection);
        else
        {
            weaponGimble.UpdateRotationClientRpc(aimDirection);
        }

        stateMachine.Update();
    }

    public void AdjustWaterLevel(int value)
    {
        _currentWaterLevel = Mathf.Min(Mathf.Max(_currentWaterLevel + value, 0), maxWaterLevel);
        if(IsOwner) waterLevelTracker.Value = (float)currentWaterLevel / (float)maxWaterLevel;
    }

    /*********** Shoot ***********/
    #region Shoot
    public void StartShoot()
    {
        var canShoot = (stateMachine.currentState.name == WeaponStates.Gun || stateMachine.currentState.name == WeaponStates.Throwable) && current;
        if (!canShoot) return;

        CancelReload();
        current.StartShoot();
    }

    public void StopShoot()
    {
        if (!current) return;
        
        current.StopShoot();
    }
    #endregion

    /*********** Shield ***********/
    #region Shield
    public void StartShield()
    {
        root.combat.CancelDry();
        ChangeState(WeaponStates.Shield);
    }

    [ServerRpc]
    public void StartShieldServerRpc()
    {
        StartShield();
        StartShieldClientRpc();
    }

    [ClientRpc]
    public void StartShieldClientRpc()
    {
        if (IsOwner || IsHost) return;
        StartShield();
    }

    public void StopShield()
    {
        if (stateMachine.currentState.name == WeaponStates.Shield) stateMachine.ExitCurrentState();
    }

    [ServerRpc]
    public void StopShieldServerRpc()
    {
        StopShield();
        StopShieldClientRpc();
    }

    [ClientRpc]
    public void StopShieldClientRpc()
    {
        if (IsOwner) return;
        StopShield();
    }
    #endregion

    /*********** Weapon Equip ***********/
    #region Weapon Equip
    public void EquipWeapon(int index)
    {
        if(current) current.UnEquip();
        if(weapons.Count <= index)
        {
            Debug.LogError("Desired weapon index out of range of guns List\n" +
                "index: " + index + " guns.Count: " + weapons.Count, gameObject);
            return;
        }
        weapons[index].Equip();
        current = weapons[index];
        _currentWeaponIndex = index;
    }

    [ServerRpc]
    void EquipWeaponServerRpc(int index)
    {
        EquipWeapon(index);
        EquipWeaponClientRpc(index);
    }

    [ClientRpc]
    void EquipWeaponClientRpc(int index)
    {
        if (IsHost || IsOwner) return;

        EquipWeapon(index);
    }

    public void UnEquipWeapon()
    {
        if(!current) return;

        current.UnEquip();
        current = null;
    }

    [ServerRpc]
    void UnequipWeaponServerRpc()
    {
        if (!IsOwner) return;

        UnEquipWeapon();
        UnequipWeaponClientRpc();
    }

    [ClientRpc]
    void UnequipWeaponClientRpc()
    {
        if(IsServer || IsOwner) return;

        UnEquipWeapon();
    }
    #endregion

    /*********** Throwable ***********/
    #region Throwable
    public void ToggleThrowable()
    {
        root.combat.CancelDry();
        if (stateMachine.currentState.name != WeaponStates.Throwable)
        {
            stateMachine.ChangeState(WeaponStates.Throwable);
        }
        else
        {
            stateMachine.ExitCurrentState();
        }
    }

    [ServerRpc]
    void ToggleThrowableServerRpc(bool value)
    {
        if (value)
        {
            waterBalloon.Equip();
            current = waterBalloon;
        }
        else
        {
            waterBalloon.UnEquip();
        }

        ToggleThrowableClientRpc(value);
    }

    [ClientRpc]
    void ToggleThrowableClientRpc(bool value)
    {
        if (IsServer || IsOwner) return;

        if (value)
        {
            waterBalloon.Equip();
            current = waterBalloon;
        }
        else
        {
            waterBalloon.UnEquip();
        }
    }
    #endregion


    public void Reload()
    {
        if (isRefilling) return;

        if (inRefillZone && currentWaterLevel < maxWaterLevel)
        {
            reloadStart.Raise(this);
            if (current != null && current.isCharging) current.CancelCharge();
            refillCoroutine = StartCoroutine(RefillHelper());
        }
        else if (current.currentPressure < current.maxPressure && currentWaterLevel > 0)
        {
            current.Charge();
        }
    }

    public void CancelReload()
    {
        CancelRefill();
        CancelCharge();
    }

    IEnumerator RefillHelper()
    {
        WaitForSeconds delay = new WaitForSeconds(reloadTime / maxWaterLevel);
        isRefilling = true;

        while (currentWaterLevel < maxWaterLevel && inRefillZone)
        {
            yield return delay;

            AdjustWaterLevel(1);
        }

        isRefilling = false;
        reloadCancel.Raise(this);
        if (current != null && current.currentPressure < current.maxPressure) current.Charge();
    }

    public void CancelCharge()
    {
        if (!current || !current.isCharging) return;

        current.CancelCharge();
        if (stateMachine.currentState.name != WeaponStates.Gun) ChangeState(WeaponStates.Gun);
    }

    public void CancelRefill()
    {
        if (isRefilling)
        {
            reloadCancel.Raise(this);
            StopCoroutine(refillCoroutine);
            isRefilling = false;
            if(stateMachine.currentState.name != WeaponStates.Gun) ChangeState(WeaponStates.Gun);
        }
    }

    public void NextWeapon()
    {
        if (stateMachine.currentState.name == WeaponStates.None)
        {
            root.combat.CancelDry();
            stateMachine.ChangeState(WeaponStates.Gun);
            return;
        }

        if (stateMachine.currentState.name != WeaponStates.Gun) return;
        
        int j = currentWeaponIndex;

        if (current != waterBalloon) j = (currentWeaponIndex + 1) % weapons.Count;

        EquipWeapon(j);
        if (IsServer) EquipWeaponClientRpc(j);
        else
        {
            EquipWeaponServerRpc(j);
        }

        root.combat.CancelDry();
    }

    public void PreviousWeapon()
    {
        if (stateMachine.currentState.name == WeaponStates.None)
        {
            root.combat.CancelDry();
            stateMachine.ChangeState(WeaponStates.Gun);
            return;
        }

        if (stateMachine.currentState.name != WeaponStates.Gun) return;
        
        int j = currentWeaponIndex;
        
        if (current != waterBalloon)
        {
            j = (currentWeaponIndex - 1) % weapons.Count;
            if (j < 0) j = weapons.Count - 1;
        }
        
        EquipWeapon(j);

        if (IsServer) EquipWeaponClientRpc(j);
        else if (IsClient)
        {
            EquipWeaponServerRpc(j);
        }
    }

    public void ReloadSkillCheck(bool value)
    {
        if (IsOwner && value) AdjustWaterLevel(maxWaterLevel / 3);
    }

    public void AdjustBalloonCount(int value)
    {
        waterBalloonCount.Value = Mathf.Clamp(waterBalloonCount.Value + value, 0, _maxWaterBalloons);

        if (waterBalloonCount.Value <= 0)
        {
            if (weapons.Count <= 0) stateMachine.ChangeState(WeaponStates.None);
            else
            {
                NextWeapon();
            }
            return;
        }
    }

    public void ResetForSpawn()
    {
        for (int i = 1; i <= weapons.Count; i++)
        {
            WeaponBase weapon = weapons[i-1];
            weapon.AdjustPressure(weapon.maxPressure);
        }

        AdjustWaterLevel(maxWaterLevel);
        _currentWeaponIndex = defaultWeaponIndex;
        ChangeState(startingState);
    }

    private void OnValidate()
    {
        defaultWeaponIndex = Mathf.Clamp(defaultWeaponIndex, 0, weapons.Count);
    }
}