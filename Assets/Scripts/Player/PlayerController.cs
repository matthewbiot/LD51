using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D m_RigidBody;

    [Header("Walk")]
    [SerializeField] private float m_Acceleration = 2f;
    [SerializeField] private float m_WalkSpeed = 2f;
    [SerializeField] private float m_CoyoteTime = 0.2f;
    [SerializeField] private float m_CurrentMovementLerpSpeed = 100;

    [Header("Jump")]
    [SerializeField] private float m_JumpForce = 15;
    [SerializeField] private float m_JumpVelocityFalloff = 8f;
    [SerializeField] private float m_FallMultiplier = 7f;
    [SerializeField] private bool m_EnableDoubleJump;


    [Header("Wall Jump")]
    [SerializeField] private float m_WallJumpMovementLerp = 5;

    [Header("Detection")]
    [SerializeField] private LayerMask m_GroundMask;
    [SerializeField] private float m_GrounderOffset = -1, m_GrounderRadius = 0.2f;


    private PlayerInputActions m_Inputs;
    private InputAction m_Move;

    // Ground
    private readonly Collider2D[] m_Ground = new Collider2D[1];
    [SerializeField] bool m_Grounded;
    private bool m_IsFacingLeft;
    private float m_TimeLeftGrounded = -10f;

    private bool m_Grabbing;
    private bool m_IsAgainstLeftWall, m_IsAgainstRightWall;
    private bool againstWall => m_IsAgainstLeftWall || m_IsAgainstRightWall;


    // Jump
    private bool m_HasJumped;
    private bool m_HasDoubleJumped;


    // Dash
    private bool m_Dashing;
    private bool m_HasDashed;

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

    void Update()
    {
        GatherInputs();
    }

    void FixedUpdate()
    {
        HandleGround();
        HandleWalk();
        HandleJump();
        HandleDash();
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
    }

    private void HandleWalk()
    {
        //m_CurrentMovementLerpSpeed = Mathf.MoveTowards(m_CurrentMovementLerpSpeed, 100, m_WallJumpMovementLerp * Time.fixedDeltaTime);

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
            if (m_Grabbing || !m_Grounded && againstWall)
            {
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

    private void HandleDash()
    {

    }

    private void Jump(Vector2 direction, bool doubleJump)
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

    // private void DrawWallSlideGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position + new Vector3(-m_WallCheckOffset, 0), m_WallCheckRadius);
    //     Gizmos.DrawWireSphere(transform.position + new Vector3(m_WallCheckOffset, 0), m_WallCheckRadius);
    // }

    private void OnDrawGizmos()
    {
        DrawGrounderGizmos();
        //DrawWallSlideGizmos();
    }

}
