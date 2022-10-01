using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLevelLoader : MonoBehaviour
{
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
            SceneManager.LoadSceneAsync("Initialization", LoadSceneMode.Additive);
    }
#endif
}
