using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float k_DashTime = 10;

    [Header("References")]
    [SerializeField] Rigidbody2D m_RigidBody;

    [Header("Chase")]
    [SerializeField] float m_MaxDistance;
    [SerializeField] float m_ChaseSpeed;

    [Header("Dash")]
    [SerializeField] float m_DashReactionStartTime;
    [SerializeField] float m_DashReactionEndTime;
    [SerializeField] float m_DashSpeed;
    [SerializeField] float m_DashLength;

    private GameObject m_Target;

    private Vector2 m_Direction;
    private float m_Speed;

    private bool m_Stopping;
    private float m_StoppingSpeed;
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

        var distance = m_Target.transform.position - transform.position;

        if (m_TimeSinceLastDash >= k_DashTime)
        {
            m_DashDirection = distance.normalized;
            m_Dashing = true;
            m_TimeStartedDash = Time.time;
            return;
        }
        m_TimeSinceLastDash += Time.deltaTime;

        var reactionTime = k_DashTime - m_TimeSinceLastDash;
        if (reactionTime <= m_DashReactionStartTime)
        {
            if (!m_Stopping)
                m_StoppingSpeed = m_Speed;
            m_Stopping = true;

            m_Speed = Mathf.Lerp(m_StoppingSpeed, 0f, (stoppingTime - reactionTime) / stoppingTime);
            return;
        }

        var longestDistance = Mathf.Max(Mathf.Abs(distance.x), Mathf.Abs(distance.y));

        if (longestDistance > m_MaxDistance)
            m_Speed = m_ChaseSpeed;
        else
        {
            var ratio = longestDistance / (m_MaxDistance);
            m_Speed = Mathf.Lerp(m_ChaseSpeed, 0f, 1 - ratio);
        }

        m_Direction = distance.normalized;
    }

    void FixedUpdate()
    {
        if (m_Target == null)
            return;

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

#if DEBUG
    void OnDrawGizmos()
    {
        var style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(transform.position, ((int)(k_DashTime - m_TimeSinceLastDash)).ToString(), style);
    }
#endif
}
