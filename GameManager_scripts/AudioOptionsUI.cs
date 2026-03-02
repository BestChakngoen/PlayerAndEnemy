using UnityEngine;
using UnityEngine.UI;

namespace GameManger
{
    public class AudioOptionsUI : MonoBehaviour
    {
        public Slider masterSlider;
        public Slider bgmSlider;
        public Slider sfxSlider;

        private void Start()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                if (masterSlider != null) masterSlider.value = SaveManager.Instance.CurrentSaveData.masterVolume;
                if (bgmSlider != null) bgmSlider.value = SaveManager.Instance.CurrentSaveData.bgmVolume;
                if (sfxSlider != null) sfxSlider.value = SaveManager.Instance.CurrentSaveData.sfxVolume;
            }

            if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        private void OnDestroy()
        {
            if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            if (bgmSlider != null) bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }

        private void OnMasterVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(value);
            }
        }

        private void OnBGMVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }
        }

        private void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
        }
    }
}