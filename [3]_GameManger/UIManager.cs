using UnityEngine;
using TMPro; 
using System.Collections;

namespace GameManger
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Element References")] [Tooltip("Text สำหรับแสดงจำนวน Kills")]
        public TextMeshProUGUI killCounterText;

        [Tooltip("Text สำหรับแสดงข้อความแจ้ง Wave")]
        public TextMeshProUGUI waveNotificationText;

        [Header("Wave Notification Settings")]
        [Tooltip("ตำแหน่ง Y (0.0 ถึง 1.0) บนหน้าจอ, X จะอยู่ตรงกลาง")]
        [Range(0.0f, 1.0f)]
        public float notificationYPosition = 0.8f;

        [Tooltip("ความเร็วในการ Fade-in/Fade-out")]
        public float fadeDuration = 0.5f;

        [Tooltip("ระยะเวลาที่ข้อความค้างอยู่บนจอ")]
        public float displayDuration = 2.0f;
        
        private int totalKills = 0;
        private Coroutine fadeRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            if (waveNotificationText != null)
            {
                RectTransform rt = waveNotificationText.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, notificationYPosition);
                rt.anchorMax = new Vector2(0.5f, notificationYPosition);
                rt.pivot = new Vector2(0.5f, 0.5f);
                
                waveNotificationText.color = new Color(waveNotificationText.color.r, waveNotificationText.color.g,
                    waveNotificationText.color.b, 0f);
            }
            
            UpdateKillCounter();
        }
        
        public IEnumerator DisplayWaveNotificationRoutine(int waveNumber) 
        {
            if (waveNotificationText == null) yield break; 
            
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }
    
            waveNotificationText.text = "WAVE " + waveNumber;
            
            yield return StartCoroutine(FadeAndDisplayRoutine());
            
        }

        IEnumerator FadeAndDisplayRoutine()
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                SetTextAlpha(waveNotificationText, alpha);
                timer += Time.deltaTime;
                yield return null;
            }

            SetTextAlpha(waveNotificationText, 1f); 
            
            yield return new WaitForSeconds(displayDuration);
            
            timer = 0f;
            while (timer < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                SetTextAlpha(waveNotificationText, alpha);
                timer += Time.deltaTime;
                yield return null;
            }

            SetTextAlpha(waveNotificationText, 0f); 
        }

        private void SetTextAlpha(TextMeshProUGUI text, float alpha)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
        
        public void AddKill()
        {
            totalKills++;
            UpdateKillCounter();
        }

        private void UpdateKillCounter()
        {
            if (killCounterText != null)
            {
                killCounterText.text = "Kills: " + totalKills;
            }
        }
    }
}