using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct PlayerInputs
{
    public Vector2 moveDirection;
    public Vector2 aimDirection;
}

public class InputListener : MonoBehaviour
{
    [SerializeField]
    CharacterInputHandler inputHandler;

    InputReader inputReader;

    PlayerInputs inputs;

    private void Start()
    {
        Debug.Assert(inputHandler != null, "No Input Handler was provided, you missed adding a reference in the inspector", this);
        inputs = new PlayerInputs();
        inputs.moveDirection = Vector2.zero;
    }

    private void OnEnable()
    {
        EnableListeners();
    }

    private void OnDisable()
    {
        DisableListeners();
    }

    void EnableListeners()
    {
        inputReader.moveEvent += OnMove;
        inputReader.aimEvent += OnAim;
        inputReader.fireEventStart += OnFire;
    }

    void DisableListeners()
    {
        inputReader.moveEvent -= OnMove;
        inputReader.aimEvent -= OnAim;
        inputReader.fireEventStart -= OnFire;
    }

    #region Action Listeners
    public void OnMove(Vector2 value)
    {
        inputs.moveDirection = value;
    }

    public void OnAim(Vector2 value)
    {
        inputs.aimDirection = value;
    }

    public void OnFire()
    {
        
    }

    public void OnFireCancel()
    {
        
    }
    #endregion
}