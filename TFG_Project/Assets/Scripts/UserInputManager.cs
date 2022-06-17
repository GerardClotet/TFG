using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem;


/// <summary>
/// Handles & triggers the events related to inputs
/// </summary>
public class UserInputManager : MonoBehaviour
{
    public static UserInputManager Instance { get; private set; }

    /// <summary>
    /// Use public static Func<T> if we want a delegate that returns a value
    /// </summary>
    public Action<float, float> moveInputEvent; //gets input but doesn't return anything. It's like a delegate
    public Action<PLAYER_STATE> requestChangeStateEvent;
    public Action jumpEvent;
    public Action jumpCanceled;
    public Action dashEvent;
    public Action jumpStarted;
    public Action <object> openMenu;
    public Action closeMenu;
    User userActionInput;

    private void Awake()
    {
        Instance = this;
        userActionInput = new User();
        EnablePlayerInput();
        LinkEvents();
    }
    private void LinkEvents()
    {

        //move
        userActionInput.Player.Move.started += context => requestChangeStateEvent.Invoke(PLAYER_STATE.MOVE);
        userActionInput.Player.Move.performed += context => moveInputEvent.Invoke(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
        userActionInput.Player.Move.canceled += context => requestChangeStateEvent.Invoke(PLAYER_STATE.IDLE);
        userActionInput.Player.Move.canceled += context => moveInputEvent.Invoke(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
        //jump
        userActionInput.Player.Jump.started += context => requestChangeStateEvent.Invoke(PLAYER_STATE.JUMP);
        userActionInput.Player.Jump.performed += context => jumpEvent.Invoke();
        userActionInput.Player.Jump.canceled += context => jumpCanceled.Invoke();
        //dash
        userActionInput.Player.Dash.started += context => requestChangeStateEvent.Invoke(PLAYER_STATE.HOLD_DASH);
        userActionInput.Player.Dash.started += context => dashEvent.Invoke();
        //Start
        userActionInput.Player.Menu.started += context => openMenu.Invoke(context);
        userActionInput.UI.Start.started += context => closeMenu.Invoke();

        openMenu += func => { DisablePlayerInput(); EnableUiInput(); };
    }
    private void OnEnable() 
    {
       EnablePlayerInput();
    }

    private void OnDisable()
    {
        DisablePlayerInput();
    }

    public void EnablePlayerInput()
    {
        userActionInput.Player.Enable();
    }

    public void DisablePlayerInput()
    {
        //userActionInput.Player.Move.started -= context => requestChangeStateEvent(PLAYER_STATE.MOVE);
        //userActionInput.Player.Move.performed -= context => moveInputEvent(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
        //userActionInput.Player.Move.canceled -= context => requestChangeStateEvent(PLAYER_STATE.IDLE);
        //userActionInput.Player.Jump.performed -= context => jumpEvent();
        //userActionInput.Player.Dash.performed -= context => dashEvent();
        userActionInput.Player.Disable();
    }

    public void EnableUiInput()
    {
        userActionInput.UI.Enable();
    }

    public void DisableUiInput()
    {
        userActionInput.UI.Disable();
    }
}
