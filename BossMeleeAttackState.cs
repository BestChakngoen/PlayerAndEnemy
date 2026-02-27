using BasicEnemy;
using UnityEngine;
using Boss.core;

namespace Boss.scripts
{
    public class BossMeleeAttackState : State
    {
        private BossFSM fsm;
        private float actionTimer;

        public BossMeleeAttackState(BossFSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }

        public override void Enter()
        {
            base.Enter();
            actionTimer = 0f;
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
                if (actionTimer >= currentAnimLength + 0.1f)
                {
                    fsm.NextState = new BossWalkBackState(fsm);
                    StateStage = StateEvent.EXIT;
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