using UnityEngine;

namespace GameManger
{
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver
    }

    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        [Header("Scriptable Event Publisher")] [Tooltip("Asset Event ที่ใช้แจ้งเตือนสถานะเกมใหม่ (GameState)")]
        public GameStateEventSO GameStateChangedChannel;

        [Header("Dependencies")] [Tooltip("UI สำหรับ Pause Menu (จะถูกเปิดเมื่อ Paused)")]
        public GameObject pauseMenuUI;

        [Tooltip("UI สำหรับ Game Over Screen (จะถูกเปิดเมื่อ GameOver)")]
        public GameObject gameOverUI;

        public GameState CurrentState { get; private set; } = GameState.Gameplay;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            if (gameOverUI != null) gameOverUI.SetActive(false);

            Time.timeScale = 1f;
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;

            switch (newState)
            {
                case GameState.Paused:
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    break;
            }

            HandleUIForState(newState);

            if (GameStateChangedChannel != null)
            {
                GameStateChangedChannel.RaiseEvent(newState);
            }
        }

        private void HandleUIForState(GameState state)
        {
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            if (gameOverUI != null) gameOverUI.SetActive(false);

            switch (state)
            {
                case GameState.Paused:
                    if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
                    break;
                case GameState.GameOver:
                    if (gameOverUI != null) gameOverUI.SetActive(true);
                    break;
            }
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Gameplay)
            {
                SetState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Gameplay);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

}