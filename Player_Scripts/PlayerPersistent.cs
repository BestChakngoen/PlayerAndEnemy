using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace PlayerInputs
{
    public class PlayerPersistent : MonoBehaviour
    {
        public static PlayerPersistent Instance { get; private set; }

        [Header("Allowed Scenes")]
        public string[] allowedScenes = { "Act 1 Map", "Act 2 Map", "Act 3 Boss Map", "LoadingScene" };

        private void Awake()
        {
            // 🛑 [ระบบป้องกัน] ถ้าเป็นตัวละครออนไลน์ ให้ทำลายแค่สคริปต์นี้ทิ้ง ห้ามทำลาย GameObject เด็ดขาด!
            PhotonView pv = GetComponent<PhotonView>();
            if (pv != null && PhotonNetwork.IsConnected)
            {
                Destroy(this); 
                return;
            }

            // ระบบ Singleton สำหรับโหมดเล่นคนเดียว
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
            // ถ้าอยู่ในห้องออนไลน์ ปล่อยให้ OnlineGameManager จัดการแทน
            if (PhotonNetwork.InRoom)
            {
                return;
            }

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