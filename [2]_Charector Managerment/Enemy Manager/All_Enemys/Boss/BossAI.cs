using BasicEnemy;
using UnityEngine;
using UnityEngine.AI;
using GameManger;
using System;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossAI : MonoBehaviour
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
        
        [Header("Dependencies")]
        public BossAnimator bossAnimator; 
        public BossSkills bossSkills;       

        private NavMeshAgent navMeshAgent;
        private Transform playerTransform;
        private Health bossHealth;
        
        private float currentMoveSpeed;
        private bool isAttacking = false;
        private bool isDead;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            bossHealth = GetComponent<Health>();
            
            if (bossAnimator == null) bossAnimator = GetComponent<BossAnimator>();
            if (bossSkills == null) bossSkills = GetComponent<BossSkills>();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else Debug.LogError("Player GameObject with 'Player' tag not found!");

            currentMoveSpeed = walkSpeed;
            navMeshAgent.speed = currentMoveSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
        }
        void Update()
        {
            //if (playerTransform == null || bossSkills.IsJumping) return;
            
            UpdateMovementSpeed(); 

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer > jumpAttackRange && bossSkills.CanPerformJumpAttack())
            {
                bossSkills.StartJumpAttackSequence();
            }
            else if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else if (distanceToPlayer > stoppingDistance)
            {
                ChasePlayer();
            }
            else
            {
                StopChasing();
            }
            
            bossAnimator.SetSpeed(navMeshAgent.velocity.magnitude);
        }
        private void UpdateMovementSpeed()
        {
            if (bossHealth != null && bossHealth.currentHealth <= bossHealth.maxHealth * runThreshold)
            {
                if (currentMoveSpeed != runSpeed)
                {
                    currentMoveSpeed = runSpeed;
                    navMeshAgent.speed = currentMoveSpeed;
                    bossAnimator.SetRunning(true);
                }
            }
            else if (currentMoveSpeed != walkSpeed)
            {
                currentMoveSpeed = walkSpeed;
                navMeshAgent.speed = currentMoveSpeed;
                bossAnimator.SetRunning(false);
            }
        }
        private void ChasePlayer()
        {
            if (isAttacking) return;
            navMeshAgent.isStopped = false;
            RotateToPlayer();
            navMeshAgent.SetDestination(playerTransform.position);
        }
        private void StopChasing()
        {
            if (isAttacking) return;
            navMeshAgent.isStopped = true;
        }
        private void AttackPlayer()
        {
            if (!isAttacking)
            {
                isAttacking = true;
                StopChasing();
                RotateToPlayer();
                bossAnimator.TriggerAttack(); 
            }
        }
        private void RotateToPlayer()
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            directionToPlayer.y = 0;
            transform.forward = directionToPlayer;
        }
        public void OnAttackAnimationEnd()
        {
            isAttacking = false;
            navMeshAgent.isStopped = false;
        }
        public void DieLogic()
        {
            if (isDead) return;
            isDead = true;
            StopChasing();
            navMeshAgent.enabled = false;

            bossAnimator.TriggerDie();

        }
        public void OnDeathAnimationEnd()
        {
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.MonsterDied();
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.AddKill();
            }
            Destroy(gameObject);
        }
        public void StopMovement() => navMeshAgent.isStopped = true;
        public void ResumeMovement() => navMeshAgent.isStopped = false;
        public float GetCurrentSpeed() => currentMoveSpeed;
        public Transform GetPlayerTransform() => playerTransform;
    }
}