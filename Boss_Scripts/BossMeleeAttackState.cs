using BasicEnemy;
using UnityEngine;
using Boss.core;
using CCSystem;

namespace Boss.scripts
{
    public class BossMeleeAttackState : State
    {
        private BossFSM fsm;
        private float actionTimer;
        private bool hasDealtDamage;

        public BossMeleeAttackState(BossFSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }

        public override void Enter()
        {
            base.Enter();
            actionTimer = 0f;
            hasDealtDamage = false; 
            fsm.meleeAttackTimer = fsm.meleeAttackCooldown;
            fsm.StopMovement();
            fsm.LookAtPlayerImmediate();
            fsm.bossAnimator.TriggerMutantPunch();
        }

        public override void Update()
        {
            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
                float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;

                actionTimer += Time.deltaTime;

                if (!hasDealtDamage && actionTimer >= currentAnimLength * 0.5f)
                {
                    hasDealtDamage = true;
                    ApplyCCEffect();
                }

                if (actionTimer >= currentAnimLength + 0.1f)
                {
                    fsm.NextState = new BossWalkBackState(fsm);
                    StateStage = StateEvent.EXIT;
                }
            }
        }

        private void ApplyCCEffect()
        {
            if (fsm.playerTransform != null)
            {
                float distance = Vector3.Distance(fsm.BossTransform.position, fsm.playerTransform.position);
                if (distance <= fsm.meleeTriggerDistance + 1.0f) 
                {
                    if (fsm.ccEffects != null)
                    {
                        foreach (var effect in fsm.ccEffects)
                        {
                            if (effect is KnockbackEffectSO knockbackEffect)
                            {
                                ICrowdControlReceiver receiver = fsm.playerTransform.GetComponentInChildren<ICrowdControlReceiver>();
                                if (receiver != null)
                                {
                                    receiver.AddCC(knockbackEffect, fsm.BossTransform.position);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            if (animator != null)
            {
                animator.ResetTrigger("MutantPunch");
            }
        }
    }
}