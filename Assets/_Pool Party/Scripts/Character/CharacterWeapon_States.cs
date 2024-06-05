using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daybrayk.StateMachine;
public partial class CharacterWeapon
{
    SimpleStateMachine<WeaponStates> stateMachine;
    public WeaponStates currentState => stateMachine.currentState.name;

    void SetupStateMachine()
    {
        stateMachine = new SimpleStateMachine<WeaponStates>();

        //Setup None State
        //StateDelegate beginNone = new StateDelegate(BeginNoneState);
        //StateDelegate updateNone = new StateDelegate(UpdateNoneState);
        //StateDelegate endNone = new StateDelegate(EndNoneState);

        stateMachine.AddState(new SimpleStateMachine<WeaponStates>.SimpleState(WeaponStates.None, BeginNoneState, UpdateNoneState, EndNoneState));

        //Setup Weapon State
        //StateDelegate beginWeapon = new StateDelegate(BeginWeaponState);
        //StateDelegate updateWeapon = new StateDelegate(UpdateWeaponState);
        //StateDelegate endWeapon = new StateDelegate(EndWeaponState);

        stateMachine.AddState(new SimpleStateMachine<WeaponStates>.SimpleState(WeaponStates.Gun, BeginWeaponState, UpdateWeaponState, EndWeaponState));

        //Setup Shield State
        //StateDelegate beginShield = new StateDelegate(BeginShieldState);
        //StateDelegate updateShield = new StateDelegate(UpdateShieldState);
        //StateDelegate endShield = new StateDelegate(EndShieldState);

        stateMachine.AddState(new SimpleStateMachine<WeaponStates>.SimpleState(WeaponStates.Shield, BeginShieldState, UpdateShieldState, EndShieldState));

        //Setup Throwable State
        //StateDelegate beginThrowable = new StateDelegate(BeginThrowableState);
        //StateDelegate updateThrowable = new StateDelegate(UpdateThrowableState);
        //StateDelegate endThrowable = new StateDelegate(EndThrowableState);

        stateMachine.AddState(new SimpleStateMachine<WeaponStates>.SimpleState(WeaponStates.Throwable, BeginThrowableState, UpdateThrowableState, EndThrowableState));
        
        Debug.Log(stateMachine.ToString());
        stateMachine.StartUp();
    }

    public void ChangeState(WeaponStates newState)
    {
        if (stateMachine.currentState.name == newState) return;
        stateMachine.ChangeState(newState);
    }

    public void ExitCurrentState()
    {
        stateMachine.ExitCurrentState();
    }

    #region None State
    void BeginNoneState()
    {
        root.visualization.ToggleShield(false);
        root.visualization.ToggleWeapon(false);

    }

    void UpdateNoneState() { }

    void EndNoneState()
    {
        Debug.Log("Ending None State");
    }
    #endregion

    #region Weapon State
    void BeginWeaponState()
    {
        if (currentWeaponIndex >= weapons.Count)
        {
            stateMachine.ExitImmediate();
            return;
        }
        root.visualization.ToggleWeapon(true);

        EquipWeapon(currentWeaponIndex);
        if (IsServer) EquipWeaponClientRpc(currentWeaponIndex);
        else if (IsOwner)
        {
            EquipWeaponServerRpc(currentWeaponIndex);
        }
    }

    void UpdateWeaponState() { }

    void EndWeaponState()
    {
        if (!current) return;

        CancelReload();
        root.visualization.ToggleWeapon(false);
        UnEquipWeapon();
        if (IsServer) UnequipWeaponClientRpc();
        else if (IsOwner)
        {
            UnequipWeaponServerRpc();
        }

    }
    #endregion

    #region Shield State

    void BeginShieldState()
    {
        root.visualization.ToggleShield(true);
    }

    void UpdateShieldState()
    {
        if (currentWaterLevel < shieldWaterUsage) StopShield();

        if (shieldTickTimer <= 0)
        {
            AdjustWaterLevel(-shieldWaterUsage);
            shieldTickTimer = tickTime;
        }

        shieldTickTimer -= Time.deltaTime;
    }

    void EndShieldState()
    {
        root.visualization.ToggleShield(false);
    }
    #endregion

    #region Throwable State
    void BeginThrowableState()
    {
        if (balloonCount <= 0)
        {
            stateMachine.ExitImmediate();
            return;
        }

        root.visualization.ToggleWeapon(true);

        waterBalloon.Equip();
        current = waterBalloon;

        if (IsServer) ToggleThrowableClientRpc(true);
        else if(IsOwner)
        {
            ToggleThrowableServerRpc(true);
        }
    }

    void UpdateThrowableState()
    {

    }

    void EndThrowableState()
    {
        waterBalloon.UnEquip();

        if (IsServer) ToggleThrowableClientRpc(false);
        else if(IsOwner)
        {
            ToggleThrowableServerRpc(false);
        }
    }
    #endregion
}
