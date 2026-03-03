using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CoreSystem;

namespace GameSystem
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Health targetHealth;
        [SerializeField] private Image frontFill;
        [SerializeField] private Image backLerpFill;
        [SerializeField] private float lerpSpeed = 5f;
        [SerializeField] private GameObject uiContainer;
        [SerializeField] private bool hideOnDeath = false;

        private float currentFill = 1f;
        private float targetFill = 1f;

        private void Awake()
        {
            if (targetHealth != null)
            {
                targetHealth.OnHealthChanged += OnHealthChanged;
                targetHealth.OnDeath += OnDeath;
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            if (targetHealth != null)
            {
                targetFill = targetHealth.GetCurrentHealth() / targetHealth.GetMaxHealth();
                currentFill = targetFill;
                UpdateUIImmediate();
            }
        }

        private void OnDestroy()
        {
            if (targetHealth != null)
            {
                targetHealth.OnHealthChanged -= OnHealthChanged;
                targetHealth.OnDeath -= OnDeath;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (targetHealth != null)
            {
                targetFill = targetHealth.GetCurrentHealth() / targetHealth.GetMaxHealth();
                currentFill = targetFill;
                UpdateUIImmediate();
            }
        }

        private void Update()
        {
            if (Mathf.Abs(currentFill - targetFill) > 0.001f)
            {
                currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * lerpSpeed);
                
                if (backLerpFill != null)
                {
                    backLerpFill.fillAmount = currentFill;
                }
            }
        }

        private void OnHealthChanged(float currentHealth, float maxHealth)
        {
            targetFill = currentHealth / maxHealth;
            
            if (currentHealth >= maxHealth)
            {
                currentFill = targetFill;
                UpdateUIImmediate();
            }
            else
            {
                if (frontFill != null)
                {
                    frontFill.fillAmount = targetFill;
                }
            }
            
            if (uiContainer != null && !uiContainer.activeSelf && currentHealth > 0)
            {
                uiContainer.SetActive(true);
            }
        }

        private void OnDeath()
        {
            if (hideOnDeath && uiContainer != null)
            {
                uiContainer.SetActive(false);
            }
        }

        public void UpdateUIImmediate()
        {
            if (frontFill != null) frontFill.fillAmount = targetFill;
            if (backLerpFill != null) backLerpFill.fillAmount = targetFill;
            
            if (uiContainer != null)
            {
                uiContainer.SetActive(targetHealth != null && (targetHealth.GetCurrentHealth() > 0 || !hideOnDeath));
            }
        }
    }
}