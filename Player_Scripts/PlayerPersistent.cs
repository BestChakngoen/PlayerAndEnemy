using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerInputs
{
    public class PlayerPersistent : MonoBehaviour
    {
        public static PlayerPersistent Instance { get; private set; }

        [Header("Allowed Scenes")]
        public string[] allowedScenes = { "Act 1 Map", "Act 2 Map", "Act 3 Boss Map", "LoadingScene" };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool isAllowed = false;
            foreach (string allowedScene in allowedScenes)
            {
                if (scene.name.Contains(allowedScene))
                {
                    isAllowed = true;
                    break;
                }
            }

            if (!isAllowed)
            {
                DestroyPlayer();
            }
        }

        public void DestroyPlayer()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            Destroy(gameObject);
        }
    }
}