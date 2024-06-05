using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daybrayk.StateMachine;
using Unity.Netcode;
public class CharacterState : NetworkBehaviour
{
    public enum CharacterStates
    {
        Default,
        Inactive,
        Normal,
        Drying,
        Soaked,
        Stunned,
    }

    [SerializeField]
    CharacterStates startingState;

    public bool isDrying => stateMachine.currentState.name == CharacterStates.Drying;
    public bool isActive => stateMachine.currentState.name != CharacterStates.Inactive;
    public bool isStunned => stateMachine.currentState.name != CharacterStates.Stunned;

    public NetworkVariable<CharacterStates> currentState { get; private set; }

    CharacterRoot root;

    SimpleStateMachine<CharacterStates> stateMachine;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
    }

    private void Start()
    {
        SetupStateMachine();
        stateMachine.ChangeState(startingState);
    }

    void SetupStateMachine()
    {
        var inactiveState = new SimpleStateMachine<CharacterStates>.SimpleState(CharacterStates.Inactive, BeginInactive, null, EndInactive);
        
        //Create state machine with default state
        stateMachine = new SimpleStateMachine<CharacterStates>(inactiveState);

        //Setup Normal state
        var normalState = new SimpleStateMachine<CharacterStates>.SimpleState(CharacterStates.Normal, BeginNormal, UpdateNormal, EndNormal);
        stateMachine.AddState(normalState);

        //Setup Drying state
        var dryingState = new SimpleStateMachine<CharacterStates>.SimpleState(CharacterStates.Drying, BeginDrying, UpdateDrying, EndDrying);
        stateMachine.AddState(dryingState);

        var soakedState = new SimpleStateMachine<CharacterStates>.SimpleState(CharacterStates.Soaked, BeginSoaked, UpdateSoaked, EndSoaked);
        stateMachine.AddState(soakedState);

        var stunnedState = new SimpleStateMachine<CharacterStates>.SimpleState(CharacterStates.Stunned, BeginStunned, UpdateStunned, EndStunned);
        stateMachine.AddState(stunnedState);

        stateMachine.StartUp();
    }

    public void ChangeState(CharacterStates state)
    {

    }

    #region Inactive State
    void BeginInactive()
    {
        root.input.DisableInput();
        root.visualization.ToggleVisrep(false);
    }

    void EndInactive()
    {
        root.input.EnableInput();
        root.visualization.ToggleVisrep(true);
    }
    #endregion

    #region Normal State
    void BeginNormal()
    {

    }

    void UpdateNormal()
    {

    }

    void EndNormal()
    {

    }

    #endregion

    #region Drying State
    void BeginDrying()
    {
        root.weapon.ChangeState(CharacterWeapon.WeaponStates.None);
    }

    void UpdateDrying()
    {

    }

    void EndDrying()
    {
        root.weapon.ExitCurrentState();
    }
    #endregion

    #region Soaked State
    void BeginSoaked()
    {
        root.visualization.ToggleVisrep(false);
        root.weapon.StopShoot();
        root.collider.enabled = false;
    }

    void UpdateSoaked()
    {

    }

    void EndSoaked()
    {

    }
    #endregion

    #region Stunned State
    void BeginStunned()
    {

    }

    void UpdateStunned()
    {
        
    }

    void EndStunned()
    {

    }
    #endregion
}