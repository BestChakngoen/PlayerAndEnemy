using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossJumpAttackState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;
        
        // ตัวจับเวลาเผื่อกันบัคค้าง
        private float stuckTimer = 0f;
        private float maxWaitTime = 6f; // ให้เวลาสูงสุด 6 วินาทีในการกระโดด (ปรับได้ตามความยาวแอนิเมชัน)

        public BossJumpAttackState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();

            fsm.StopMovement();
            fsm.bossAnimator.SetSpeed(0f);
            fsm.LookAtPlayerImmediate();
            
            // สั่งเริ่มกระบวนการกระโดด
            fsm.skillController.jumpAttackSkill.StartSequence();
            
            stuckTimer = 0f; // เริ่มนับเวลา
        }

        public override void Update()
        {
            // ระบบป้องกันบอสค้าง (Failsafe) 
            // กรณีที่ลืมใส่ Animation Event หรือแอนิเมชันไม่ยอมเล่นตาม Transition
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= maxWaitTime)
            {
                Debug.LogWarning("[Boss AI] ⚠️ บังคับออกจาก JumpAttack! เนื่องจากรอแอนิเมชันนานเกินไป (ลืมใส่ Event หรือเปล่า?)");
                
                // บังคับรีเซ็ตสถานะการกระโดดให้กลับมาพร้อมใช้งานรอบถัดไป
                if (fsm.skillController.jumpAttackSkill.IsJumping)
                {
                    fsm.skillController.jumpAttackSkill.OnAnimationEnd();
                }

                FSM.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
            }
        }

        public override void Exit()
        {
            base.Exit();

            // เตรียมกลับไปให้ State ถัดไปเคลื่อนที่ได้
            fsm.ResumeMovement();
        }

        // ฟังก์ชันนี้จะถูกเรียกปกติเมื่อแอนิเมชันเล่นจบ (ถ้าตั้ง Event ไว้ถูกต้อง)
        public void OnAttackAnimationEnd()
        {
            Debug.Log("[Boss AI] Jump Attack จบสมบูรณ์ตามปกติ");
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }
        public void OnActionSequenceEnd() { }
    }
}