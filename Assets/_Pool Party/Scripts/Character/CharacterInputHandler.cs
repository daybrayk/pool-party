using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Daybrayk;

[RequireComponent(typeof(CharacterRoot), typeof(NetworkObject))]
public class CharacterInputHandler : NetworkBehaviour
{
    public enum DodgeMechanic
    {
        Dodge,
        Shield,
    }

    PlayerInput playerInput;
    public Vector2 moveDirection => _moveDirection;
    public Vector2 aimDirection => _aimDirection;
    public bool acceptInput => _acceptInput;

    [Header("Debug")]
    [SerializeField]
    [ReadOnly]
    Vector2 _moveDirection = new Vector2();
    [SerializeField]
    [ReadOnly]
    Vector2 _aimDirection = new Vector2();
    public DodgeMechanic dodge = DodgeMechanic.Dodge;
    [SerializeField]
    [ReadOnly]
    bool _acceptInput;
    
    CharacterRoot root;

    private void Awake()
    {
        root = GetComponent<CharacterRoot>();

        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnNetworkSpawn()
    {
        if(!IsClient || !IsOwner)
        {
            enabled = false;
            playerInput.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!acceptInput) return;
        if (!root.hasMovement) return;
        if (!root.hasCamera) return;

        root.movement.UpdateVelocity(_moveDirection);

        if (playerInput.currentControlScheme.Equals("Keyboard&Mouse"))
        {
            Vector2 aim = Mouse.current.position.ReadValue();
            Vector3 screenPos = root.camera.mainCamera.WorldToScreenPoint(root.weapon.weaponGimble.transform.position);
            _aimDirection = aim - (Vector2)screenPos;
        }
    }

    public void OnMove(InputValue value)
    {
        if (!acceptInput) return;
        if (!root.hasMovement) return;

        Vector2 v = value.Get<Vector2>();
        _moveDirection = v;
    }

    public void OnAim(InputValue value)
    {
        if (!acceptInput) return;
        if (!root.hasMovement) return;

        Vector2 v = value.Get<Vector2>();
        
        if (playerInput.currentControlScheme.Equals("Gamepad")) _aimDirection = v;
        else
        {
            Vector3 screenPos = root.camera.mainCamera.WorldToScreenPoint(root.weapon.weaponGimble.transform.position);
            _aimDirection = v - (Vector2)screenPos;
        }
        
        _aimDirection.Normalize();

        root.movement.UpdateOrientation(_aimDirection);
    }

    public void OnFire()
    {
        if (!acceptInput) return;
        if (!root.hasWeapon) return;

        root.weapon.StartShoot();
    }

    public void OnFireReleased()
    {
        if (!acceptInput) return;
        if (!root.hasWeapon) return;

        root.weapon.StopShoot();
    }

    public void OnReload()
    {
        if (!acceptInput) return;
        if (!root.hasWeapon) return;

        root.weapon.Reload();
    }

    public void OnNextWeapon()
    {
        if (!root.combat.isAlive) return;
        if (!acceptInput) return;
        if (!root.hasWeapon) return;

;        root.weapon.NextWeapon();
    }

    public void OnPrevWeapon()
    {
        if (!root.combat.isAlive) return;
        if (!acceptInput) return;
        if (!root.hasWeapon) return;

        root.weapon.PreviousWeapon();
    }

    public void OnChangeWeapon(InputValue value)
    {
        if (!root.combat.isAlive) return;
        if (!acceptInput) return;
        if (!root.hasWeapon) return;
        
        Vector2 v = value.Get<Vector2>();
        if (v.y > 0)
        {
            root.weapon.NextWeapon();
        }
        else if (v.y < 0)
        {
            root.weapon.PreviousWeapon();
        }
    }

    public void OnDodge(InputValue value)
    {
        if (!acceptInput) return;
        if (!root.hasMovement) return;

        switch (dodge)
        {
            case DodgeMechanic.Dodge:
                break;
            case DodgeMechanic.Shield:
                if(value.isPressed)
                {
                    if(IsServer)
                    {
                        root.weapon.StartShield();
                        root.weapon.StartShieldClientRpc();
                    }
                    else
                    {
                        root.weapon.StartShield();
                        root.weapon.StartShieldServerRpc();
                    }
                }
                else
                {
                    if (IsServer)
                    {
                        root.weapon.StopShield();
                        root.weapon.StopShieldClientRpc();
                    }
                    else
                    {
                        root.weapon.StopShield();
                        root.weapon.StopShieldServerRpc();
                    }
                    
                }
                break;
        }
    }

    public void OnDry()
    {
        if (!root.hasCombat) return;

        if (root.combat.isDrying)
        {
            root.combat.CancelDry();
        }
        else
        {
            root.combat.BeginDry();
        }
    }

    public void OnThrowable()
    {
        if (!root.combat.isAlive) return;
        if (!acceptInput) return;
        if (!root.hasWeapon) return;

        root.weapon.ToggleThrowable();
    }

    public void EnableInput()
    {
        _acceptInput = true;
    }

    public void DisableInput()
    {
        _acceptInput = false;
        root.weapon.StopShoot();
        _moveDirection = Vector2.zero;
    }
}