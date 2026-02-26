using UnityEngine;
using GameManger;
using BasicEnemy;
using System.Collections;
using System;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    [RequireComponent(typeof(BuffManager))]
    public class BossFSM : FiniteStateMachine
    {
        [Header("Movement")]
        public float walkSpeed = 3f;
        public float runSpeed = 6f;
        public float stoppingDistance = 1.5f;
        public float attackRange = 1.5f;

        [Header("Jump Attack")]
        public float jumpAttackRange = 10f;

        [Header("Boss State")]
        public float runThreshold = 0.5f;

        [Header("Phase 2")]
        public EnrageBuffData enrageData;

        [Header("Dependencies")]
        [HideInInspector] public BossAnimator bossAnimator;
        [HideInInspector] public BossSkills bossSkills;
        [HideInInspector] public Transform playerTransform;
        [HideInInspector] public Health bossHealth;

        private BuffManager buffManager;
        private Animator animator;

        private bool isDead = false;
        private bool isPhase2 = false;
        private bool isStopped = false;

        public bool IsPhase2 => isPhase2;

        void Awake()
        {
            bossHealth = GetComponent<Health>();
            bossAnimator = GetComponent<BossAnimator>();
            bossSkills = GetComponent<BossSkills>();
            animator = GetComponent<Animator>();
            buffManager = GetComponent<BuffManager>();

    
            
                // โหมด single player ใช้วิธีเดิม
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
            
            
            if (playerTransform == null)
                Debug.LogError("Player not found in scene!");
        }

        void Start()
        {
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

            if (enrageData == null)
            {
                Debug.LogError("EnrageData ไม่ได้ถูกกำหนดใน Inspector!", this);
            }
            else if (!isPhase2 &&
                     bossHealth.currentHealth <= bossHealth.maxHealth * enrageData.healthThreshold)
            {
                StartPhase2();
            }

            if (bossHealth.currentHealth <= 0)
            {
                DieLogic();
            }

            base.Update();
        }

        private void StartPhase2()
        {
            if (isPhase2) return;

            isPhase2 = true;
            buffManager.AddBuff(enrageData.CreateBuff());
        }

        public void NotifyPhase2Ended()
        {
            isPhase2 = false;
        }

        public interface IAnimationEventHandler
        {
            void OnAttackAnimationEnd();
            void OnDeathAnimationEnd();
            void OnRoarAnimationEnd();
        }

        public void OnAttackAnimationEnd()
        {
            (CurrentState as IAnimationEventHandler)?.OnAttackAnimationEnd();
        }

        public void OnDeathAnimationEnd()
        {
            (CurrentState as IAnimationEventHandler)?.OnDeathAnimationEnd();
        }

        public void OnRoarAnimationEnd()
        {
            (CurrentState as IAnimationEventHandler)?.OnRoarAnimationEnd();
        }

        public void DieLogic()
        {
            if (isDead) return;
            isDead = true;

            if (animator != null) animator.speed = 1f;

            buffManager?.ClearAllBuffs();

            NextState = new BossDieState(this);
            CurrentState.StateStage = StateEvent.EXIT;
        }

        public void RotateToPlayer()
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;
            transform.forward = direction.normalized;
        }

        public void MoveToPlayer()
        {
            if (isStopped) return;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= stoppingDistance) return;

            Vector3 target = playerTransform.position;
            target.y = transform.position.y;

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                GetCurrentMoveSpeed() * Time.deltaTime
            );
        }

        public float GetCurrentMoveSpeed()
        {
            float baseSpeed =
                (bossHealth.currentHealth <= bossHealth.maxHealth * runThreshold)
                ? runSpeed
                : walkSpeed;

            if (isPhase2 && enrageData != null)
                return baseSpeed * enrageData.speedMultiplier;

            return baseSpeed;
        }

        public void StopMovement()
        {
            isStopped = true;
        }

        public void ResumeMovement()
        {
            isStopped = false;
        }

        public Transform GetPlayerTransform() => playerTransform;
    }
}
