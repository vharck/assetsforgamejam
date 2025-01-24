using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovementPlatform : MonoBehaviour
{
    // -> COMPONENTS
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    #region Inputs Management

    private Vector2 input;
    private bool jumpIsHeld = false;

    public void OnMoveCtrl(InputAction.CallbackContext context) => input = context.ReadValue<Vector2>();
    public void OnJumpKey(InputAction.CallbackContext context) 
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                jumpIsHeld = true;
                break;
            case InputActionPhase.Canceled:
                jumpIsHeld = false;
                break;
        }
    }

    #endregion

    #region Casters

    [Header("Casters")]
    [SerializeField] private LayerMask groundLayer;

    private bool onGround = false;

    private void CastCollisions()
    {
        bool _groundHit = Physics2D.Raycast(col.bounds.center, Vector2.down, col.bounds.extents.y + 0.1f);

        if (_groundHit && !onGround)
        {
            onGround = true;
            isCoyoteUsable = true;
            isBufferJumpUsable = true;
            return;
        }

        if (!_groundHit && onGround)
        {
            onGround = false;
            coyoteTimer = Time.time;
            return;
        }
    }

    #endregion

    #region Horizontal Movement

    [Header("Horizontal Movement")]
    [SerializeField] private float maxHorizontalSpeed = 10;
    [SerializeField] private float groundFriction = 2;
    [SerializeField] private float airFriction = 1;
    [SerializeField] private float acceleration = 1;
    private float horizontalSpeed = 0;
    private bool canMove = true;

    public void CanMove(bool value) => canMove = value;

    private void HorizontalMovement()
    {
        if (!canMove) input.x = 0;

        if (input.x != 0)
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, maxHorizontalSpeed * input.x, acceleration * Time.deltaTime);
        else
        {
            float _deceleration = onGround ? groundFriction : airFriction;
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, _deceleration * Time.deltaTime);
        }
    }

    #endregion

    #region Vertical Movement

    [Header("Vertical Movement")]
    [SerializeField] private float jumpSpeed = 20;
    [SerializeField] private float jumpBuffer = 0.1f;
    [SerializeField] private float jumpMaxDuration = 1;
    [SerializeField] private float coyoteDuration = 0.1f;
    private float verticalSpeed = 0;
    private float jumpTimeBegin = 0;
    private float jumpBufferTimer = 0;
    private float coyoteTimer = 0;
    private bool isJumping = false;
    private bool isCoyoteUsable = false;
    private bool isBufferJumpUsable = false;
    private bool canUseJump = false;
    private bool jumpEndedEarly = false;
    private bool jumpWasHeldLastFrame = false;

    public void CanUseJump(bool value) => canUseJump = value;

    private bool JumpInBuffer => isBufferJumpUsable && Time.time < jumpBufferTimer + jumpBuffer;
    private bool CanUseCoyote => isCoyoteUsable && !onGround && Time.time < coyoteTimer + coyoteDuration;
    private bool JumpPressedThisFrame => jumpIsHeld && !jumpWasHeldLastFrame;
    private bool JumpReleasedThisFrame => !jumpIsHeld && jumpWasHeldLastFrame;
    private bool CanJump => onGround || JumpInBuffer || CanUseCoyote;

    private void TryToJump()
    {
        if (!canUseJump)
        {
            jumpWasHeldLastFrame = jumpIsHeld;
            return;
        }

        if (JumpPressedThisFrame && CanJump)
        {
            verticalSpeed = jumpSpeed;
            jumpTimeBegin = Time.time;
            jumpBufferTimer = 0;
            coyoteTimer = 0;
            isBufferJumpUsable = false;
            isCoyoteUsable = false;
            jumpEndedEarly = false;
            isJumping = true;

            jumpWasHeldLastFrame = jumpIsHeld;
            return;
        }

        if (Time.time - jumpTimeBegin < jumpMaxDuration)
        {
            if (jumpIsHeld)
            {
                verticalSpeed = jumpSpeed;
                jumpTimeBegin += Time.deltaTime;
                isJumping = true;
            }
            else if (JumpReleasedThisFrame)
            {
                jumpEndedEarly = true;
                isJumping = false;
            }
        }
        else isJumping = false;

        jumpWasHeldLastFrame = jumpIsHeld;
    }

    #endregion

    #region Custom Gravity

    [SerializeField] private float maxFallSpeed = 10;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float gravityModEarlyEnd = 2;

    private void CalculateGravity()
    {
        if (onGround || isJumping) return;

        verticalSpeed -= jumpEndedEarly ? gravity * gravityModEarlyEnd * Time.deltaTime : gravity * Time.deltaTime;

        verticalSpeed = Mathf.Max(verticalSpeed, -maxFallSpeed);
    }

    #endregion

    void Update()
    {
        CastCollisions();
        HorizontalMovement();
        TryToJump();
        CalculateGravity();
    }
}
