using UnityEngine;
using System;

namespace BasicEnemy
{
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")] 
        public float maxHealth = 100f;
        public float currentHealth;

        [Header("Event Publishers")] 
        public SimpleEventSO OnDiedEvent;
        public HealthChangedEventSO OnHealthChangedIDEvent;
        public DamageTakenEventSO OnTakeDamageEvent;
        private int myInstanceID;
        private InstanceIdentity identity; 

        [Header("Damage Text Settings")] 
        public Vector3 damageTextOffset = new Vector3(0, 1.5f, 0);
        private bool isInvulnerable = false;
        private BuffManager buffManager;
        void Awake()
        {
            identity = GetComponent<InstanceIdentity>();
            if (identity == null)
            {
                myInstanceID = gameObject.GetInstanceID();
                Debug.LogError($"[Health:{myInstanceID}] InstanceIdentity missing. Using GameObject ID.");
            }
            else
            {
                myInstanceID = identity.GetID();
            }
            buffManager = GetComponent<BuffManager>();
        }

        void Start()
        {
            currentHealth = maxHealth;
            OnHealthChangedIDEvent?.RaiseEvent(myInstanceID, currentHealth, maxHealth);
        }

        public void TakeDamage(float damageAmount)
        {
            if (isInvulnerable)
            {
                //Debug.Log("Damage blocked by I-Frame.");
                return; 
            }
            float effectiveDamage = damageAmount;
            
            if (buffManager != null)
            {
                effectiveDamage = buffManager.ApplyDamageModifiers(damageAmount);
            }
            currentHealth -= effectiveDamage;
            currentHealth = Mathf.Max(currentHealth, 0);
            
            Vector3 spawnPosition = transform.position + damageTextOffset;
            OnTakeDamageEvent?.RaiseEvent(effectiveDamage, spawnPosition); 

            if (OnHealthChangedIDEvent != null)
            {
                OnHealthChangedIDEvent.RaiseEvent(myInstanceID, currentHealth, maxHealth); 
            }
            if (currentHealth <= 0)
            {
                this.enabled = false; 
            }
        }
        public void Heal(float amount)
        {
            if (amount <= 0 || currentHealth <= 0) return;
            currentHealth += amount;
            
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            OnHealthChangedIDEvent.RaiseEvent(myInstanceID, currentHealth, maxHealth); 
            //Debug.Log($"Player Healed by {amount}. Current HP: {currentHealth}");
        }
        public bool IsInvulnerable()
        {
            return isInvulnerable;
        }
        public void SetInvulnerable(bool invulnerable)
        {
            isInvulnerable = invulnerable;
        }

    }
}