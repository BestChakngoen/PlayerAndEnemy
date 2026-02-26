using UnityEngine;
using UnityEngine.InputSystem;

namespace GameManger
{
    public class PauseInputHandler : MonoBehaviour
    {
        [Tooltip("Input Action ที่ใช้ในการ Toggle Pause (เช่น ESC)")]
        public InputActionReference pauseAction; 

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