using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossChaseState : State
    {
        private BossFSM fsm;
        private float currentSpeed;

        public BossChaseState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.ResumeMovement();     // แทน navMeshAgent.isStopped = false
            currentSpeed = -1f;
        }

        public override void Update()
        {
            if (fsm.playerTransform == null) return;

            float distance = Vector3.Distance(
                fsm.transform.position,
                fsm.playerTransform.position
            );

            // ===== Transition Logic (เหมือนเดิมทุกประการ) =====
            if (distance <= fsm.bossSkills.roarRange && fsm.bossSkills.CanRoar())
            {
                FSM.NextState = new BossRoarState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            if (distance > fsm.jumpAttackRange && fsm.bossSkills.CanPerformJumpAttack())
            {
                FSM.NextState = new BossJumpAttackState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }
            else if (distance <= fsm.attackRange)
            {
                FSM.NextState = new BossAttackState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }
            else if (distance <= fsm.stoppingDistance)
            {
                FSM.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            // ===== Chase Logic (แทน NavMesh) =====
            fsm.RotateToPlayer();

            float targetSpeed = fsm.GetCurrentMoveSpeed();
            if (currentSpeed != targetSpeed)
            {
                currentSpeed = targetSpeed;

                bool shouldBeRunning =
                    (fsm.bossHealth.currentHealth <=
                     fsm.bossHealth.maxHealth * fsm.runThreshold);

                fsm.bossAnimator.SetRunning(shouldBeRunning);
            }

            // เคลื่อนที่ด้วย Transform
            Vector3 previousPosition = fsm.transform.position;
            fsm.MoveToPlayer();

            // คำนวณ speed สำหรับ Animator (แทน navMeshAgent.velocity)
            float moveSpeed =
                (fsm.transform.position - previousPosition).magnitude / Time.deltaTime;

            fsm.bossAnimator.SetSpeed(moveSpeed);
        }

        public override void Exit()
        {
            base.Exit();
            fsm.StopMovement();       // แทน navMeshAgent.isStopped = true
            fsm.bossAnimator.SetSpeed(0f);
        }
    }
}
