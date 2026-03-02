using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    public class PlayerSpawnManager : MonoBehaviour
    {
        public static PlayerSpawnManager Instance { get; private set; }

        [Header("Gameplay State Settings")]
        public string[] gameplayScenes = { "Act 1 Map", "Act 2 Map", "Act 3 Boss Map" };

        [Header("Camera Settings")]
        public string forcedThirdPersonScene = "Act 3 Boss Map";

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
            if (GameStateManager.Instance != null)
            {
                foreach (string gameplayScene in gameplayScenes)
                {
                    if (scene.name.Contains(gameplayScene))
                    {
                        GameStateManager.Instance.SetState(GameState.Gameplay);
                        StartCoroutine(ResetPlayerPositionRoutine(scene));
                        break;
                    }
                }
            }
        }

        public void HidePlayer()
        {
            GameObject player = GetPlayer();
            if (player != null)
            {
                player.SetActive(false);
            }
        }

        private IEnumerator ResetPlayerPositionRoutine(Scene scene)
        {
            yield return new WaitForEndOfFrame();

            GameObject player = GetPlayer();

            if (player != null)
            {
                player.SetActive(true);

                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                PlayerSpawnPoint spawnPoint = Object.FindFirstObjectByType<PlayerSpawnPoint>(FindObjectsInactive.Include);
                
                if (spawnPoint != null)
                {
                    player.transform.position = spawnPoint.transform.position;
                    player.transform.rotation = spawnPoint.transform.rotation;
                }
                else
                {
                    player.transform.position = Vector3.zero;
                }

                Physics.SyncTransforms();

                if (cc != null) cc.enabled = true;

                if (scene.name.Contains(forcedThirdPersonScene))
                {
                    CameraViewSwitcher viewSwitcher = player.GetComponentInChildren<CameraViewSwitcher>();
                    if (viewSwitcher != null)
                    {
                        viewSwitcher.ForceThirdPersonView();
                    }
                }
            }
        }

        private GameObject GetPlayer()
        {
            if (PlayerInputs.PlayerPersistent.Instance != null)
            {
                return PlayerInputs.PlayerPersistent.Instance.gameObject;
            }
            return GameObject.FindGameObjectWithTag("Player");
        }
    }
}