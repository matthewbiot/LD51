using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string k_LevelSceneName = "Scene0{0}";

    private static GameManager s_Instance;

    public static GameManager instance => s_Instance;


    [SerializeField] Crossfade m_Crossfade;
    [SerializeField] float m_FadeTime;


    private bool m_Playing;
    private float m_TimeInGame;
    private int m_Deaths;
    private Room m_CurrentRoom;
    private Enemy m_CurrentEnemy;

    private string m_SceneToUnload;
    private string m_CurrentLoadingScene;

    public Enemy Enemy => m_CurrentEnemy;
    public int Deaths => m_Deaths;
    public float TimeInGame => m_TimeInGame;

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

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (!m_Playing)
            return;

        m_TimeInGame += Time.deltaTime;
    }

    public void StartGame()
    {
        m_Deaths = 0;
        m_TimeInGame = 0f;
        m_Playing = true;
        LoadLevel("Level01");
    }

    public void EndGame()
    {
        m_Playing = false;
    }

    public void Pause()
    {
        m_Playing = false;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        m_Playing = true;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadLevel(string levelName)
    {
        FadeOut(() =>
        {
            m_SceneToUnload = SceneManager.GetActiveScene().name;
            m_CurrentLoadingScene = levelName;
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        });
    }

    public void SetRoom(Room room)
    {
        m_CurrentRoom = room;
    }

    public void SetEnemy(Enemy enemy)
    {
        m_CurrentEnemy = enemy;
    }

    public void KillPlayer()
    {
        FadeOut(() =>
        {
            m_Deaths++;
            m_CurrentRoom.Restart(m_CurrentEnemy);
            FadeIn();
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (m_CurrentLoadingScene != scene.name)
            return;

        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync(m_SceneToUnload);
        Invoke("ShowGame", 0.5f);

    }

    private void ShowGame()
    {
        FadeIn();
    }


    private void FadeOut(Action onComplete = null)
    {
        m_Crossfade.FadeOut(m_FadeTime, onComplete);
    }

    private void FadeIn(Action onComplete = null)
    {
        m_Crossfade.FadeIn(m_FadeTime, onComplete);
    }
}
