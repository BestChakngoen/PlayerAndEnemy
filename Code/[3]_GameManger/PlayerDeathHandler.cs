using System;
using UnityEngine;
using BasicEnemy;
using PlayerInputs;

namespace GameManger
{
    public class PlayerDeathHandler : MonoBehaviour
    {
        [Header("Scriptable Event Subscriber")]
        public HealthChangedEventSO HealthUpdateIDChannel; 
        private int myInstanceID;
        private PlayerAnimationFacade playerAnimator;
        private InstanceIdentity identity;
        //private bool isHandlingDeath = false;

        private void Awake()
        {
            if (playerAnimator == null) playerAnimator = GetComponentInParent<PlayerAnimationFacade>();
            identity = GetComponentInParent<InstanceIdentity>();
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
                    HandlePlayerDeath(); 
                }
            }
        }
        private void HandlePlayerDeath()
        {
            Animator animator = playerAnimator.GetAnimator();
            int jumpScareLayerIndex = animator.GetLayerIndex("JumpScare Layer");
            if (jumpScareLayerIndex >= 0)
            {
                animator.SetLayerWeight(jumpScareLayerIndex, 0f);
                playerAnimator.PlayDie();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void KillByJumpScare()
        {
            HandlePlayerDeath();
        }

    }
}