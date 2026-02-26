using BasicEnemy;
using UnityEngine;


namespace BasicEnemy
{
    public class EnemyDeathHandler : MonoBehaviour
    {
        [Header("Dependencies")]
        public HealthChangedEventSO HealthUpdateIDChannel; 
        public EnemyAnimator enemyAnimator;
        private int myInstanceID; 
        private InstanceIdentity identity;
        private BasicEnemyAI aiController;
        private bool isHandlingDeath = false;
        
        [Header("Drop Settings")]
        [Tooltip("Prefab ของ Heal Pickup ที่จะดรอป")]
        public GameObject healPickupPrefab; 

        [Tooltip("โอกาสเป็นเปอร์เซ็นต์ (0-100) ที่จะดรอป Heal Pickup")]
        [Range(0, 100)]
        public float dropChancePercent = 20f;
        private Health enemyHealth;
        void Awake()
        {
            enemyHealth = GetComponent<Health>();
            aiController = GetComponent<BasicEnemyAI>();
            if (enemyAnimator == null) enemyAnimator = GetComponent<EnemyAnimator>();
            
            identity = GetComponent<InstanceIdentity>();
            if (identity == null)
            {
                Debug.LogError("Handler requires InstanceIdentity on Parent GameObject!");
            }
            else
            {
                myInstanceID = identity.GetID();
            }
        }

        void OnEnable()
        {
            if (HealthUpdateIDChannel != null)
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
            // 1. Log ID ของตัวมันเอง
            //Debug.Log($"[DeathHandler:{myInstanceID}] Received Event. EventID: {eventInstanceID}, Health: {currentHealth}");

            // 2. ตรวจสอบ ID
            if (eventInstanceID == myInstanceID)
            {
                //Debug.Log($"[DeathHandler:{myInstanceID}] ID MATCH! Handling death.");
        
                if (currentHealth <= 0)
                {
                    HandleDeath(); 
                    TryDropHealPickup();
                }
            }
        }
        private void TryDropHealPickup()
        {
            if (healPickupPrefab == null)
            {
                Debug.LogWarning($"Heal Pickup Prefab is not assigned on {gameObject.name}. Cannot drop item.");
                return;
            }
            
            float randomValue = Random.Range(0f, 100f);
            
            if (randomValue <= dropChancePercent)
            {
                Vector3 dropPosition = transform.position;
                dropPosition.y += 0f; 

                Instantiate(
                    healPickupPrefab, 
                    dropPosition, 
                    Quaternion.identity 
                );
                
                //Debug.Log($"Enemy dropped Heal Pickup with {dropChancePercent}% chance.");
            }
        }
        private void HandleDeath()
        {
            if (isHandlingDeath) return;
            isHandlingDeath = true;
            
            if (aiController != null)
            {
                aiController.DieLogic(); 
            }
            
            if (enemyAnimator != null)
            {
                enemyAnimator.TriggerDie();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void OnDeathAnimationEnd()
        {
            Destroy(gameObject);
        }
    }
}