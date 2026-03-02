using UnityEngine;

namespace GameSystem
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

        [Header("Scriptable Event Publisher")]
        public GameStateEventSO GameStateChangedChannel;

        public GameState CurrentState { get; private set; } = GameState.Gameplay;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializeGame();
        }

        private void InitializeGame()
        {
            Time.timeScale = 1f;
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;

            Time.timeScale = (newState == GameState.Paused || newState == GameState.GameOver) ? 0f : 1f;

            if (GameStateChangedChannel != null)
            {
                GameStateChangedChannel.RaiseEvent(newState);
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