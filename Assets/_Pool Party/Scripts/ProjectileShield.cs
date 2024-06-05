using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShield : MonoBehaviour
{
    [SerializeField]
    Transform[] positions;
    Quaternion horizontalRotation = Quaternion.Euler(0, 0, 90);

    public void UpdateOrientation(CharacterMovement.Orientation orientation)
    {
        switch (orientation)
        {
            case CharacterMovement.Orientation.South:
                transform.position = positions[0].position;
                transform.rotation = horizontalRotation;
                break;
            case CharacterMovement.Orientation.SouthWest:
                break;
            case CharacterMovement.Orientation.West:
                transform.position = positions[1].position;
                transform.rotation = Quaternion.identity;
                break;
            case CharacterMovement.Orientation.NorthWest:
                break;
            case CharacterMovement.Orientation.North:
                transform.position = positions[2].position;
                transform.rotation = horizontalRotation;
                break;
            case CharacterMovement.Orientation.NorthEast:
                break;
            case CharacterMovement.Orientation.East:
                transform.position = positions[3].position;
                transform.rotation = Quaternion.identity;
                break;
            case CharacterMovement.Orientation.SouthEast:
                break;
        }
    }
}
