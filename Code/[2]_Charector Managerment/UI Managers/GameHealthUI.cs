using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BasicEnemy
{
    public class GameHealthUI : MonoBehaviour
    {
        [Header("UI References")] 
        public Slider healthSlider;
        public Slider easeHealthSlider;
        public float lerpSpeed = 0.05f;

        [Header("Event Channel to Listen")] [Tooltip("ลาก HealthChangedEventSO ของ Player มาใส่")]
        public HealthChangedEventSO GameHealthIDChannel;
        private int myInstanceID;
        private Coroutine healthBarCoroutine;
        public HealthBarCleanupFinishedSO OnCleanupFinishedEvent; 

        private void Awake()
        {
            if (transform.parent != null && transform.parent.GetComponent<Health>() != null)
            {
                myInstanceID = transform.parent.gameObject.GetInstanceID();
            }
            else
            {
                Health healthPublisher = GetComponentInParent<Health>();
                if (healthPublisher != null)
                {
                    myInstanceID = healthPublisher.gameObject.GetInstanceID();
                }
                else
                {
                    myInstanceID = transform.parent != null ? transform.parent.gameObject.GetInstanceID() : gameObject.GetInstanceID();
                }
            }
        }

        void OnEnable()
        {
            if (GameHealthIDChannel != null)
            {
                GameHealthIDChannel.OnEventRaised += OnGlobalHealthChanged;
            }
        }

        void OnDisable()
        {
            if (GameHealthIDChannel != null)
            {
                GameHealthIDChannel.OnEventRaised -= OnGlobalHealthChanged;
            }
        }
        private void OnGlobalHealthChanged(int eventInstanceID, float currentHealth, float maxHealth)
        {
            if (eventInstanceID == myInstanceID)
            {
                UpdateHealthBarUI(currentHealth, maxHealth); 
            }
            else
            {
                Debug.Log($"Controller ID MISMATCH. My ID: {myInstanceID}, Received ID: {eventInstanceID}");
            }
        }
        public void UpdateHealthBarUI(float currentHealth, float maxHealth)
        {
            if (easeHealthSlider != null)
            {
                easeHealthSlider.value = currentHealth / maxHealth;
            }

            if (healthBarCoroutine != null)
            {
                StopCoroutine(healthBarCoroutine);
            }

            healthBarCoroutine = StartCoroutine(UpdateEaseSlider(currentHealth, maxHealth));
        }

        private IEnumerator UpdateEaseSlider(float currentHealth, float maxHealth)
        {
            if (healthSlider == null) yield break;
            
            float targetValue = currentHealth / maxHealth;

            while (Mathf.Abs(healthSlider.value - targetValue) > 0.01f)
            {
                healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, lerpSpeed);
                yield return null;
            }

            healthSlider.value = targetValue;
            if (targetValue <= 0) 
            {
                if (OnCleanupFinishedEvent != null)
                {
                    OnCleanupFinishedEvent.RaiseEvent(myInstanceID);
                }
            }
        }
    }
}