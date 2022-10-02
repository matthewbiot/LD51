using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] Rigidbody2D m_RigidBody;
    [SerializeField] SpriteAnimator m_Animator;

    [Header("Animation")]
    [SerializeField] Sheet m_IdleSheet;
    [SerializeField] Sheet m_RunSheet;
    [SerializeField] Sheet m_JumpSheet;
    [SerializeField] Sheet m_FallSheet;
    [SerializeField] Sheet m_WallSlideSheet;
    [SerializeField] Sheet m_DashSheet;

    [Header("Walk")]
    [SerializeField] private float m_Acceleration = 2f;
    [SerializeField] private float m_WalkSpeed = 2f;
    [SerializeField] private float m_CoyoteTime = 0.2f;
    [SerializeField] private float m_CurrentMovementLerpSpeed = 100;
    [SerializeField] private float m_SlideSpeed = 1;
    [SerializeField] private float m_ExitSlideTime = 0.5f;

    [Header("Jump")]
    [SerializeField] private float m_JumpForce = 15;
    [SerializeField] private float m_JumpVelocityFalloff = 8f;
    [SerializeField] private float m_FallMultiplier = 7f;
    [SerializeField] private bool m_EnableDoubleJump;

    [Header("Wall Jump")]
    [SerializeField] private float m_WallJumpMovementLerp = 5;

    [Header("Dash")]
    [SerializeField] private float m_DashSpeed = 15;
    [SerializeField] private float m_DashLength = 1;


    [Header("Detection")]
    [SerializeField] private LayerMask m_GroundMask;
    [SerializeField] private float m_GrounderOffset = -1, m_GrounderRadius = 0.2f;
    [SerializeField] private float m_WallCheckOffset = -1, m_WallCheckRadius = 0.2f;

    private PlayerInputActions m_Inputs;
    private InputAction m_Move;

    // Ground
    private readonly Collider2D[] m_Ground = new Collider2D[1];
    private readonly Collider2D[] m_LeftWall = new Collider2D[1];
    private readonly Collider2D[] m_RightWall = new Collider2D[1];
    private bool m_Grounded;
    private bool m_IsFacingLeft;
    private float m_TimeLeftGrounded = -10f;

    private bool m_IsAgainstLeftWall, m_IsAgainstRightWall;
    private bool m_PushingLeftWall, m_PushingRightWall;
    private bool againstWall => m_IsAgainstLeftWall || m_IsAgainstRightWall;
    private bool pushingAgainstWall => m_PushingLeftWall || m_PushingRightWall;
    private bool m_WallSliding;
    private float m_TimeStartedSliding;


    // Jump
    private bool m_HasJumped;
    private bool m_HasDoubleJumped;


    // Dash
    private bool m_Dashing;
    private bool m_HasDashed;
    private Vector3 m_DashDirection;
    private float m_TimeStartedDash;

    // Inputs
    private Vector2 m_Movement;

    private bool m_JumpPressedLastFrame;
    private bool m_JumpPressed;
    private bool m_DashPressed;

    void Awake()
    {
        m_Inputs = new PlayerInputActions();
        EnableInputs();
    }

    void Start()
    {
        m_Animator.Play(m_IdleSheet, true);
    }

    void Update()
    {
        GatherInputs();
    }

    void FixedUpdate()
    {
        HandleGround();
        HandleWalk();
        HandleJump();
        HandleWallSlide();
        HandleDash();
        HandleAnimation();
    }

    void OnDestroy()
    {
        DisableInputs();
    }

    public void EnableInputs()
    {
        m_Inputs.Game.Enable();

        m_Move = m_Inputs.Game.Move;

        Inputs.Add(m_Inputs.Game.Jump, OnJump);
        Inputs.Add(m_Inputs.Game.Dash, OnDash);
    }

    public void DisableInputs()
    {
        if (m_Inputs == null)
            return;

        Inputs.Remove(m_Inputs.Game.Jump, OnJump);
        Inputs.Remove(m_Inputs.Game.Dash, OnDash);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        m_JumpPressed = !ctx.canceled;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        m_DashPressed = !ctx.canceled;
    }

    private void GatherInputs()
    {
        m_Movement = m_Move.ReadValue<Vector2>();
        m_IsFacingLeft = m_Movement.normalized.x == -1;
    }

    private void HandleGround()
    {
        var grounded = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, m_GrounderOffset), m_GrounderRadius, m_Ground, m_GroundMask) > 0;

        // we were in the air but just landed
        if (!m_Grounded && grounded)
        {
            m_Grounded = true;
            m_HasDashed = false;
            m_HasJumped = false;
            m_HasDoubleJumped = false;
            m_CurrentMovementLerpSpeed = 100;

            // m_JumpPressed = false;
        }
        // we were grounded but we're not
        else if (m_Grounded && !grounded)
        {
            m_Grounded = false;
            m_TimeLeftGrounded = Time.time;
        }

        // Wall detection
        m_IsAgainstLeftWall = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(-m_WallCheckOffset, 0), m_WallCheckRadius, m_LeftWall, m_GroundMask) > 0;
        m_IsAgainstRightWall = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(m_WallCheckOffset, 0), m_WallCheckRadius, m_RightWall, m_GroundMask) > 0;
        m_PushingLeftWall = m_IsAgainstLeftWall && m_Movement.x < 0;
        m_PushingRightWall = m_IsAgainstRightWall && m_Movement.x > 0;

        // If we are wall sliding
        if (m_WallSliding || (!m_Grounded && againstWall))
        {
            if (!againstWall)
                m_WallSliding = false;
            else
            {
                if (Time.time >= m_TimeStartedSliding + m_ExitSlideTime &&
                ((m_IsAgainstLeftWall && m_Movement.x > 0) ||
                // Or we are on the right wall but pushing left
                (m_IsAgainstRightWall && m_Movement.x < 0)))
                    m_WallSliding = false;
            }
        }
    }

    private void HandleWalk()
    {
        m_CurrentMovementLerpSpeed = Mathf.MoveTowards(m_CurrentMovementLerpSpeed, 100, m_WallJumpMovementLerp * Time.fixedDeltaTime);

        if (m_Dashing)
            return;

        var acceleration = m_Grounded ? m_Acceleration : m_Acceleration * 0.5f;
        var normalized = m_Movement.normalized;

        if (normalized.x == -1)
        {
            if (m_RigidBody.velocity.x > 0) m_Movement.x = 0;
            m_Movement.x = Mathf.MoveTowards(m_Movement.x, -1, acceleration * Time.fixedDeltaTime);
        }
        else if (normalized.x == 1)
        {
            if (m_RigidBody.velocity.x < 0) m_Movement.x = 0;
            m_Movement.x = Mathf.MoveTowards(m_Movement.x, 1, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            m_Movement.x = Mathf.MoveTowards(m_Movement.x, 0, acceleration * Time.fixedDeltaTime);
        }

        var velocity = new Vector3(m_Movement.x * m_WalkSpeed, m_RigidBody.velocity.y);
        m_RigidBody.velocity = Vector3.MoveTowards(m_RigidBody.velocity, velocity, m_CurrentMovementLerpSpeed * Time.fixedDeltaTime);
    }

    private void HandleJump()
    {
        if (m_Dashing) return;
        if (!m_JumpPressedLastFrame && m_JumpPressed)
        {
            //Debug.Log("Jump");
            if (m_WallSliding)
            {
                // m_CurrentMovementLerpSpeed = m_WallJumpMovementLerp;
                Jump(new Vector2(m_IsAgainstLeftWall ? m_JumpForce : -m_JumpForce, m_JumpForce));
                //Debug.Log("against the wall");
            }
            else if (m_Grounded || (!m_HasJumped && Time.time < m_TimeLeftGrounded + m_CoyoteTime) || (m_EnableDoubleJump && !m_HasDoubleJumped))
            {
                if (!m_HasJumped || m_HasJumped && !m_HasDoubleJumped) Jump(new Vector2(m_RigidBody.velocity.x, m_JumpForce), m_HasJumped);
            }
        }

        m_JumpPressedLastFrame = m_JumpPressed;

        if ((m_RigidBody.velocity.y > 0 && !m_JumpPressed) || (m_RigidBody.velocity.y < m_JumpVelocityFalloff))
            m_RigidBody.velocity += m_FallMultiplier * Physics.gravity.y * Vector2.up * Time.fixedDeltaTime;
    }

    private void HandleWallSlide()
    {
        if (!m_Grounded && pushingAgainstWall && !m_WallSliding)
        {
            m_WallSliding = true;
            m_TimeStartedSliding = Time.time;
        }

        if (m_WallSliding)
        {
            if (m_RigidBody.velocity.y < 0)
                m_RigidBody.velocity = new Vector3(0, -m_SlideSpeed);
        }
    }

    private void HandleDash()
    {
        if (m_DashPressed && !m_HasDashed)
        {
            m_DashDirection = new Vector3(m_Movement.normalized.x, 0);
            if (m_DashDirection == Vector3.zero)
                m_DashDirection = m_IsFacingLeft ? Vector3.left : Vector3.right;

            m_Dashing = true;
            m_HasDashed = true;
            m_TimeStartedDash = Time.time;
            m_RigidBody.gravityScale = 0;
        }

        if (m_Dashing)
        {
            m_RigidBody.velocity = m_DashDirection * m_DashSpeed;

            if (Time.time >= m_TimeStartedDash + m_DashLength)
            {
                m_Dashing = false;
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x > 15 ? 15 : m_RigidBody.velocity.x, m_RigidBody.velocity.y);

                if (m_Grounded)
                    m_HasDashed = false;

                m_RigidBody.gravityScale = 1;
            }
        }
    }

    private enum State
    {
        Idle,
        Run,
        Jump,
        Fall,
        Dash,
        WallSlide
    }

    private State m_State;

    private void HandleAnimation()
    {
        var flipX = m_RigidBody.velocity.x < 0;

        if (m_Dashing)
            m_Animator.Play(m_DashSheet, false, sheet => sheet != m_DashSheet);
        else if (m_WallSliding)
        {
            flipX = m_IsAgainstRightWall;
            m_Animator.Play(m_WallSlideSheet, true, sheet => sheet != m_WallSlideSheet);
        }
        else if (!m_Grounded && m_RigidBody.velocity.y <= 0)
            m_Animator.Play(m_FallSheet, true, sheet => sheet != m_FallSheet);
        else if (!m_Grounded && m_RigidBody.velocity.y > 0)
            m_Animator.Play(m_JumpSheet, false, sheet => sheet != m_JumpSheet);
        else if (m_Grounded && m_RigidBody.velocity.x != 0)
            m_Animator.Play(m_RunSheet, false, sheet => sheet != m_RunSheet);
        else
            m_Animator.Play(m_IdleSheet, false, sheet => sheet != m_IdleSheet);

        m_SpriteRenderer.flipX = flipX;
    }

    private void Jump(Vector2 direction, bool doubleJump = false)
    {
        Debug.Log("Jump");
        m_RigidBody.velocity = direction;
        m_HasDoubleJumped = doubleJump;
        m_HasJumped = true;
    }

    private void DrawGrounderGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, m_GrounderOffset), m_GrounderRadius);
    }

    private void DrawWallSlideGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(-m_WallCheckOffset, 0), m_WallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(m_WallCheckOffset, 0), m_WallCheckRadius);
    }

    private void OnDrawGizmos()
    {
        DrawGrounderGizmos();
        DrawWallSlideGizmos();
    }

}
