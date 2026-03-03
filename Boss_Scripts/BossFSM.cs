using System.Collections.Generic;
using BasicEnemy;
using UnityEngine;
using Boss.core;
using CCSystem;
using CoreSystem;
using GameManger;

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
        
        [Header("Damage Settings")]
        public float baseMeleeDamage = 20f;
        [SerializeField] private ConeOverlapAttacker coneAttacker;

        [Header("CC Effects")]
        public List<CCEffectSO> ccEffects = new List<CCEffectSO>();
        
        [Header("Boss Audio")]
        public AudioClip[] screamSounds;
        public AudioClip[] teleportSounds;
        public AudioClip[] teleportSwipeSounds;

        [Header("Visual Effects")]
        public Renderer[] bossRenderers; 
        [Tooltip("ชื่อตัวแปร Reference ใน Shader Graph (มักจะมี _ นำหน้า)")]
        public string dissolvePropertyName = "_DissolveAmount"; 

        public List<BossSkillSO> bossSkills = new List<BossSkillSO>();
        private Dictionary<BossSkillSO, float> skillCooldownTimers = new Dictionary<BossSkillSO, float>();

        private bool isDead = false;

        protected virtual void Awake()
        {
            if (BossTransform == null) BossTransform = transform;
            if (bossAnimator == null) bossAnimator = GetComponent<BossAnimator>();
            if (coneAttacker == null) coneAttacker = GetComponent<ConeOverlapAttacker>();
        }

        protected virtual void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            foreach (var skill in bossSkills)
            {
                if (skill != null && !skillCooldownTimers.ContainsKey(skill))
                {
                    skillCooldownTimers[skill] = 0f;
                }
            }

            if (bossAnimator != null)
            {
                bossAnimator.OnDealDamage += HandleMeleeDamageEvent;
            }

            if (coneAttacker != null)
            {
                coneAttacker.OnTargetHit += HandleTargetHitCC;
            }

            CurrentState = new BossIdleState(this);
        }

        protected override void Update()
        {
            if (isDead)
            {
                base.Update();
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;

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

        // ฟังก์ชันสั่งการหายตัวของบอส (0 = ปรากฏ, 1 = หายตัว)
        public void SetDissolveAmount(float amount)
        {
            if (bossRenderers == null || bossRenderers.Length == 0) return;

            foreach (var r in bossRenderers)
            {
                if (r != null && r.material != null)
                {
                    // ส่งค่าไปที่ตัวแปรใน Shader
                    r.material.SetFloat(dissolvePropertyName, amount);
                }
            }
        }

        public void PlaySound(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0 && AudioManager.Instance != null)
            {
                AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                AudioManager.Instance.PlaySFX(clip, BossTransform.position);
            }
        }

        private void HandleMeleeDamageEvent()
        {
            if (coneAttacker != null && !isDead)
            {
                coneAttacker.Attack(baseMeleeDamage);
            }
        }

        private void HandleTargetHitCC(GameObject target)
        {
            if (ccEffects != null && ccEffects.Count > 0)
            {
                foreach (var effect in ccEffects)
                {
                    if (effect is KnockbackEffectSO knockbackEffect)
                    {
                        ICrowdControlReceiver receiver = target.GetComponentInChildren<ICrowdControlReceiver>();
                        if (receiver != null)
                        {
                            receiver.AddCC(knockbackEffect, BossTransform.position);
                        }
                        break;
                    }
                }
            }
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
        }

        public void ResumeMovement()
        {
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

        private void OnDestroy()
        {
            if (bossAnimator != null)
            {
                bossAnimator.OnDealDamage -= HandleMeleeDamageEvent;
            }
            if (coneAttacker != null)
            {
                coneAttacker.OnTargetHit -= HandleTargetHitCC;
            }
        }
    }
}