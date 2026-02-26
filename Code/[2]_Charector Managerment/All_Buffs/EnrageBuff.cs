using UnityEngine;
using BasicEnemy.Enemy.Wendigo_FolkFall;
using BasicEnemy;

namespace BasicEnemy
{
    public class EnrageBuff : Buff
    {
        private EnrageBuffData data;
        private Animator animator;
        private Weapon weapon;
        private BossSkills skills;
        private GameObject auraInstance;
        private float baseAnimSpeed;
        private float baseDamage;
        private float regenTimer;

        public EnrageBuff(EnrageBuffData data) : base(data.duration) 
        {
            this.data = data;
        }

        public override void OnApply(GameObject target)
        {
            base.OnApply(target); 
            
            animator = target.GetComponent<Animator>();
            weapon = target.GetComponentInChildren<Weapon>();
            skills = target.GetComponent<BossSkills>();
            
            if (animator != null) baseAnimSpeed = animator.speed;
            if (weapon != null) baseDamage = weapon.damage;
            
            if (data.auraEffectPrefab != null)
            {
                auraInstance = Object.Instantiate(data.auraEffectPrefab, target.transform);
            }
            if (animator != null)
            {
                animator.speed = data.attackSpeedMultiplier;
            }
            if (weapon != null)
            {
                weapon.damage = baseDamage * data.damageMultiplier;
            }
            if (skills != null)
            {
                skills.ApplyCooldownModifier(data.skillCDReduction);
            }
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime); 
            if (targetHealth.currentHealth > targetHealth.maxHealth * data.healthThreshold)
            {
                IsFinished = true; 
                return; 
            }
            
            regenTimer += deltaTime;
            if (regenTimer >= data.regenInterval)
            {
                regenTimer -= data.regenInterval;
                if (targetHealth != null)
                {
                    float healAmount = targetHealth.maxHealth * data.regenPercent;
                    targetHealth.Heal(healAmount);
                }
            }
        }

        public override float ModifyIncomingDamage(float damage)
        {
            return damage * (1f - data.damageReduction);
        }

        public override void OnExpire()
        {
            base.OnExpire();
            
            if (animator != null)
                animator.speed = baseAnimSpeed;
            if (weapon != null)
                weapon.damage = baseDamage;
            if (skills != null)
                skills.ApplyCooldownModifier(0f); 
            if (auraInstance != null)
                Object.Destroy(auraInstance);
            
            var fsm = target.GetComponent<BossFSM>();
            if (fsm != null)
            {
                fsm.NotifyPhase2Ended();
            }
        }
    }
}