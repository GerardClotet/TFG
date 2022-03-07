using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYER_STATE
{
    IDLE,
    MOVE,
    JUMP,
    DASH
}
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private Vector2 leftStick;
    private PLAYER_STATE playerState = PLAYER_STATE.IDLE;

    [Header("Movement Stats")]
    [SerializeField] private float playerSpeed = 1;


    private void Awake()
    {
        Instance = this; //Check if there is a better way than using singleton
        GetComponent<UserInputManager>().EnablePlayerInput(); //temporary execution order doesn't work properly
    }

    public void OnMoveInput(float x, float y)
    {
        leftStick.x = x;
        leftStick.y = y;
    }


    private void Update()
    {
        switch (playerState)
        {
            case PLAYER_STATE.IDLE:
                break;
            case PLAYER_STATE.MOVE:
                Move(leftStick);
                break;
            case PLAYER_STATE.JUMP:
                break;
            case PLAYER_STATE.DASH:
                break;
        }
    }

    private void Move(Vector2 v2)
    {
        if(v2 != Vector2.zero && v2 != null)
        {
            Vector3 dst = transform.position + new Vector3(v2.x, 0, 0) * playerSpeed * Time.deltaTime;
            transform.position = dst;
        }
       
    }

    //here we check if we can properly change player state
    //This is called when InputActions are cancelled
    public void RequestChangePlayerState(PLAYER_STATE state)
    {
        switch(state)
        {
            case PLAYER_STATE.IDLE: //requests idle
                ChangeState(PLAYER_STATE.MOVE);
                break;
            case PLAYER_STATE.MOVE:
                ChangeState(PLAYER_STATE.IDLE);
                break;
        }
    }

    private void ChangeState(PLAYER_STATE state)
    {
        playerState = state; 
    }
    
}
