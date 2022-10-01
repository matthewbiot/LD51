using System.Collections;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject m_LeftWall;
    [SerializeField] StartPoint m_StartPoint;
    [SerializeField] StartPoint m_EnemyStartPoint;
    [SerializeField] GameObject m_VirtualCam;

    private PlayerController m_Player;

    public void Restart(Enemy enemy)
    {
        m_Player.transform.position = m_StartPoint.transform.position;

        SetEnemyPosition(enemy);
    }

    public void SetEnemyPosition(Enemy enemy)
    {
        if (enemy == null || m_EnemyStartPoint == null)
            return;
        enemy.transform.position = m_EnemyStartPoint.transform.position;
        enemy.Reset();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.isTrigger)
            return;

        GameManager.instance?.SetRoom(this);

        m_Player = other.GetComponent<PlayerController>();
        m_VirtualCam.SetActive(true);

        if (m_LeftWall != null)
            StartCoroutine(ActivateLeftWall());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.isTrigger)
            return;
        m_Player = null;
        m_VirtualCam.SetActive(false);
    }

    private IEnumerator ActivateLeftWall()
    {
        yield return new WaitForSeconds(0.5f);
        m_LeftWall.SetActive(true);
    }
}
