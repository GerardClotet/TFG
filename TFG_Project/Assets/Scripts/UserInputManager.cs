using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem;

//Modify events parameters through making child classes of UnityEvent
[Serializable] public class Vector2InputEvent : UnityEvent<float, float> { }
[Serializable] public class PLAYER_STATE_InputEvent : UnityEvent<PLAYER_STATE> { }

public class UserInputManager : MonoBehaviour
{
    
    //Events
    Vector2InputEvent moveInputEvent;
    UnityEvent jumpEvent;
    UnityEvent dashEvent;
    User userActionInput;

    PLAYER_STATE_InputEvent requestChangeStateEvent;

    private void Awake()
    {
        userActionInput = new User();
    }
    private void OnEnable() //This is done in OnEnable func because  maybe que wantit to desable 
    {
       // EnablePlayerInput();
    }
    private void Start()
    {
        EnablePlayerInput();
    }

    public void EnablePlayerInput()
    {
        moveInputEvent = new Vector2InputEvent();
        jumpEvent = new UnityEvent();
        dashEvent = new UnityEvent();
        requestChangeStateEvent = new PLAYER_STATE_InputEvent();


        moveInputEvent.AddListener(Player.Instance.OnMoveInput);
        requestChangeStateEvent.AddListener(Player.Instance.RequestChangePlayerState);
        //sets the player input map active
        userActionInput.Player.Enable();

        //move
        userActionInput.Player.Move.started += context => requestChangeStateEvent.Invoke(PLAYER_STATE.MOVE);
        userActionInput.Player.Move.performed += OnMovePerformed;
        userActionInput.Player.Move.canceled += context => requestChangeStateEvent.Invoke(PLAYER_STATE.IDLE);
        //jump
        userActionInput.Player.Jump.performed += context => jumpEvent.Invoke();
        //dash
        userActionInput.Player.Dash.performed += context => dashEvent.Invoke();
    }


    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveInputEvent.Invoke(moveInput.x, moveInput.y);
    }

    //for single float events
    //private void onPushPerformed(InputAction.CallbackContext context)
    //{
    //    pushInputEvent.Invoke(context.ReadValue<float>());
    //}
}
