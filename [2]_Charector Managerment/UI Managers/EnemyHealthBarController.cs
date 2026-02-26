using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BasicEnemy
{
    public class EnemyHealthBarController : MonoBehaviour
    {
        [Header("UI References")] 
        public Slider healthSlider;
        public Slider easeHealthSlider;
        public float lerpSpeed = 0.05f;

        [Header("Event Channel to Listen")] [Tooltip("ลาก HealthChangedEventSO ของ Enemy มาใส่")]
        public HealthChangedEventSO HealthUpdateIDChannel;
        private int myInstanceID;
        private InstanceIdentity identity; 
        private Transform cameraTransform;
        private Coroutine healthBarCoroutine;
        public HealthBarCleanupFinishedSO OnCleanupFinishedEvent; 

        void Awake()
        {
            identity = GetComponentInParent<InstanceIdentity>(); 
        
            if (identity == null)
            {
                GameObject root = transform.root.gameObject;
                myInstanceID = root.GetInstanceID();
                Debug.LogError($"[HealthBar] InstanceIdentity component missing on {root.name}. Cannot guarantee correct ID matching.");
            }
            else
            {
                myInstanceID = identity.GetID();
            }
            
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogError("No Main Camera found in the scene! Health bar cannot look at the camera.");
            }
            if (healthSlider == null || easeHealthSlider == null)
            {
                Debug.LogError("HealthBarController requires HealthSlider and LerpSlider to be assigned.");
            }
        }

        void Update()
        {
            if (cameraTransform != null)
            {
                transform.LookAt(transform.position + cameraTransform.forward);
            }
        }

        void OnEnable()
        {
            if (HealthUpdateIDChannel  != null)
            {
                HealthUpdateIDChannel.OnEventRaised += OnGlobalHealthChanged;
            }
        }

        void OnDisable()
        {
            if (HealthUpdateIDChannel != null)
            {
                HealthUpdateIDChannel.OnEventRaised -= OnGlobalHealthChanged;
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
                //Debug.Log($"Controller ID MISMATCH. My ID: {myInstanceID}, Received ID: {eventInstanceID}");
            }
        }
        private void UpdateHealthBarUI(float currentHealth, float maxHealth)
        {
            if (easeHealthSlider != null)
            {
                easeHealthSlider.value = currentHealth / maxHealth;
            }
        
            if (healthBarCoroutine != null)
            {
                StopCoroutine(healthBarCoroutine);
            }
        
            healthBarCoroutine = StartCoroutine(UpdateLerpSlider(currentHealth, maxHealth));
        }

        private IEnumerator UpdateLerpSlider(float currentHealth, float maxHealth)
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