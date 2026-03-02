using UnityEngine;

namespace GameSystem
{
    public class UIManager : MonoBehaviour
    {
        [Header("Event Listening")]
        public GameStateEventSO GameStateChangedChannel;

        [Header("UI Panels")]
        public GameObject pauseMenuUI;
        public GameObject gameOverUI;

        private void OnEnable()
        {
            if (GameStateChangedChannel != null)
            {
                GameStateChangedChannel.OnEventRaised.AddListener(HandleUIChange);
            }
        }

        private void OnDisable()
        {
            if (GameStateChangedChannel != null)
            {
                GameStateChangedChannel.OnEventRaised.RemoveListener(HandleUIChange);
            }
        }

        private void HandleUIChange(GameState newState)
        {
            if (pauseMenuUI != null) pauseMenuUI.SetActive(newState == GameState.Paused);
            if (gameOverUI != null) gameOverUI.SetActive(newState == GameState.GameOver);
        }
    }
}