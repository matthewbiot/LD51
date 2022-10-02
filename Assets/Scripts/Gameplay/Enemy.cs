using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    private const float k_DashTime = 10;

    [Header("References")]
    [SerializeField] Rigidbody2D m_RigidBody;
    [SerializeField] SpriteAnimator m_Animator;
    [SerializeField] SpriteRenderer m_SpriteRenderer;

    [Header("Animation")]
    [SerializeField] Sheet m_FollowSheet;
    [SerializeField] Sheet m_DashUpSheet;
    [SerializeField] Sheet m_DashForwardSheet;
    [SerializeField] Sheet m_ChargingSheet;


    [Header("Chase")]
    [SerializeField] float m_MinDistance;
    [SerializeField] float m_MaxDistance;
    [SerializeField] float m_FollowSpeed;
    [SerializeField] float m_FollowSpeedLerp = 2;
    [SerializeField] float m_FollowDeceleration = 10;
    [SerializeField] AnimationCurve m_ChaseCurve;
    [SerializeField] float m_ChaseSpeed;
    [SerializeField] float m_ChaseSpeedLerp = 10;

    [Header("Dash")]
    [SerializeField] float m_DashReactionStartTime;
    [SerializeField] float m_DashReactionEndTime;
    [SerializeField] float m_ChargingDistance;
    [SerializeField] AnimationCurve m_ChargingCurve;
    [SerializeField] float m_DashSpeed;
    [SerializeField] float m_DashLength;
    [SerializeField] AudioClip m_DashSound;

    private GameObject m_Target;

    private float m_DistanceLastFrame;
    private Vector2 m_Direction;
    private float m_Speed;

    private bool m_Stopping;
    private float m_StoppingSpeed;
    private bool m_Charging;
    private Vector2 m_ChargingDirection;
    private bool m_Dashing;
    private Vector2 m_DashDirection;
    private float m_TimeSinceLastDash;
    private float m_TimeStartedDash;

    private float stoppingTime => m_DashReactionStartTime - m_DashReactionEndTime;

    void Start()
    {
        GameManager.instance?.SetEnemy(this);
        var player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Failed to find player");
            return;
        }

        m_Target = player.gameObject;
    }

    public void Reset()
    {
        m_Dashing = false;
        m_TimeSinceLastDash = 0f;
    }

    void Update()
    {
        if (m_Target == null)
            return;

        if (m_Dashing)
            return;

        var distance = GetDistance();
        var direction = (m_Target.transform.position - transform.position).normalized;

        if (m_TimeSinceLastDash >= k_DashTime)
        {
            m_DashDirection = direction;
            m_Dashing = true;
            m_Charging = false;
            m_TimeStartedDash = Time.time;
            AudioManager.instance.PlaySFX(m_DashSound);
            return;
        }
        m_TimeSinceLastDash += Time.deltaTime;

        var reactionTime = k_DashTime - m_TimeSinceLastDash;
        if (reactionTime <= m_DashReactionEndTime)
        {
            m_Stopping = false;
            m_Charging = true;
            var chargingDirection = direction * -1;

            var t = (m_DashReactionEndTime - reactionTime) / m_DashReactionEndTime;
            var p = m_ChargingCurve.Evaluate(t);
            m_ChargingDirection = new Vector2(m_ChargingDistance, m_ChargingDistance) * p;
        }
        else if (reactionTime <= m_DashReactionStartTime)
        {
            if (!m_Stopping)
                m_StoppingSpeed = m_Speed;
            m_Stopping = true;

            m_Speed = Mathf.Lerp(m_StoppingSpeed, 0f, (stoppingTime - reactionTime) / stoppingTime);
            return;
        }

        var speed = m_Speed;
        var lerp = (distance - m_DistanceLastFrame) > 0 ? m_FollowSpeedLerp : m_FollowDeceleration;

        if (distance > m_MaxDistance)
        {
            speed = m_ChaseSpeed;
            lerp = m_ChaseSpeedLerp;
        }
        else if (distance < m_MinDistance)
        {
            speed = m_FollowSpeed;
        }
        else
        {
            var t = ((distance - m_MinDistance) * 100) / (m_MaxDistance - m_MinDistance);
            speed = m_FollowSpeed + (m_ChaseCurve.Evaluate(t) * (m_ChaseSpeed - m_FollowSpeed));
        }

        m_DistanceLastFrame = distance;
        m_Speed = Mathf.MoveTowards(m_Speed, speed, lerp * Time.deltaTime);
        m_Direction = direction;

        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (m_Target == null)
            return;

        if (m_Charging)
        {
            m_RigidBody.velocity = m_ChargingDirection;
            return;
        }

        if (m_Dashing)
        {
            m_RigidBody.velocity = m_DashDirection * m_DashSpeed;

            if (Time.time >= m_TimeStartedDash + m_DashLength)
            {
                m_Dashing = false;
                m_TimeSinceLastDash = 0f;
            }
            return;
        }

        m_RigidBody.velocity = m_Direction * m_Speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.isTrigger)
            return;

        GameManager.instance.KillPlayer();
    }

    private float GetDistance()
    {
        // distance between 2 points sqrt( (x2 - x1)^2 + (y2 - y1)^2 )
        return Mathf.Sqrt(
            Mathf.Pow(m_Target.transform.position.x - transform.position.x, 2) +
            Mathf.Pow(m_Target.transform.position.y - transform.position.y, 2)
        );
    }

    private void HandleAnimation()
    {
        bool flipX = m_Direction.x < 0;
        bool flipY = false;
        if (m_Dashing)
        {
            var vertical = Mathf.Abs(m_Direction.y) > Mathf.Abs(m_Direction.x);
            if (vertical && m_Direction.y < 0)
                flipY = true;
            var newSheet = vertical ? m_DashUpSheet : m_DashForwardSheet;
            flipX = m_DashDirection.x < 0;
            m_Animator.Play(newSheet, true, sheet => sheet != newSheet);
        }
        else if (m_Stopping || m_Charging)
            m_Animator.Play(m_ChargingSheet, true, sheet => sheet != m_ChargingSheet);
        else
            m_Animator.Play(m_FollowSheet, true, sheet => sheet != m_FollowSheet);

        m_SpriteRenderer.flipX = flipX;
        m_SpriteRenderer.flipY = flipY;
    }

}
