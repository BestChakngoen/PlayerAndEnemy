using BasicEnemy;
using UnityEngine;
using Boss.core;

namespace Boss.scripts
{
    public class BossScreamState : State
    {
        private BossFSM fsm;
        private BossScreamSkillSO skill;
        private float actionTimer;
        private bool hasDebuffed = false;

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
            if (anim != null) anim.SetFloat("Speed", 0f);
            
            fsm.bossAnimator.TriggerScream();
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
                    ApplyScreamDebuff();
                }

                if (actionTimer >= currentAnimLength + 0.1f)
                {
                    fsm.NextState = new BossIdleState(fsm);
                    StateStage = StateEvent.EXIT;
                }
            }
        }

        private void ApplyScreamDebuff()
        {
            if (fsm.playerTransform != null && skill != null && skill.debuffEffect != null)
            {
                float distance = Vector3.Distance(fsm.BossTransform.position, fsm.playerTransform.position);
                if (distance <= skill.effectRange)
                {
                    IEffectable effectable = fsm.playerTransform.GetComponent<IEffectable>();
                    if (effectable != null)
                    {
                        effectable.AddEffect(skill.debuffEffect);
                    }
                }
            }
        }
    }
}