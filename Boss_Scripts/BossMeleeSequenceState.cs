using UnityEngine;
using System.Collections;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossMeleeSequenceState : State
    {
        private BossFSM fsm;
        private Coroutine sequenceCoroutine;

        public BossMeleeSequenceState(FiniteStateMachine fsm) : base(fsm) 
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            fsm.LookAtPlayerImmediate(); // หันหน้าเข้าหาเป้าหมายก่อนโจมตี
            
            sequenceCoroutine = fsm.StartCoroutine(ExecuteMeleeSequence());
        }

        private IEnumerator ExecuteMeleeSequence()
        {
            // 1. เล่น Animation โจมตีเร็ว
            fsm.bossAnimator.TriggerMutantPunch();
            // รอจนกว่า Animation ต่อยจะจบ (สมมติใช้เวลา 1 วินาที หรือปรับตามความยาวคลิปจริง)
            yield return new WaitForSeconds(1.2f); 

            // 2. ถอยไปตั้งหลัก ซ้าย หรือ ขวา แบบสุ่ม
            bool goLeft = Random.value > 0.5f;
            fsm.bossAnimator.TriggerGetAway(goLeft);
            yield return new WaitForSeconds(1.0f); // รอระยะเวลาถอย

            // 3. กรีดร้อง
            fsm.bossAnimator.TriggerScream();
            yield return new WaitForSeconds(1.5f); // รอจนกรีดร้องเสร็จ

            // กลับไปตั้งหลักที่ Idle
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public override void Update()
        {
            // ปล่อยให้ Coroutine จัดการ
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