using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossIdleState : State
    {
        private BossFSM fsm;

        public BossIdleState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();              // แทน navMeshAgent.isStopped = true
            fsm.bossAnimator.SetSpeed(0f);
        }

        public override void Update()
        {
            if (fsm.playerTransform == null) return;

            float distance = Vector3.Distance(
                fsm.transform.position,
                fsm.playerTransform.position
            );

            // ===== Transition Logic (เหมือนเดิม) =====
            if (distance > fsm.jumpAttackRange &&
                fsm.bossSkills.CanPerformJumpAttack())
            {
                FSM.NextState = new BossJumpAttackState(fsm);
                StateStage = StateEvent.EXIT;
            }
            else if (distance <= fsm.attackRange)
            {
                FSM.NextState = new BossAttackState(fsm);
                StateStage = StateEvent.EXIT;
            }
            else
            {
                FSM.NextState = new BossChaseState(fsm);
                StateStage = StateEvent.EXIT;
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}