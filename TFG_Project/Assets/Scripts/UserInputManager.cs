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
    public static Action dashEvent;

    User userActionInput;

    private void Awake()
    {
        Instance = this;
        userActionInput = new User();
    }

    private void OnEnable() //This is done in OnEnable func because  maybe que wantit to desable 
    {
       EnablePlayerInput();
    }

    private void OnDisable()
    {
        DisablePlayerInput();
    }

    public void EnablePlayerInput()
    {
        //sets the player input map active
        userActionInput.Player.Enable();

        //move
        userActionInput.Player.Move.started += context => requestChangeStateEvent.Invoke(PLAYER_STATE.MOVE);
        userActionInput.Player.Move.performed += context => moveInputEvent.Invoke(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
        userActionInput.Player.Move.canceled += context => requestChangeStateEvent.Invoke(PLAYER_STATE.IDLE);
        //jump
        userActionInput.Player.Jump.performed += context => jumpEvent.Invoke();
        //dash
        userActionInput.Player.Dash.performed += context => dashEvent.Invoke();
    }

    public void DisablePlayerInput()
    {
        userActionInput.Player.Move.started -= context => requestChangeStateEvent(PLAYER_STATE.MOVE);
        userActionInput.Player.Move.performed -= context => moveInputEvent(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
        userActionInput.Player.Move.canceled -= context => requestChangeStateEvent(PLAYER_STATE.IDLE);
        userActionInput.Player.Jump.performed -= context => jumpEvent();
        userActionInput.Player.Dash.performed -= context => dashEvent();
        userActionInput.Player.Disable();
        
    }

    //for single float events
    //private void onPushPerformed(InputAction.CallbackContext context)
    //{
    //    pushInputEvent.Invoke(context.ReadValue<float>());
    //}
}
