using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossStareState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;
        private bool isTurning = false;
        
        // ตัวแปรสำหรับหน่วงเวลาไม่ให้เล่นอนิเมชันรัวเกินไป
        private float turnCooldownTimer = 0f;

        public BossStareState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            fsm.bossAnimator.SetSpeed(0f);
            
            // สั่งให้เล่นท่า Idle ทันทีที่เข้าสู่ State จ้องมอง
            //fsm.bossAnimator.GetComponent<Animator>().SetTrigger("Idle");
            
            isTurning = false; 
            turnCooldownTimer = 0f;
        }

        public override void Update()
        {
            if (fsm.playerTransform == null) return;

            // 1. คำนวณองศาและความต่างของการหันหน้าตลอดเวลา
            Vector3 dirToPlayer = fsm.playerTransform.position - fsm.transform.position;
            dirToPlayer.y = 0;

            if (dirToPlayer == Vector3.zero) return;

            float angleToPlayer = Vector3.SignedAngle(fsm.transform.forward, dirToPlayer, Vector3.up);
            float absAngle = Mathf.Abs(angleToPlayer);

            // 2. ตรวจสอบว่าหันตรงกับ Player หรือยัง (เผื่อระยะคลาดเคลื่อนให้ 15 องศา)
            // จะเช็คและออกไป Idle ก็ต่อเมื่อ "ไม่ได้กำลังหันอยู่" เพื่อให้แอนิเมชันเล่นจนจบจังหวะก่อน
            if (!isTurning && absAngle <= 15f)
            {
                Debug.Log("[Boss AI] หันตรงเป้าหมายแล้ว กลับไปหน้า Idle");
                FSM.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            // 3. นับเวลา Cooldown ถอยหลัง
            if (turnCooldownTimer > 0)
            {
                turnCooldownTimer -= Time.deltaTime;
            }

            // 4. สั่งเล่น Animation เมื่อไม่ได้หมุนอยู่, หมดคูลดาวน์แล้ว และองศายังไม่ตรงเป้าหมาย
            if (!isTurning && turnCooldownTimer <= 0f && absAngle > 15f)
            {
                isTurning = true;
                Animator animator = fsm.bossAnimator.GetComponent<Animator>();

                if (angleToPlayer > 0f) // ผู้เล่นอยู่ทางขวา
                {
                    if (absAngle > 60f)
                        animator.SetTrigger("TurnRight90");
                    else
                        animator.SetTrigger("TurnRight45");
                }
                else // ผู้เล่นอยู่ทางซ้าย (angleToPlayer < 0)
                {
                    if (absAngle > 60f)
                        animator.SetTrigger("TurnLeft90");
                    else
                        animator.SetTrigger("TurnLeft45");
                }
            }

            // 5. ให้การหมุน หมุนในขณะที่กำลังเล่น Animation เท่านั้น
            if (isTurning)
            {
                // ปรับความเร็วจาก 3f เป็น 6f เพื่อให้ Transform หมุนตามภาพแอนิเมชันได้ทันเวลา
                fsm.RotateToPlayerSmoothly(6f);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        // Event นี้จะถูกเรียกเมื่อ Animation หันตัวเล่นจบ 
        public void OnActionSequenceEnd()
        {
            // บังคับให้ Transform หันตรงเป้าหมายแบบ 100% ในเฟรมสุดท้ายที่ท่าหมุนเล่นจบพอดี
            // (ป้องกันปัญหา Transform หมุนไม่ทันแล้วโมเดลเด้งกลับตอนสลับไป Idle)
            fsm.LookAtPlayerImmediate();

            isTurning = false;
            // ใส่ Cooldown 0.5 วินาที หลังจากเล่น Animation จบ เพื่อไม่ให้รัวเกินไป
            turnCooldownTimer = 15f; 
            
            // สั่งให้กลับไปเล่นท่า Idle ระหว่างรอ 0.5 วินาที
            //fsm.bossAnimator.GetComponent<Animator>().SetTrigger("Idle");
        }

        public void OnAttackAnimationEnd() { }
        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }
    }
}