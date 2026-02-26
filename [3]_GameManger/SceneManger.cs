using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManger : MonoBehaviour
{
    protected static string activeAdditiveSceneName;
    protected static bool IsSceneOptionsLoaded;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public void LoadSceneAdditive(string sceneName)
    {
        if (!string.IsNullOrEmpty(activeAdditiveSceneName))
        {
            Scene sceneToUnload = SceneManager.GetSceneByName(activeAdditiveSceneName);
            if (sceneToUnload.isLoaded)
            {
                SceneManager.UnloadSceneAsync(activeAdditiveSceneName);
            }
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        activeAdditiveSceneName = sceneName;
    }

    public void UnloadActiveAdditiveScene()
    {
        if (!string.IsNullOrEmpty(activeAdditiveSceneName))
        {
            Scene sceneToUnload = SceneManager.GetSceneByName(activeAdditiveSceneName);
            if (sceneToUnload.isLoaded)
            {
                SceneManager.UnloadSceneAsync(activeAdditiveSceneName);
            }

            activeAdditiveSceneName = null;
        }
    }

    public void LoadOptionsScene(string optionSceneName)
    {
        if (!IsSceneOptionsLoaded)
        {
            SceneManager.LoadScene(optionSceneName, LoadSceneMode.Additive);
            IsSceneOptionsLoaded = true;
        }
    }

    public void UnloadOptionsScene(string optionSceneName)
    {
        if (IsSceneOptionsLoaded)
        {
            SceneManager.UnloadSceneAsync(optionSceneName);
            IsSceneOptionsLoaded = false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #region Scene Load and Unload Events Handler

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += SceneUnloadEventHandler;
        SceneManager.sceneLoaded += SceneLoadedEventHandler;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= SceneUnloadEventHandler;
        SceneManager.sceneLoaded -= SceneLoadedEventHandler;
    }

    private void SceneUnloadEventHandler(Scene scene)
    {


    }

    private void SceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.CompareTo("SceneOptions") != 0)
        {
            IsSceneOptionsLoaded = false;
        }
    }

    #endregion
}
