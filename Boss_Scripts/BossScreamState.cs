using BasicEnemy;
using UnityEngine;
using Boss.core;
using CCSystem;

namespace Boss.scripts
{
    public class BossScreamState : State
    {
        private BossFSM fsm;
        private BossScreamSkillSO skill;
        private float actionTimer;
        private bool hasDebuffed;

        public BossScreamState(BossFSM fsm, BossScreamSkillSO skill) : base(fsm)
        {
            this.fsm = fsm;
            this.skill = skill;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            actionTimer = 0f;
            hasDebuffed = false;
            
            Animator anim = fsm.bossAnimator.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Scream");
            }
        }

        public override void Update()
        {
            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
                float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;

                actionTimer += Time.deltaTime;

                if (!hasDebuffed && actionTimer >= currentAnimLength * 0.5f)
                {
                    hasDebuffed = true;
                    ApplyScreamStun();
                }

                if (actionTimer >= currentAnimLength + 0.1f)
                {
                    fsm.NextState = new BossIdleState(fsm);
                    StateStage = StateEvent.EXIT;
                }
            }
        }

        private void ApplyScreamStun()
        {
            if (fsm.playerTransform != null && skill != null)
            {
                float distance = Vector3.Distance(fsm.BossTransform.position, fsm.playerTransform.position);
                if (distance <= skill.effectRange && skill.stunEffect != null)
                {
                    ICrowdControlReceiver ccReceiver = fsm.playerTransform.GetComponentInChildren<ICrowdControlReceiver>();
                    if (ccReceiver != null)
                    {
                        ccReceiver.AddCC(skill.stunEffect, fsm.BossTransform.position);
                    }
                }
            }
        }
    }
}