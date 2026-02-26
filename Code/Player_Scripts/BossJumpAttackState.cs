using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossJumpAttackState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;

        public BossJumpAttackState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();

            // แทน navMeshAgent.isStopped = true
            fsm.StopMovement();

            fsm.bossAnimator.SetSpeed(0f);
            fsm.RotateToPlayer();
            fsm.bossSkills.StartJumpAttackSequence();
        }

        public override void Update()
        {
            // JumpAttack เป็น animation-driven
            // ไม่ต้องมี movement logic ใด ๆ
        }

        public override void Exit()
        {
            base.Exit();

            // เตรียมกลับไปให้ State ถัดไปเคลื่อนที่ได้
            fsm.ResumeMovement();
        }

        public void OnAttackAnimationEnd()
        {
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }
    }
}