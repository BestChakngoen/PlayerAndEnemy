using UnityEngine;
using GameSystem;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    public class PauseMenuHandler : MonoBehaviour
    {
        [Header("Settings")]
        public string mainMenuSceneName = "MainMenuScene";

        public void ResumeGame()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.ResumeGame();
            }
        }

        public void RestartGame()
        {
            Object.FindFirstObjectByType<CursorController>()?.ForceUnlockCursor();
            
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }

            if (SceneManger.Instance != null)
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManger.Instance.LoadWithLoadingScreen(currentSceneName);
            }
        }

        public void GoToMainMenu()
        {
            Object.FindFirstObjectByType<CursorController>()?.ForceUnlockCursor();
            
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }

            if (GameManager.Instance != null)
            {
                Destroy(GameManager.Instance.gameObject);
            }

            if (SceneManger.Instance != null)
            {
                SceneManger.Instance.LoadWithLoadingScreen(mainMenuSceneName);
            }
        }
    }
}