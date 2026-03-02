using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    public class PauseInputHandler : MonoBehaviour
    {
        [Tooltip("Input Action ที่ใช้ในการ Toggle Pause (เช่น ESC)")]
        public InputActionReference pauseAction; 

        [Header("Restrictions")]
        public string[] disabledScenes = { "MainMenuScene", "LoadingScene", "SceneOptions", "Cutscene" };

        private void OnEnable()
        {
            if (pauseAction != null && pauseAction.action != null)
            {
                pauseAction.action.Enable();
                pauseAction.action.performed += OnPausePerformed;
            }
        }

        private void OnDisable()
        {
            if (pauseAction != null && pauseAction.action != null)
            {
                pauseAction.action.performed -= OnPausePerformed;
                pauseAction.action.Disable();
            }
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (GameStateManager.Instance == null) return;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                string loadedSceneName = SceneManager.GetSceneAt(i).name;
                
                foreach (string disabledScene in disabledScenes)
                {
                    if (loadedSceneName.Contains(disabledScene))
                    {
                        return;
                    }
                }
            }
            
            if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
            {
                GameStateManager.Instance.PauseGame();
            }
            else if (GameStateManager.Instance.CurrentState == GameState.Paused)
            {
                GameStateManager.Instance.ResumeGame();
            }
        }
    }
}