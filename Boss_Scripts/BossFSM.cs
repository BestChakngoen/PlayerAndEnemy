using UnityEngine;
using GameManger;
using BasicEnemy;
using System.Collections;
using System;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossFSM : FiniteStateMachine
    {
        [Header("Movement")]
        public float walkSpeed = 3f;
        public float runSpeed = 6f;
        public float stoppingDistance = 1.5f;
        
        [Header("New Boss Logic Settings")]
        public float meleeTriggerDistance = 1.5f; // ระยะที่เข้าใกล้แล้วจะโดนต่อย
        // ลบ teleportCooldown และ lastTeleportTime เดิมออกไปเลย

        [Header("Jump Attack (Legacy)")]
        public float jumpAttackRange = 10f;

        [Header("Boss State")]
        public float runThreshold = 0.5f;

        [Header("Phase 2")]

        [Header("Dependencies")]
        [HideInInspector] public BossAnimator bossAnimator;
        [HideInInspector] public Transform playerTransform;
        //[HideInInspector] public Health bossHealth;

        //private BuffManager buffManager;
        private Animator animator;

        private bool isDead = false;
        private bool isPhase2 = false;
        private bool isStopped = false;

        public bool IsPhase2 => isPhase2;

        void Awake()
        {
            //bossHealth = GetComponent<Health>();
            bossAnimator = GetComponent<BossAnimator>();
            animator = GetComponent<Animator>();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            
            if (playerTransform == null)
                Debug.LogError("Player not found in scene!");
        }

        void Start()
        {
            // ลบ lastTeleportTime = Time.time; บรรทัดนี้ออกด้วยครับ
            CurrentState = new BossIdleState(this);
        }

        protected override void Update()
        {
            if (isDead) return;

            if (playerTransform == null)
            {
                if (!(CurrentState is BossIdleState))
                    CurrentState = new BossIdleState(this);

                base.Update();
                return;
            }

            /*if (enrageData != null && !isPhase2 &&
                bossHealth.currentHealth <= bossHealth.maxHealth * enrageData.healthThreshold)
            {
                StartPhase2();
            }

            if (bossHealth.currentHealth <= 0)
            {
                DieLogic();
            }*/

            base.Update();
        }

        private void StartPhase2()
        {
            if (isPhase2) return;
            isPhase2 = true;
        }

        public void NotifyPhase2Ended()
        {
            isPhase2 = false;
        }

        // Interface สำหรับจัดการ Animation Event
        public interface IAnimationEventHandler
        {
            void OnAttackAnimationEnd();
            void OnDeathAnimationEnd();
            void OnRoarAnimationEnd();
            void OnActionSequenceEnd(); // เพิ่มสำหรับ Sequence ใหม่ๆ
        }

        public void OnAttackAnimationEnd() => (CurrentState as IAnimationEventHandler)?.OnAttackAnimationEnd();
        public void OnDeathAnimationEnd() => (CurrentState as IAnimationEventHandler)?.OnDeathAnimationEnd();
        public void OnRoarAnimationEnd() => (CurrentState as IAnimationEventHandler)?.OnRoarAnimationEnd();
        public void OnActionSequenceEnd() => (CurrentState as IAnimationEventHandler)?.OnActionSequenceEnd();

        public void DieLogic()
        {
            if (isDead) return;
            isDead = true;

            if (animator != null) animator.speed = 1f;

            NextState = new BossDieState(this);
            CurrentState.StateStage = StateEvent.EXIT;
        }

        public void RotateToPlayerSmoothly(float speed = 5f)
        {
            if (playerTransform == null) return;
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }

        public void LookAtPlayerImmediate()
        {
            if (playerTransform == null) return;
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;
            transform.forward = direction.normalized;
        }

        public void StopMovement() => isStopped = true;
        public void ResumeMovement() => isStopped = false;
        public Transform GetPlayerTransform() => playerTransform;
    }
}