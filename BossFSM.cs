using System.Collections.Generic;
using BasicEnemy;
using UnityEngine;
using Boss.core;

namespace Boss.scripts
{
    public class BossFSM : FiniteStateMachine, ISpeedModifiable
    {
        public Transform playerTransform;
        public Transform BossTransform;
        public BossAnimator bossAnimator;
        
        public float meleeTriggerDistance = 2.0f;
        public float meleeAttackCooldown = 3.0f;
        [HideInInspector] public float meleeAttackTimer = 0f;
        public float baseSpeedMultiplier = 1.0f; 
        
        public List<BossSkillSO> bossSkills = new List<BossSkillSO>();
        private Dictionary<BossSkillSO, float> skillCooldownTimers = new Dictionary<BossSkillSO, float>();

        private bool isDead = false;
        private bool isStopped = false;

        protected virtual void Awake()
        {
            if (BossTransform == null) BossTransform = transform;
            if (bossAnimator == null) bossAnimator = GetComponent<BossAnimator>();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        protected virtual void Start()
        {
            foreach (var skill in bossSkills)
            {
                if (skill != null && !skillCooldownTimers.ContainsKey(skill))
                {
                    skillCooldownTimers[skill] = 0f;
                }
            }

            CurrentState = new BossIdleState(this);
        }

        protected override void Update()
        {
            if (isDead) return;

            if (meleeAttackTimer > 0) meleeAttackTimer -= Time.deltaTime;

            if (playerTransform == null)
            {
                if (!(CurrentState is BossIdleState))
                    CurrentState = new BossIdleState(this);

                base.Update();
                return;
            }

            UpdateSkillCooldowns();
            
            if (TryExecuteSkills())
            {
                return;
            }

            base.Update();
        }

        private void UpdateSkillCooldowns()
        {
            List<BossSkillSO> keys = new List<BossSkillSO>(skillCooldownTimers.Keys);
            foreach (var skill in keys)
            {
                if (skillCooldownTimers[skill] > 0)
                {
                    skillCooldownTimers[skill] -= Time.deltaTime;
                }
            }
        }

        private bool TryExecuteSkills()
        {
            foreach (var skill in bossSkills)
            {
                if (skillCooldownTimers[skill] <= 0f && skill.CanExecute(this))
                {
                    skillCooldownTimers[skill] = skill.cooldown;
                    skill.Execute(this);
                    return true;
                }
            }
            return false;
        }

        public void MultiplySpeed(float multiplier)
        {
            baseSpeedMultiplier *= multiplier;
            Animator anim = bossAnimator.GetComponent<Animator>();
            if(anim != null) anim.speed *= multiplier;
        }

        public void DivideSpeed(float multiplier)
        {
            if (multiplier != 0)
            {
                baseSpeedMultiplier /= multiplier;
                Animator anim = bossAnimator.GetComponent<Animator>();
                if(anim != null) anim.speed /= multiplier;
            }
        }

        public void StopMovement()
        {
            isStopped = true;
        }

        public void ResumeMovement()
        {
            isStopped = false;
        }

        public void LookAtPlayerImmediate()
        {
            if (playerTransform == null) return;
            
            Vector3 dir = playerTransform.position - BossTransform.position;
            dir.y = 0;
            
            if (dir != Vector3.zero)
            {
                BossTransform.rotation = Quaternion.LookRotation(dir);
            }
        }

        public void RotateToPlayerSmoothly(float speed)
        {
            if (playerTransform == null) return;
            
            Vector3 dir = playerTransform.position - BossTransform.position;
            dir.y = 0;
            
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                BossTransform.rotation = Quaternion.Slerp(BossTransform.rotation, targetRot, speed * Time.deltaTime);
            }
        }

        public void DieLogic()
        {
            if (isDead) return;
            isDead = true;

            NextState = new BossDieState(this);
            if (CurrentState != null)
            {
                CurrentState.StateStage = StateEvent.EXIT;
            }
        }
    }
}