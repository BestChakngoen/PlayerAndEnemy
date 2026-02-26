using BasicEnemy;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossDeathHandler : MonoBehaviour
    {
        [Header("Dependencies")] public HealthChangedEventSO HealthUpdateIDChannel;
        [FormerlySerializedAs("wendigoAnimator")] public BossAnimator animator;
        private int myInstanceID;
        private InstanceIdentity identity;
        private BossAI aiController;
        private bool isHandlingDeath = false;

        void Awake()
        {
            aiController = GetComponent<BossAI>();
            if (animator == null) animator = GetComponent<BossAnimator>();

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
                }
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

            if (animator != null)
            {
                animator.TriggerDie();
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