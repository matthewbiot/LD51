using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [SerializeField] bool m_Enemy;

    void OnDrawGizmos()
    {
        Gizmos.color = m_Enemy ? Color.red : Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
