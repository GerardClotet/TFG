using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYER_STATE
{
    IDLE,
    MOVE,
    JUMP,
    DASH,
    ON_AIR
}
public class Player : MonoBehaviour
{
    private Vector2 leftStick;
    private PLAYER_STATE playerState = PLAYER_STATE.IDLE;

    [Header("Layer")]
    [SerializeField]    private LayerMask plaftormLayer;

    [Header("Movement Stats")]
    [SerializeField]    private float playerSpeed = 10;
    [Range(0, 5f)]  [SerializeField] private float fallLongMult = 0.85f;
    [Range(0, 5f)]  [SerializeField] private float fallShortMult = 1.55f;
    [Range(4f,10f)] [SerializeField] private float jumpVel = 5f;

    private     Rigidbody2D rigidBody2D;
    private     BoxCollider2D boxCollider2D;
    private     UserInputManager inputManager;
    private     bool jumpHeld = false;

    private void Awake()
    {
        inputManager = UserInputManager.Instance;
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        if (inputManager)
        {
            inputManager.moveInputEvent += OnMoveInput;
            inputManager.requestChangeStateEvent += RequestChangePlayerState;
            inputManager.jumpEvent += JumpPerformed;
            inputManager.jumpCanceled += JumpCancel;
        }
    }

    private void OnDisable()
    {
        if (inputManager)
        {
            inputManager.moveInputEvent -= OnMoveInput;
            inputManager.requestChangeStateEvent -= RequestChangePlayerState;
            inputManager.jumpEvent -= JumpPerformed;
        }
        leftStick = Vector2.zero;
    }

    public void OnMoveInput(float x, float y)
    {
        leftStick.x = x;
        leftStick.y = y;
    }

    private void Update()
    {
        if (playerState == PLAYER_STATE.ON_AIR && IsGrounded())
        {
            if (leftStick != Vector2.zero)
            {
                playerState = PLAYER_STATE.MOVE;
            }
            else
            {
                playerState = PLAYER_STATE.IDLE; //or move idk
            }
        }
    }


    private bool IsGrounded()
    {
        float extraHeightText = 0.09f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, extraHeightText, plaftormLayer);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + extraHeightText), Vector2.right * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);

        return raycastHit.collider != null;
    }

    /// <summary>
    /// physics related
    /// </summary>
    private void FixedUpdate()
    {

        switch(playerState)
        {
            case PLAYER_STATE.IDLE:
                break;

            case PLAYER_STATE.MOVE:
                Move(leftStick);
                break;

            case PLAYER_STATE.JUMP:
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 1 * jumpVel);
                Move(leftStick);
                playerState = PLAYER_STATE.ON_AIR;
                break;
            case PLAYER_STATE.DASH:
                break;

            case PLAYER_STATE.ON_AIR:
                Move(leftStick);
                if (jumpHeld && rigidBody2D.velocity.y > 0)
                {
                    rigidBody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallLongMult - 1) * Time.fixedDeltaTime;
                }
                //else if (!jumpHeld && rigidBody2D.velocity.y > 0)
                //{
                //    rigidBody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallShortMult - 1) * Time.fixedDeltaTime;
                //}
                break;
        }
    }

    private void Move(Vector2 v2)
    {
        if(v2 != Vector2.zero && v2 != null)
        {
            //Vector3 dst = transform.position + new Vector3(v2.x, 0, 0) * playerSpeed * Time.deltaTime;
            //transform.position = dst;
            rigidBody2D.velocity += new Vector2(v2.x,0)*playerSpeed *Time.deltaTime;
        }
    }

    //jump + move?
    private void JumpPerformed()//maybe a coroutine?
    {
        Debug.Log("performed");
        jumpHeld = true;
    }

    private void JumpCancel()
    {
        Debug.Log("Canceled");
        jumpHeld = false;
    }
    //here we check if we can properly change player state
    //This is called when InputActions are cancelled
    public void RequestChangePlayerState(PLAYER_STATE state)
    {
        switch(state)
        {
            case PLAYER_STATE.MOVE: //requests idle
                if (playerState != PLAYER_STATE.JUMP && playerState != PLAYER_STATE.ON_AIR)
                {
                    leftStick = Vector2.zero;
                    playerState = state;
                }
                break;

            case PLAYER_STATE.IDLE:
                if (playerState != PLAYER_STATE.JUMP && playerState != PLAYER_STATE.ON_AIR)
                    playerState = state;
                break;

            case PLAYER_STATE.JUMP:
                if (playerState != PLAYER_STATE.ON_AIR && playerState != PLAYER_STATE.JUMP)
                {
                   // Debug.Log(playerState.ToString());
                    playerState = state;
                }
                break;
        }
    }
}
