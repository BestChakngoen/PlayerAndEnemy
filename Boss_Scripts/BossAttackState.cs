using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossAttackState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;

        public BossAttackState(FiniteStateMachine fsm) : base(fsm) 
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.LookAtPlayerImmediate(); 
            fsm.bossAnimator.TriggerAttack();
        }

        public override void Update() { }

        public void OnAttackAnimationEnd()
        {
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }
        public void OnActionSequenceEnd() { }
    }
}