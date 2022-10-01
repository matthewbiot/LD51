using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string k_LevelSceneName = "Scene0{0}";

    private static GameManager s_Instance;

    public static GameManager instance => s_Instance;


    private int m_Deaths;

    public void Awake()
    {
        if (s_Instance != null && s_Instance != this)
            Destroy(this);
        else
        {
            s_Instance = this;
            DontDestroyOnLoad(this);
        }
    }


    public void StartGame()
    {

    }

    public void IncrementDeathCounter()
    {
        m_Deaths++;
    }
}
