using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input Reader")]
public class InputReader : ScriptableObject//, DefaultActions.IPlayerActions, DefaultActions.IUIActions
{
	public UnityAction<Vector2> moveEvent;
	public UnityAction<Vector2> aimEvent;
	public UnityAction fireEventStart;
    public UnityAction fireEVentEnd;

    public UnityAction<Vector2> navigateEvent;
    public UnityAction submitEvent;
    public UnityAction cancelEvent;
    public UnityAction<Vector2> pointEvent;
    public UnityAction clickEvent;
    public UnityAction<Vector2> scrollWheelEvent;
    public UnityAction middleClickEvent;
    public UnityAction rightClickEvent;

    DefaultActions inputActions;

    private void OnEnable()
    {
        if(inputActions == null)
        {
            inputActions = new DefaultActions();
            //inputActions.Player.SetCallbacks(this);
        }
        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void EnableGameplayInput()
    {
        inputActions.Player.Enable();
        inputActions.UI.Disable();
    }

    public void EnableUIInput()
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();
    }

    public void DisableAllInput()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    #region Player Actions
    public void OnAim(InputAction.CallbackContext context)
    {
        aimEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed) fireEventStart.Invoke();
        else if(context.canceled)
        {
            fireEVentEnd.Invoke();
        }
    }

    public void OnFireReleased(InputAction.CallbackContext context)
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRecharge(InputAction.CallbackContext context)
    {

    }
    #endregion

    #region UI Actions
    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    public void OnCancel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}