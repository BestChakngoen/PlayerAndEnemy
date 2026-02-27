using UnityEngine;
using BasicEnemy;

namespace Boss.scripts
{
    public class BossAttackState : State
    {
        private BossFSM fsm;
        private float actionTimer;

        public BossAttackState(FiniteStateMachine fsm) : base(fsm) 
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            actionTimer = 0f;
            fsm.LookAtPlayerImmediate(); 
            fsm.bossAnimator.TriggerAttack();
        }

        public override void Update() 
        {
            Animator animator = fsm.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
            float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;

            actionTimer += Time.deltaTime;
            if (actionTimer >= currentAnimLength)
            {
                FSM.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
            }
        }
    }
}