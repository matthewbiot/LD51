using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Initialization")
            return;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
            return;

        SceneManager.SetActiveScene(scene);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
