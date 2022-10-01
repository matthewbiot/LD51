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

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public void StartGame()
    {

    }

    private string m_SceneToUnload;
    private string m_CurrentLoadingScene;

    public void LoadLevel(string levelName)
    {
        m_Crossfade.FadeOut(m_FadeTime, () =>
        {
            m_SceneToUnload = SceneManager.GetActiveScene().name;
            m_CurrentLoadingScene = levelName;
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        });
    }

    public void IncrementDeathCounter()
    {
        m_Deaths++;
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
        m_Crossfade.FadeIn(m_FadeTime, () =>
        {

        });
    }
}
