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
    ON_AIR,
    GRAB_WALL,
    BOUNCE,
    BOUNCE_AIR
}
public class Player : MonoBehaviour
{
    private Vector2 leftStick;
    private PLAYER_STATE playerState = PLAYER_STATE.IDLE;

    [Header("Layer")]
    [SerializeField]    private LayerMask plaftormLayer;

    [Header("Movement Stats")]
    [SerializeField] private float playerSpeed = 10;
    [SerializeField] private float playerHorizontalMaxVelocity = 10f;    
    [SerializeField] private float cancelRateJump = 100f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpButtonTime = 0.5f;
    [SerializeField] private float fallingGravityScale = 10f;
    [SerializeField] private float bounceImpulse = 5f;
    [Range(0.5f,1f)] [SerializeField] private float bounceInclination = 0.5f;
    [Range(-10,-0.1f)][SerializeField] private float grabDownfallVelocity = -1f;
    [SerializeField] private float cancelRateBounce = 30f;

    private     Rigidbody2D rigidBody2D;
    private     BoxCollider2D boxCollider2D;
    private     UserInputManager inputManager;
    private     bool jumpHeld = false;
    private     float jumpTime = 0f;
    private     bool jumping = false;
    private     float defaulGravityScale = 1f;
    private     Vector2 grabNormal = Vector2.zero; 

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
        Debug.Log(rigidBody2D.velocity);
        if(jumping)
        {
            jumpTime += Time.deltaTime;


            if(jumpTime >= jumpButtonTime)
            {
                jumping = false;
            }
        }

        if (playerState == PLAYER_STATE.ON_AIR && IsGrounded() || playerState == PLAYER_STATE.GRAB_WALL && IsGrounded() || playerState == PLAYER_STATE.BOUNCE_AIR && IsGrounded()) //Not sure, TODO Comprobació més neta, no pasant a cada frame al Update
        {
            jumpTime = 0f;
            jumping = false;
            rigidBody2D.gravityScale = defaulGravityScale;
            if (leftStick != Vector2.zero)
            {
                playerState = PLAYER_STATE.MOVE;
            }
            else
            {
                playerState = PLAYER_STATE.IDLE;
            }
        }
        else if(playerState != PLAYER_STATE.ON_AIR && playerState != PLAYER_STATE.GRAB_WALL && playerState != PLAYER_STATE.BOUNCE && !IsGrounded() && playerState != PLAYER_STATE.BOUNCE_AIR) //TODO Fer que la comprobació sigui de una forma més neta
        {
            playerState = PLAYER_STATE.ON_AIR;
        }
    }


    private bool IsGrounded()
    {
        float extraHeightText = 0.05f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, extraHeightText, plaftormLayer);
        //Color rayColor;
        //if (raycastHit.collider != null)
        //{
        //    rayColor = Color.green;
        //}
        //else
        //{
        //    rayColor = Color.red;
        //}
        //Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        //Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        //Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + extraHeightText), Vector2.right * (boxCollider2D.bounds.extents.y + extraHeightText), rayColor);
        return raycastHit.collider != null;
    }

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
                float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigidBody2D.gravityScale));
                rigidBody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                Move(leftStick);
                playerState = PLAYER_STATE.ON_AIR;
                break;


            case PLAYER_STATE.ON_AIR:
                MoveOnAir(leftStick);
                if (!jumpHeld && jumping && rigidBody2D.velocity.y > 0)
                {
                    rigidBody2D.AddForce(Vector2.down * cancelRateJump);
                }
                else if (rigidBody2D.velocity.y <= 0 && rigidBody2D.gravityScale != fallingGravityScale)
                {
                    rigidBody2D.gravityScale = fallingGravityScale;
                }
                break;

            case PLAYER_STATE.GRAB_WALL:
                rigidBody2D.velocity += new Vector2(0, grabDownfallVelocity);
                Move(leftStick);
                break;

            case PLAYER_STATE.BOUNCE:
                Move(leftStick);
                rigidBody2D.AddForce(new Vector2(grabNormal.x, bounceInclination)*bounceImpulse);
                grabNormal = Vector2.zero;
                playerState = PLAYER_STATE.BOUNCE_AIR;
                break;

            case PLAYER_STATE.BOUNCE_AIR:
                MoveOnAir(leftStick);
                if (rigidBody2D.velocity.y > 0)
                {
                    rigidBody2D.AddForce(Vector2.down * cancelRateBounce); 
                }
                else if(rigidBody2D.gravityScale != fallingGravityScale)
                {
                    rigidBody2D.gravityScale = fallingGravityScale;
                }
                //else 
                //{
                //    MoveOnAir(leftStick);
                //}
                break;

            case PLAYER_STATE.DASH:
                break;
        }
    }

    private void Move(Vector2 v2)
    {
        if(v2 != Vector2.zero && v2 != null)
        {
            if (playerHorizontalMaxVelocity > Mathf.Abs(rigidBody2D.velocity.x))
            {
                float x = v2.x > 0 ? 1 : -1;
                rigidBody2D.velocity += new Vector2(x, 0) * playerSpeed * Time.deltaTime;
            }
        }
    }

    private void MoveOnAir(Vector2 v2)
    {
        if (v2 != Vector2.zero && v2 != null)
        {
            if (playerHorizontalMaxVelocity > Mathf.Abs(rigidBody2D.velocity.x))
            {
                float x = v2.x > 0 ? 1 : -1;
                rigidBody2D.velocity += new Vector2(x, 0) * (playerSpeed/1.4f) * Time.deltaTime;
            }
        }
    }

    //jump + move?
    private void JumpPerformed()//maybe a coroutine?
    {
        jumpHeld = true;
    }

    private void JumpCancel()
    {
        jumpHeld = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {

            if (rigidBody2D.gravityScale != defaulGravityScale)
            {
                rigidBody2D.gravityScale = defaulGravityScale;
            }
            RequestChangePlayerState(PLAYER_STATE.GRAB_WALL);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            grabNormal = collision.GetContact(0).normal;
            if (playerState != PLAYER_STATE.GRAB_WALL && playerState != PLAYER_STATE.BOUNCE)
            {
                RequestChangePlayerState(PLAYER_STATE.GRAB_WALL);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            if(playerState == PLAYER_STATE.GRAB_WALL)
            {
                playerState = jumping ? PLAYER_STATE.ON_AIR : PLAYER_STATE.BOUNCE_AIR;
            }
        }
    }
    //here we check if we can properly change player state
    //This is called when InputActions are cancelled
    public void RequestChangePlayerState(PLAYER_STATE state)
    {
        switch(state)
        {
            case PLAYER_STATE.MOVE: //requests idle
                if (playerState == PLAYER_STATE.IDLE)
                {
                    leftStick = Vector2.zero;
                    playerState = state;
                }
                break;

            case PLAYER_STATE.IDLE:
                if (playerState == PLAYER_STATE.MOVE)
                {
                    playerState = state;
                }
                break;

            case PLAYER_STATE.JUMP:

                if(playerState == PLAYER_STATE.GRAB_WALL)
                {
                    playerState = PLAYER_STATE.BOUNCE;
                    return;
                }

                if (playerState != PLAYER_STATE.ON_AIR && playerState != PLAYER_STATE.JUMP && playerState != PLAYER_STATE.BOUNCE_AIR)
                {
                    jumping = true;
                    playerState = state;
                }
                break;

            case PLAYER_STATE.GRAB_WALL:
                if(playerState != PLAYER_STATE.MOVE && playerState != PLAYER_STATE.IDLE && playerState != PLAYER_STATE.BOUNCE)
                {
                    playerState = state;
                }
                break;

            case PLAYER_STATE.BOUNCE:
                if(playerState == PLAYER_STATE.GRAB_WALL)
                {
                    playerState = state;
                }
                break;
        }
    }
}
