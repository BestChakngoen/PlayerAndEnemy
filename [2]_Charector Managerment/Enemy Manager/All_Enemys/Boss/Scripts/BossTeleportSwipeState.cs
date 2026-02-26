using UnityEngine;
using System.Collections;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossTeleportSwipeState : State
    {
        private BossFSM fsm;
        private Coroutine sequenceCoroutine;

        public BossTeleportSwipeState(FiniteStateMachine fsm) : base(fsm) 
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            
            sequenceCoroutine = fsm.StartCoroutine(ExecuteTeleportSequence());
        }

        private IEnumerator ExecuteTeleportSequence()
        {
            // 1. เรียกใช้งานสกิล Teleport ซึ่งรวม Logic การคำนวณระยะและการวาร์ปไว้หมดแล้ว
            fsm.skillController.teleportSkill.PerformTeleport(fsm.playerTransform);

            // 2. หน่วงเวลาเตรียมโจมตี 0.5 - 1.0 วินาที (ให้ผู้เล่นกลิ้งหลบ)
            float warningDelay = Random.Range(0.5f, 1.0f);
            yield return new WaitForSeconds(warningDelay);

            // 3. สั่งเล่น Animation โจมตี (Swiping)
            fsm.bossAnimator.TriggerSwiping();
            
            // 4. รอจนกว่าคลิปแอนิเมชัน Swiping จะเล่นจบ
            yield return new WaitForSeconds(1.5f);

            // 5. รีเซ็ตเวลาจับเวลา Teleport ใหม่ (เรียกใช้งาน TriggerCooldown จากสกิลแทน)
            fsm.skillController.teleportSkill.TriggerCooldown();

            // 6. กลับไปตั้งหลักที่ Idle State
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public override void Update()
        {
            // ปล่อยให้ Coroutine จัดการ Sequence ตามเวลา
        }

        public override void Exit()
        {
            if (sequenceCoroutine != null)
            {
                fsm.StopCoroutine(sequenceCoroutine);
            }
            base.Exit();
        }
    }
}