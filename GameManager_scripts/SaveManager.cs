using UnityEngine;

namespace GameManger
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public SaveData CurrentSaveData { get; private set; }
        
        private const string SaveKey = "GameSaveData";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadGame();
        }

        public void SaveGame()
        {
            string json = JsonUtility.ToJson(CurrentSaveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public void LoadGame()
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                string json = PlayerPrefs.GetString(SaveKey);
                CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                CurrentSaveData = new SaveData();
            }
        }
    }
}