using UnityEngine;

namespace GameManger
{
    public class MainMenuBGM : MonoBehaviour
    {
        public AudioClip menuBGM;

        private void Start()
        {
            if (AudioManager.Instance != null && menuBGM != null)
            {
                AudioManager.Instance.PlayBGM(menuBGM);
            }
        }
    }
}