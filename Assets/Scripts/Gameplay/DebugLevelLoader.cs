using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLevelLoader : MonoBehaviour
{
    [SerializeField] Room m_StartingRoom;

#if DEBUG
    void Start()
    {
        var initializationLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == "Initialization")
            {
                initializationLoaded = true;
                break;
            }
        }

        if (!initializationLoaded)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync("Initialization", LoadSceneMode.Additive);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Initialization")
        {
            GameManager.instance.SetRoom(m_StartingRoom);
            GameManager.instance.SetEnemy(FindObjectOfType<Enemy>());
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this);
        }
    }
#endif
}
