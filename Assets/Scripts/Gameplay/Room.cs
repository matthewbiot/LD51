using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject m_LeftWall;
    [SerializeField] GameObject m_StartPoint;
    [SerializeField] GameObject m_VirtualCam;

    private PlayerController m_Player;

    void Start()
    {
        foreach (var trap in GetComponentsInChildren<Trap>())
            trap.RegisterRoom(this);
    }

    public void Restart()
    {
        m_Player.transform.position = m_StartPoint.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.isTrigger)
            return;

        m_Player = other.GetComponent<PlayerController>();
        m_VirtualCam.SetActive(true);

        if (m_LeftWall != null)
            m_LeftWall.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.isTrigger)
            return;

        m_Player = null;
        m_VirtualCam.SetActive(false);
    }
}
