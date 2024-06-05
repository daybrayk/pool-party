using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Daybrayk;
using Daybrayk.rpg;
[RequireComponent(typeof(CharacterRoot))]
public class CharacterMovement : NetworkBehaviour
{
    public enum Orientation
    {
        South,
        SouthWest,
        West,
        NorthWest,
        North,
        NorthEast,
        East,
        SouthEast,
    }


    public float moveSpeed => _moveSpeedStat.value;
    [SerializeField]
    Stat _moveSpeedStat;
    public Stat moveSpeedStat => _moveSpeedStat;
    [SerializeField]
    float _stableMovementSharpness = 15f;
    public float stableMovementSharpness => _stableMovementSharpness;
    Rigidbody2D _rigidbody;
    new public Rigidbody2D rigidbody => _rigidbody; 
    public Vector2 currentVelocity { get; private set; }
    [SerializeField]
    WeaponGimble handle;

    [Header("Dodge")]
    [SerializeField]
    float dodgeDistance;
    [SerializeField]
    float dodgeDuration;

	CharacterRoot root;
    public Orientation characterOrientation { get; private set; }
    Vector2 externalVelocity;
    private void Awake()
    {
        root = GetComponent<CharacterRoot>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    [ServerRpc]
    public void UpdateVelocityServerRpc(Vector2 desiredDirection)
    {
        UpdateVelocity(desiredDirection);
    }

    [ClientRpc]
    public void UpdateVelocityClientRpc(Vector2 desiredDirection)
    {
        if (IsHost) return;

        UpdateVelocity(desiredDirection);
    }

    public void UpdateVelocity(Vector2 desiredDirection)
    {
        Vector2 targetMovementVelocity = Vector2.zero;

        targetMovementVelocity = (desiredDirection * moveSpeed) + externalVelocity;

        if (targetMovementVelocity != Vector2.zero) root.weapon.CancelRefill();

        currentVelocity = Vector2.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-stableMovementSharpness * Time.deltaTime));

        root.visualization.SetAnimParam(ConstantValues.ANIMATION_SPEED, (int)currentVelocity.sqrMagnitude);

        _rigidbody.velocity = currentVelocity;
    }

    private void LateUpdate()
    {
        externalVelocity = Vector2.zero;
    }

    public void AddExternalVelocity(Vector2 velocity)
    {
        externalVelocity = velocity;
    }


    public void UpdateOrientation(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        Orientation o = Orientation.South;
        if (angle > -45 && angle <= 45)
        {
            o = Orientation.East;
            root.visualization.SetAnimParam(ConstantValues.ANIMATION_AIM, 3);
        }
        else if (angle > 45 && angle <= 135)
        {
            o = Orientation.North;
            root.visualization.SetAnimParam(ConstantValues.ANIMATION_AIM, 2);
        }
        else if (angle > 135 || angle <= -135)
        {
            o = Orientation.West;
            root.visualization.SetAnimParam(ConstantValues.ANIMATION_AIM, 1);
        }
        else if (angle > -135 && angle <= -45)
        {
            o = Orientation.South;
            root.visualization.SetAnimParam(ConstantValues.ANIMATION_AIM, 0);
        }

        characterOrientation = o;

        if (IsServer) UpdateOrientationClientRpc((int) o);
        else
        {
            UpdateOrientationServerRpc((int)o);
        }
    }

    [ServerRpc]
    public void UpdateOrientationServerRpc(int orientation)
    {
        characterOrientation = (Orientation)orientation;
        root.visualization.SetAnimParam(ConstantValues.ANIMATION_AIM, orientation);
        UpdateOrientationClientRpc(orientation);
    }

    [ClientRpc]
    public void UpdateOrientationClientRpc(int orientation)
    {
        if (IsOwner || IsServer) return;
        characterOrientation = (Orientation)orientation;
        root.visualization.SetAnimParam(ConstantValues.ANIMATION_AIM, orientation);
    }
}