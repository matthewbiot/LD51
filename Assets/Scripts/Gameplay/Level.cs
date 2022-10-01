using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] string m_NextLevel;

    public void LoadNextLevel()
    {
        GameManager.instance.LoadLevel(m_NextLevel);
    }
}
