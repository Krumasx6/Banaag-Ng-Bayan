using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Camera Lock (Metal Slug style)")]
    [SerializeField] private CameraFollowScript cameraFollow; // optional — leave empty to disable backtracking lock

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float doubleJumpForce = 20f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Colliders")]
    [SerializeField] private GameObject standingColliderObj;
    [SerializeField] private GameObject crouchColliderObj;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;

    // State
    private Vector2 input;
    private bool facingRight = true;
    private bool isGrounded;
    private bool hasDoubleJumped;
    private bool isCrouching;
    private bool isDashing;
    private float dashTimer;
    private float dashDirection;
    private bool dashBuffered;
    private float dashCooldownTimer;

    private float defaultGravityScale;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        SetCrouchCollider(false);
    }

    private void Update()
    {
        CheckGround();
        ProcessInput();
        HandleCrouch();
        HandleJump();
        HandleDash();
        FlipCheck();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            return;
        }

        float speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(input.x * speed, rb.linearVelocity.y);

        // Block walking back past the camera's left edge (Metal Slug style lock)
        if (cameraFollow != null)
        {
            float leftBound = cameraFollow.GetLeftBoundaryX();
            if (rb.position.x < leftBound)
            {
                rb.position = new Vector2(leftBound, rb.position.y);
            }
        }
    }

    // ─── Movement (A/D) ──────────────────────────────────────────────────────

    private void ProcessInput()
    {
        if (isDashing) return;

        input = new Vector2(InputManager.Instance.Horizontal, 0f);
    }

    // ─── Crouch (S) ──────────────────────────────────────────────────────────

    private void HandleCrouch()
    {
        // Only crouch when grounded — Down in air is aim down, not crouch
        bool wantsCrouch = InputManager.Instance.Vertical < 0f && isGrounded;

        if (wantsCrouch && !isCrouching)
        {
            isCrouching = true;
            SetCrouchCollider(true);
        }
        else if (!wantsCrouch && isCrouching)
        {
            isCrouching = false;
            SetCrouchCollider(false);
        }
    }

    private void SetCrouchCollider(bool crouching)
    {
        // Lower body (crouch collider) stays active at all times.
        // Upper body (standing collider) turns off only while crouching.
        if (crouchColliderObj != null)   crouchColliderObj.SetActive(true);
        if (standingColliderObj != null) standingColliderObj.SetActive(!crouching);
    }

    // ─── Jump / Double Jump (K) — S never blocks jump ───────────────────────

    private void HandleJump()
    {
        if (!InputManager.Instance.JumpPressed) return;

        // Stand up first if crouching, then jump
        if (isCrouching)
        {
            isCrouching = false;
            SetCrouchCollider(false);
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            hasDoubleJumped = false;
        }
        else if (!hasDoubleJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            hasDoubleJumped = true;
        }
    }

    // ─── Dash (I) — Cuphead style, air allowed, no cooldown spam ────────────

    private void HandleDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        // Buffer the input — if pressed while dashing or on cooldown, store it
        if (InputManager.Instance.DashPressed)
            dashBuffered = true;

        // Clear buffer if key was released — no phantom dash
        if (InputManager.Instance.DashReleased)
            dashBuffered = false;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = defaultGravityScale;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                // Fire buffered dash after cooldown clears
                if (dashBuffered && dashCooldownTimer <= 0f)
                {
                    dashBuffered = false;
                    StartDash();
                }
            }
            return;
        }

        if (dashBuffered && dashCooldownTimer <= 0f)
        {
            dashBuffered = false;
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashDirection = facingRight ? 1f : -1f;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // kill fall momentum
    }

    // ─── Ground Check ────────────────────────────────────────────────────────

    private void CheckGround()
    {
        if (groundCheck == null) return;
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && !wasGrounded) hasDoubleJumped = false;
    }

    // ─── Flip ────────────────────────────────────────────────────────────────

    private void FlipCheck()
    {
        if (input.x > 0f && !facingRight) Flip();
        else if (input.x < 0f && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    // ─── Public Accessors (used by PlayerCombat) ────────────────────────────

    public bool IsGrounded() => isGrounded;
    public bool IsCrouching() => isCrouching;
    public bool IsDashing() => isDashing;
    public bool IsFacingRight() => facingRight;
}