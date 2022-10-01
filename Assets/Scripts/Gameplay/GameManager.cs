using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string k_LevelSceneName = "Scene0{0}";

    private static GameManager s_Instance;

    public static GameManager instance => s_Instance;

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


    public void PreloadLevel(string level)
    {

    }

    public void LoadLevel(string level)
    {
        if (string.IsNullOrWhiteSpace(level))
            return;

        var sceneName = string.Format(k_LevelSceneName, level);

        if (SceneManager.GetActiveScene().name == sceneName)
            return;

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
}
