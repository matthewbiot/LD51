using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] GameObject m_EnemyPrefab;
    [SerializeField] bool m_LoadEnemy = true;
    [SerializeField] Room m_StartingRoom;
    [SerializeField] string m_NextLevel;

    void Start()
    {
        if (m_EnemyPrefab == null || m_StartingRoom == null || !m_LoadEnemy)
            return;

        var gameObject = Instantiate(m_EnemyPrefab);
        var enemy = gameObject.GetComponent<Enemy>();
        m_StartingRoom.SetEnemyPosition(enemy);

        GameManager.instance?.SetEnemy(enemy);
    }

    public void LoadNextLevel()
    {
        GameManager.instance.LoadLevel(m_NextLevel);
    }
}
