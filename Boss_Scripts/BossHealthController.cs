using UnityEngine;
using CoreSystem;
using GameManger;

namespace Boss.scripts
{
    [RequireComponent(typeof(Health))]
    public class BossHealthController : MonoBehaviour
    {
        private Health health;
        [SerializeField] private BossFSM bossFSM;

        [Header("Audio")]
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip[] dieSounds;

        private float previousHealth;

        private void Awake()
        {
            health = GetComponent<Health>();
            if (bossFSM == null) bossFSM = GetComponent<BossFSM>();
        }

        private void Start()
        {
            if (health != null)
            {
                previousHealth = health.GetMaxHealth();
            }
        }

        private void OnEnable()
        {
            health.OnDeath += HandleDeath;
            health.OnHealthChanged += HandleHealthChanged;
        }

        private void OnDisable()
        {
            health.OnDeath -= HandleDeath;
            health.OnHealthChanged -= HandleHealthChanged;
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            if (currentHealth < previousHealth && currentHealth > 0)
            {
                if (hitSounds != null && hitSounds.Length > 0 && AudioManager.Instance != null)
                {
                    AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
                    AudioManager.Instance.PlaySFX(clip, transform.position);
                }
            }
            previousHealth = currentHealth;
        }

        private void HandleDeath()
        {
            if (dieSounds != null && dieSounds.Length > 0 && AudioManager.Instance != null)
            {
                AudioClip clip = dieSounds[Random.Range(0, dieSounds.Length)];
                AudioManager.Instance.PlaySFX(clip, transform.position);
            }

            if (bossFSM != null)
            {
                bossFSM.DieLogic();
            }
        }
    }
}