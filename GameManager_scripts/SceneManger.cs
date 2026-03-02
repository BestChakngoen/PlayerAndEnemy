using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    public class SceneManger : MonoBehaviour
    {
        public static SceneManger Instance { get; private set; }

        protected static string activeAdditiveSceneName;
        public static string targetSceneName;
        private bool wasPausedByOptions = false;
        
        [Header("Loading Settings")]
        public string defaultLoadingSceneName = "LoadingScene";

        [Header("Player Reset Settings")]
        public string[] resetPlayerScenes = { "MainMenuScene", "Act 1 Map" };

        private AudioListener fallbackListener;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            EnsureAudioListener();
        }

        public void LoadScene(string sceneName)
        {
            PrepareForSceneLoad(sceneName);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        public void LoadWithLoadingScreen(string sceneName)
        {
            PrepareForSceneLoad(sceneName);
            targetSceneName = sceneName;
            SceneManager.LoadScene(defaultLoadingSceneName, LoadSceneMode.Single);
        }

        private void PrepareForSceneLoad(string targetScene)
        {
            UnloadActiveAdditiveScene();

            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }

            if (PlayerSpawnManager.Instance != null)
            {
                PlayerSpawnManager.Instance.HidePlayer();
            }

            foreach (string resetScene in resetPlayerScenes)
            {
                if (targetScene.Contains(resetScene))
                {
                    if (PlayerInputs.PlayerPersistent.Instance != null)
                    {
                        PlayerInputs.PlayerPersistent.Instance.DestroyPlayer();
                    }
                    break;
                }
            }
        }

        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        public void LoadSceneAdditive(string sceneName)
        {
            if (!string.IsNullOrEmpty(activeAdditiveSceneName))
            {
                if (IsSceneOpen(activeAdditiveSceneName))
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
                if (IsSceneOpen(activeAdditiveSceneName))
                {
                    SceneManager.UnloadSceneAsync(activeAdditiveSceneName);
                }
                activeAdditiveSceneName = null;
            }
        }

        public void LoadOptionsScene(string optionSceneName)
        {
            if (!IsSceneOpen(optionSceneName))
            {
                SceneManager.LoadScene(optionSceneName, LoadSceneMode.Additive);
            }
        }

        public void UnloadOptionsScene(string optionSceneName)
        {
            if (IsSceneOpen(optionSceneName))
            {
                SceneManager.UnloadSceneAsync(optionSceneName);
            }
        }

        private bool IsSceneOpen(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoadedEventHandler;
            SceneManager.sceneUnloaded += SceneUnloadedEventHandler;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoadedEventHandler;
            SceneManager.sceneUnloaded -= SceneUnloadedEventHandler;
        }

        private void SceneLoadedEventHandler(Scene scene, LoadSceneMode mode)
        {
            EnsureAudioListener();

            if (scene.name.Contains("Options") || scene.name == "SceneOptions")
            {
                if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState == GameState.Gameplay)
                {
                    GameStateManager.Instance.PauseGame();
                    wasPausedByOptions = true;
                }
            }
        }

        private void SceneUnloadedEventHandler(Scene scene)
        {
            if (scene.name.Contains("Options") || scene.name == "SceneOptions")
            {
                if (wasPausedByOptions && GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.ResumeGame();
                    wasPausedByOptions = false;
                }
            }
        }

        private void EnsureAudioListener()
        {
            AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            bool hasOtherListener = false;
            foreach (var l in listeners)
            {
                if (l != fallbackListener)
                {
                    hasOtherListener = true;
                    break;
                }
            }

            if (!hasOtherListener)
            {
                if (fallbackListener == null)
                {
                    fallbackListener = gameObject.AddComponent<AudioListener>();
                }
                fallbackListener.enabled = true;
            }
            else
            {
                if (fallbackListener != null)
                {
                    fallbackListener.enabled = false;
                }
            }
        }
    }
}