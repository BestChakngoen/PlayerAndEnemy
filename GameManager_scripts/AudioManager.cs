using UnityEngine;
using UnityEngine.Audio;

namespace GameManger
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioMixer mainMixer;
        public AudioSource bgmSource;
        public GameObject sfxPrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ApplySavedSettings();
        }

        public void ApplySavedSettings()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                SetMasterVolume(SaveManager.Instance.CurrentSaveData.masterVolume);
                SetBGMVolume(SaveManager.Instance.CurrentSaveData.bgmVolume);
                SetSFXVolume(SaveManager.Instance.CurrentSaveData.sfxVolume);
            }
        }

        public void SetMasterVolume(float volume)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CurrentSaveData.masterVolume = volume;
                SaveManager.Instance.SaveGame();
            }
            
            float db = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
            if (mainMixer != null)
            {
                mainMixer.SetFloat("MasterVolume", db);
            }
        }

        public void SetBGMVolume(float volume)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CurrentSaveData.bgmVolume = volume;
                SaveManager.Instance.SaveGame();
            }
            
            float db = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
            if (mainMixer != null)
            {
                mainMixer.SetFloat("BGMVolume", db);
            }
        }

        public void SetSFXVolume(float volume)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CurrentSaveData.sfxVolume = volume;
                SaveManager.Instance.SaveGame();
            }
            
            float db = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
            if (mainMixer != null)
            {
                mainMixer.SetFloat("SFXVolume", db);
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (bgmSource == null || clip == null) return;
            
            if (bgmSource.clip == clip) return;

            bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            
            if (sfxPrefab != null)
            {
                GameObject sfxObj = Instantiate(sfxPrefab, position, Quaternion.identity);
                AudioSource source = sfxObj.GetComponent<AudioSource>();
                if (source != null)
                {
                    source.clip = clip;
                    source.volume = volume;
                    source.Play();
                    Destroy(sfxObj, clip.length); 
                }
                else
                {
                    Destroy(sfxObj);
                }
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, position, volume);
            }
        }
    }
}