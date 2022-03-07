using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem;

//Modify events parameters through making child classes of UnityEvent
[Serializable] public class Vector2InputEvent : UnityEvent<float, float> { }

public class UserInputManager : MonoBehaviour
{
    
    //Events
    Vector2InputEvent moveInputEvent;
    UnityEvent jumpEvent;
    UnityEvent dashEvent;

    private void Awake()
    {
    }
    private void OnEnable() //This is done in OnEnable func because  maybe que wantit to desable 
    {

    }


    private void EnablePlayerInput()
    {
        moveInputEvent = new Vector2InputEvent();
        jumpEvent = new UnityEvent();
        dashEvent = new UnityEvent();


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
