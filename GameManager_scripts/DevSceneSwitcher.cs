using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSystem
{
    public class DevSceneSwitcher : MonoBehaviour
    {
        public string scene1 = "Act 1 Map";
        public string scene2 = "Act 2 Map";
        public string scene3 = "Act 3 Boss Map";

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                LoadScene(scene1);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                LoadScene(scene2);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                LoadScene(scene3);
            }
        }

        private void LoadScene(string sceneName)
        {
            if (SceneManger.Instance != null)
            {
                SceneManger.Instance.LoadWithLoadingScreen(sceneName);
            }
        }
    }
}