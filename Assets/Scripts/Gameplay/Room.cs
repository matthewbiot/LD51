using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject m_LeftWall;
    [SerializeField] GameObject m_VirtualCam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
            m_VirtualCam.SetActive(true);
        if (m_LeftWall != null)
            m_LeftWall.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
            m_VirtualCam.SetActive(false);
    }
}
