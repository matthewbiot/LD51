using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject m_PlayerPrefab;
    [SerializeField] GameObject m_StartPoint;

    void Awake()
    {

    }

    void Start()
    {
        Instantiate(m_PlayerPrefab, m_StartPoint.transform.position, Quaternion.identity);
    }
}
