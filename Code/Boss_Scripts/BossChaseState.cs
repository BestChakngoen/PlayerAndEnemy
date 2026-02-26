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
            fsm.ResumeMovement();     
            currentSpeed = -1f;
        }

        public override void Update()
        {
            if (fsm.playerTransform == null) return;

            float distance = Vector3.Distance(
                fsm.transform.position,
                fsm.playerTransform.position
            );

            // ===== Transition Logic =====
            if (distance <= fsm.skillController.roarSkill.roarRange && fsm.skillController.roarSkill.CanUse())
            {
                FSM.NextState = new BossRoarState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            if (distance > fsm.jumpAttackRange && fsm.skillController.jumpAttackSkill.CanUse())
            {
                FSM.NextState = new BossJumpAttackState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }
            else if (distance <= fsm.meleeTriggerDistance) 
            {
                FSM.NextState = new BossMeleeSequenceState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }
            else if (distance <= fsm.stoppingDistance)
            {
                FSM.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            // ===== Chase Logic =====
            fsm.LookAtPlayerImmediate();

            float targetSpeed = CalculateMoveSpeed();
            if (currentSpeed != targetSpeed)
            {
                currentSpeed = targetSpeed;

                bool shouldBeRunning =
                    (fsm.bossHealth.currentHealth <=
                     fsm.bossHealth.maxHealth * fsm.runThreshold);

                fsm.bossAnimator.SetRunning(shouldBeRunning);
            }

            // เคลื่อนที่
            Vector3 previousPosition = fsm.transform.position;
            MoveTowardsPlayer(targetSpeed);

            // คำนวณ speed สำหรับ Animator
            float moveSpeed =
                (fsm.transform.position - previousPosition).magnitude / Time.deltaTime;

            fsm.bossAnimator.SetSpeed(moveSpeed);
        }

        // เพิ่มฟังก์ชันคำนวณความเร็วเข้ามาใน State โดยตรง
        private float CalculateMoveSpeed()
        {
            float baseSpeed =
                (fsm.bossHealth.currentHealth <= fsm.bossHealth.maxHealth * fsm.runThreshold)
                ? fsm.runSpeed
                : fsm.walkSpeed;

            if (fsm.IsPhase2 && fsm.enrageData != null)
                return baseSpeed * fsm.enrageData.speedMultiplier;

            return baseSpeed;
        }

        // เพิ่มฟังก์ชันเคลื่อนที่เข้ามาใน State โดยตรง
        private void MoveTowardsPlayer(float speed)
        {
            Vector3 target = fsm.playerTransform.position;
            target.y = fsm.transform.position.y; // ไม่ขยับในแกน Y เพื่อป้องกันบอสจมหรือลอย

            fsm.transform.position = Vector3.MoveTowards(
                fsm.transform.position,
                target,
                speed * Time.deltaTime
            );
        }

        public override void Exit()
        {
            base.Exit();
            fsm.StopMovement();       
            fsm.bossAnimator.SetSpeed(0f);
        }
    }
}