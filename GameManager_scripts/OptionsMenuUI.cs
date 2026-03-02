using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManger
{
    public class OptionsMenuUI : MonoBehaviour
    {
        public string optionsSceneName = "SceneOptions";

        public void OpenOptions()
        {
            if (!IsSceneLoaded(optionsSceneName))
            {
                SceneManager.LoadScene(optionsSceneName, LoadSceneMode.Additive);
            }
        }

        public void CloseOptions()
        {
            if (IsSceneLoaded(optionsSceneName))
            {
                SceneManager.UnloadSceneAsync(optionsSceneName);
            }
        }

        private bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}