using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] string m_PreviousLevel;
    [SerializeField] string m_NextLevel;

    [Header("Settings")]
    [SerializeField] GameObject m_StartPoint;
}
