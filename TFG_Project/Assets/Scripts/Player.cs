using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYER_STATE
{
    IDLE,
    MOVE,
    JUMP,
    HOLD_DASH,
    DASH,
    ON_AIR_DASH,
    ON_AIR,
    GRAB_WALL,
    BOUNCE,
    BOUNCE_AIR
}
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private Vector2 leftStick;
    private PLAYER_STATE playerState = PLAYER_STATE.IDLE;

    [Header("Layer")]
    [SerializeField]    private LayerMask plaftormLayer;
    [SerializeField]    private LayerMask bounceLayer;

    [Header("Movement Stats")]
    [SerializeField] private float playerSpeed = 10;
    [SerializeField] private float playerHorizontalMaxVelocity = 10f;    
    [SerializeField] private float cancelRateJump = 100f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpButtonTime = 0.5f;
    [SerializeField] private float coyoteJumpTime = 0.2f;
    [SerializeField] private float fallingGravityScale = 10f;
    [SerializeField] private float bounceImpulse = 5f;
    [Range(0.5f,1f)] [SerializeField] private float bounceInclination = 0.5f;
    [Range(-10,-0.1f)][SerializeField] private float grabDownfallVelocity = -1f;
    [SerializeField] private float bounceDistance = 10;
    [SerializeField] private float cancelRateBounce = 30f;
    [Range(0.01f, 1f)] [SerializeField] private float dashHoldTime = 0.5f;
    [SerializeField] private float dashImpulse = 500f;
    [SerializeField] private float cancelRateDash = 40f;
    [Tooltip("Value that is MULTIPLIED with the playerSpeed")]
    [Range(0.01f, 1f)] [SerializeField] private float onAirFriction = 1.0f;


    private     Rigidbody2D rigidBody2D;
    private     BoxCollider2D boxCollider2D;
    private     UserInputManager inputManager;
    private     Animator playerAnimator;

    private     bool jumpHeld = false;
    private     float jumpTime = 0f;
    private     bool jumping = false;
    private     float coyoteJumpCounter;
    private     float defaultGravityScale = 1f;
    private     Vector2 grabNormal = Vector2.zero;
    private     bool canDash = true;
    private     float auxDashHoldTime = 0f;
    private     bool stopCoroutineActive = false;
    private     Vector3 spawnPos = Vector3.zero;

    private static int s_BounceLayer = 7;
    private static int s_DieLayer = 8;
    private static int s_FinishLayer = 9;

    public Action dashAction;
    public Action groundedAction;
    public Action endGame;
    public Action dieAction;
    public Action getCollectableMultiplierAction;

    private void Awake()
    {
        Instance = this;
        coyoteJumpCounter = coyoteJumpTime;
        inputManager = UserInputManager.Instance;
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerAnimator = GetComponentInChildren<Animator>();
        dieAction += ResetPlayer;
    }

    private void OnEnable()
    {
        if (inputManager)
        {
            inputManager.moveInputEvent += OnMoveInput;
            inputManager.requestChangeStateEvent += RequestChangePlayerState;
            inputManager.jumpEvent += JumpPerformed;
            inputManager.jumpCanceled += JumpCancel;
            inputManager.dashEvent += DashStarted;
        }
    }

    private void OnDisable()
    {
        if (inputManager)
        {
            inputManager.moveInputEvent -= OnMoveInput;
            inputManager.requestChangeStateEvent -= RequestChangePlayerState;
            inputManager.jumpEvent -= JumpPerformed;
            inputManager.jumpCanceled -= JumpCancel;
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
        if(jumping)
        {
            jumpTime += Time.deltaTime;

            if(jumpTime >= jumpButtonTime)
            {
                jumping = false;
            }
        }

        if (canDash == false && playerState == PLAYER_STATE.HOLD_DASH)
        {
            auxDashHoldTime += Time.deltaTime;

            if (auxDashHoldTime >= dashHoldTime)
            {
                auxDashHoldTime = 0f;
                playerState = PLAYER_STATE.DASH;
            }
        }

        if (IsGrounded(plaftormLayer))
        {
            if(playerState == PLAYER_STATE.ON_AIR || playerState == PLAYER_STATE.BOUNCE_AIR || playerState == PLAYER_STATE.ON_AIR_DASH )
            {
                GroundedReset();
            }
            else if(playerState == PLAYER_STATE.GRAB_WALL)
            {
                GroundedReset();

                if (leftStick != Vector2.zero)
                {
                    playerState = PLAYER_STATE.MOVE;
                }
                else
                {
                    playerState = PLAYER_STATE.IDLE;
                }
            }
        }
        else if (playerState == PLAYER_STATE.IDLE || playerState == PLAYER_STATE.MOVE) 
        {
            if (!IsGrounded(bounceLayer))
            {
                playerState = PLAYER_STATE.ON_AIR;
            }
        }
        else if (coyoteJumpCounter > 0)
        {
            coyoteJumpCounter -= Time.deltaTime;
        }
    }

    private void GroundedReset() 
    {
        groundedAction.Invoke();
        playerAnimator.SetTrigger("GroundTrigger");
        canDash = true;
        jumpTime = 0f;
        jumping = false;
        rigidBody2D.gravityScale = defaultGravityScale;
        coyoteJumpCounter = coyoteJumpTime;
        if (leftStick != Vector2.zero)
        {
            playerState = PLAYER_STATE.MOVE;
        }
        else
        {
            playerState = PLAYER_STATE.IDLE;
        }
    }

    private bool IsGrounded(LayerMask layer)
    {
        float extraHeightText = 0.05f;
        RaycastHit2D raycastHitFloor = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, extraHeightText, layer);
        return raycastHitFloor.collider != null;
    }

    private void FixedUpdate()
    {

        switch(playerState)
        {
            case PLAYER_STATE.IDLE:
                break;

            case PLAYER_STATE.MOVE:
                Move();
                break;

            case PLAYER_STATE.JUMP:
                if(rigidBody2D.gravityScale != defaultGravityScale)
                {
                    rigidBody2D.gravityScale = defaultGravityScale;
                }
                float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigidBody2D.gravityScale));
                rigidBody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                RequestChangePlayerState(PLAYER_STATE.ON_AIR);
                break;

            case PLAYER_STATE.ON_AIR:
                MoveOnAir();
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
                break;

            case PLAYER_STATE.BOUNCE:
                rigidBody2D.AddForce(new Vector2(grabNormal.x, bounceInclination) * bounceImpulse, ForceMode2D.Impulse);
                grabNormal = Vector2.zero;

                playerState = PLAYER_STATE.BOUNCE_AIR;
                break;

            case PLAYER_STATE.BOUNCE_AIR:
                if (rigidBody2D.velocity.y > 0)
                { 
                    if(Mathf.Abs(rigidBody2D.velocity.x) < 1.5f)
                    {
                        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, -0.5f);
                    }
                    rigidBody2D.AddForce(new Vector2(Mathf.Sign(rigidBody2D.velocity.x), -1) * cancelRateBounce);
                    rigidBody2D.AddForce(Vector2.down * cancelRateBounce, ForceMode2D.Force);
                }

                if (rigidBody2D.gravityScale != fallingGravityScale)
                {
                    rigidBody2D.gravityScale = fallingGravityScale;
                }
                else
                {
                    MoveOnAir();
                }
                break;

            case PLAYER_STATE.HOLD_DASH:
                    rigidBody2D.velocity = Vector2.zero;
                    rigidBody2D.gravityScale = 0f;
                break;

            case PLAYER_STATE.DASH:
                rigidBody2D.AddForce(leftStick.normalized * dashImpulse, ForceMode2D.Impulse);
                dashAction.Invoke();
                RequestChangePlayerState(PLAYER_STATE.ON_AIR_DASH);
                break;

            case PLAYER_STATE.ON_AIR_DASH:
                if(rigidBody2D.velocity.y >0 && Mathf.Abs(rigidBody2D.velocity.x) > 0)
                {
                    rigidBody2D.AddForce(new Vector2(Mathf.Sign(rigidBody2D.velocity.x), -1) * cancelRateDash);
                }
                else if (rigidBody2D.gravityScale != fallingGravityScale)
                {
                    rigidBody2D.gravityScale = fallingGravityScale;
                }
                MoveOnAir();
                break;
        }
    }

    private void Move()
    {
        if(leftStick != Vector2.zero)
        {
            if (playerHorizontalMaxVelocity > Mathf.Abs(rigidBody2D.velocity.x) && playerState == PLAYER_STATE.MOVE)
            {
                float x = leftStick.x > 0 ? 1 : -1;
                rigidBody2D.velocity += new Vector2(x, 0) * playerSpeed * Time.deltaTime;
            }
        }
    }

    private void MoveOnAir()
    {
        if (leftStick != Vector2.zero )
        {
            if (playerHorizontalMaxVelocity > Mathf.Abs(rigidBody2D.velocity.x))
            {
                float x = leftStick.x > 0 ? 1 : -1;
                rigidBody2D.velocity += new Vector2(x, 0) * (playerSpeed*onAirFriction) * Time.deltaTime;
            }
            else if(Mathf.Sign(playerHorizontalMaxVelocity) != Mathf.Sign(rigidBody2D.velocity.x))
            {
                float x = leftStick.x > 0 ? 1 : -1;
                rigidBody2D.velocity += new Vector2(x, 0) * (playerSpeed * onAirFriction) * Time.deltaTime;
            }
        }
    }

    private void JumpPerformed()
    {
        jumpHeld = true;
    }

    private void JumpCancel()
    {
        jumpHeld = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == s_BounceLayer)
        {
            if (collision.GetContact(0).normal.y == 1)
            {
                if (playerState != PLAYER_STATE.MOVE && playerState != PLAYER_STATE.IDLE)
                {
                    GroundedReset();
                    return;
                }
                else return;
            }

            if (rigidBody2D.gravityScale != defaultGravityScale)
            {
                rigidBody2D.gravityScale = defaultGravityScale;
            }
            RequestChangePlayerState(PLAYER_STATE.GRAB_WALL);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == s_DieLayer)
        {
            dieAction.Invoke();
        }
        else if(collision.gameObject.layer == s_FinishLayer )
        {
            endGame.Invoke();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == s_BounceLayer)
        {
            if (collision.GetContact(0).normal.y == 1)
            {
                if (playerState != PLAYER_STATE.MOVE && playerState != PLAYER_STATE.IDLE && playerState != PLAYER_STATE.HOLD_DASH && playerState != PLAYER_STATE.DASH)
                {
                    if (rigidBody2D.velocity.y < 0)
                    {
                        GroundedReset();
                    }
                    return;
                }
                return;
            }
            else
            {
                if(playerState == PLAYER_STATE.GRAB_WALL)
                {
                    grabNormal = collision.GetContact(0).normal;
                    if(leftStick.x <0 && grabNormal.x <0 || leftStick.x > 0 && grabNormal.x > 0)
                    {
                        MoveOnAir();
                    }
                    else
                    {
                        rigidBody2D.velocity += new Vector2(0, grabDownfallVelocity);
                    }
                }
                else
                {
                    RequestChangePlayerState(PLAYER_STATE.GRAB_WALL);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == s_BounceLayer)
        {
            if(playerState == PLAYER_STATE.GRAB_WALL)
            {
                playerState = jumping ? PLAYER_STATE.ON_AIR : PLAYER_STATE.BOUNCE_AIR;
            }
        }
    }

    private void DashStarted()
    {
    }

    //here we check if we can properly change player state
    //This is called when InputActions are cancelled
    public void RequestChangePlayerState(PLAYER_STATE state)
    {
        if(state != PLAYER_STATE.IDLE && stopCoroutineActive)
        {
            stopCoroutineActive = false;
            StopAllCoroutines();
        }

        switch(state)
        {
            case PLAYER_STATE.MOVE: 
                if (playerState == PLAYER_STATE.IDLE)
                {
                    playerState = state;
                }
                break;

            case PLAYER_STATE.IDLE:
                if (playerState == PLAYER_STATE.MOVE)
                {
                    if(!stopCoroutineActive)
                    {
                        StartCoroutine(StopPlayerHorizontalMovement());
                    }
                    leftStick = Vector2.zero;
                    playerState = state;
                }
                break;

            case PLAYER_STATE.JUMP:
                if(playerState == PLAYER_STATE.GRAB_WALL)
                {
                    playerState = PLAYER_STATE.BOUNCE;
                    return;
                }

                if (playerState == PLAYER_STATE.MOVE || playerState == PLAYER_STATE.IDLE || coyoteJumpCounter > 0)
                {
                    coyoteJumpCounter = -1;
                    jumping = true;
                    playerState = state;
                    playerAnimator.SetTrigger("JumpTrigger");
                }
                break;

            case PLAYER_STATE.GRAB_WALL:
                if(playerState == PLAYER_STATE.BOUNCE_AIR || playerState == PLAYER_STATE.ON_AIR || playerState == PLAYER_STATE.ON_AIR_DASH)
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

            case PLAYER_STATE.HOLD_DASH:
                if (playerState != PLAYER_STATE.HOLD_DASH && canDash == true)
                {
                    canDash = false;
                    playerState = state;
                }
                break;

            case PLAYER_STATE.ON_AIR_DASH:
                if(playerState != PLAYER_STATE.JUMP || playerState != PLAYER_STATE.ON_AIR)
                {
                    playerState = state;
                }
                break;

            case PLAYER_STATE.ON_AIR:

                if (playerState != PLAYER_STATE.HOLD_DASH && playerState != PLAYER_STATE.DASH && playerState != PLAYER_STATE.ON_AIR_DASH)
                {
                    playerState = state;
                }
                break;
        }
    }

    IEnumerator StopPlayerHorizontalMovement() //TODO
    {
        stopCoroutineActive = true;
        float startVel = rigidBody2D.velocity.x;
        float reduceStep = startVel / 1000f;
        while (Mathf.Abs(rigidBody2D.velocity.x) > 0)
        {
            reduceStep += reduceStep;
            float nVel = Mathf.Lerp(startVel, 0, Mathf.Abs(reduceStep));
            rigidBody2D.velocity = new Vector2(nVel, rigidBody2D.velocity.y);
            yield return null;
        }
        stopCoroutineActive = false;
    }
    
    public void SetSpawnPoint(Vector3 pos)
    {
        spawnPos = pos;
    }
    
    private void ResetPlayer()
    {
        StopCoroutine(StopPlayerHorizontalMovement());

        canDash = true;
        jumpTime = 0f;
        jumping = false;
        rigidBody2D.gravityScale = defaultGravityScale;
        coyoteJumpCounter = coyoteJumpTime;
        if (leftStick != Vector2.zero)
        {
            playerState = PLAYER_STATE.MOVE;
        }
        else
        {
            playerState = PLAYER_STATE.IDLE;
        }
        transform.position = spawnPos;
        rigidBody2D.velocity = Vector2.zero;
    }


    public void ResetDashCollectable()
    {
        canDash = true;
        //Todo SpawnParticles or something
    }

    public void GetCollectableMultiplier()
    {
        getCollectableMultiplierAction.Invoke();
    }

}
