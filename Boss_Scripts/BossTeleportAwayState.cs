using BasicEnemy;
using UnityEngine;
using Boss.core;

namespace Boss.scripts
{
    public class BossTeleportAwayState : State
    {
        private BossFSM fsm;
        private float minDistance;
        private float maxDistance;
        private float delayTimer = 0f;

        public BossTeleportAwayState(BossFSM fsm, float min, float max) : base(fsm)
        {
            this.fsm = fsm;
            this.minDistance = min;
            this.maxDistance = max;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            delayTimer = 0f;

            TeleportAway();

            // รีเซ็ตแอนิเมชันให้กลับมายืนตั้งหลัก
            Animator anim = fsm.bossAnimator.GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("Idle");
        }

        public override void Update()
        {
            delayTimer += Time.deltaTime;
            
            // รอ 0.5 วินาทีหลังวาร์ป ก่อนที่จะให้ FSM ตัดสินใจกลับเข้าสู่ลูปปกติ
            if (delayTimer >= 0.5f)
            {
                fsm.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
            }
        }

        private void TeleportAway()
        {
            if (fsm.playerTransform != null)
            {
                Vector3 playerPos = fsm.playerTransform.position;
                
                // สุ่มทิศทางแบบ 360 องศารอบตัว Player
                Vector2 randomCircle = Random.insideUnitCircle.normalized;
                Vector3 randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
                
                // สุ่มระยะทาง
                float randomDistance = Random.Range(minDistance, maxDistance);
                
                Vector3 targetPosition = playerPos + (randomDirection * randomDistance);
                targetPosition.y = fsm.BossTransform.position.y;
                
                fsm.BossTransform.position = targetPosition;
                
                // หันหน้ากลับมามองผู้เล่นทันที
                fsm.LookAtPlayerImmediate();
            }
        }
    }
}