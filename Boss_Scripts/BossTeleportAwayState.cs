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

        private float fadeTimer = 0f;
        private float fadeDuration = 0.3f; // ใช้เวลา 0.3 วินาทีในการหายตัว/ปรากฏตัว
        private int step = 0; // 0 = กำลังหายตัว, 1 = กำลังปรากฏตัว, 2 = รอกลับสู่ Idle

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
            fadeTimer = 0f;
            step = 0;

            fsm.PlaySound(fsm.teleportSounds);
        }

        public override void Update()
        {
            if (step == 0) // เฟสค่อยๆ หายตัว
            {
                fadeTimer += Time.deltaTime;
                float dissolveValue = Mathf.Clamp01(fadeTimer / fadeDuration);
                fsm.SetDissolveAmount(dissolveValue);

                if (fadeTimer >= fadeDuration)
                {
                    fsm.SetDissolveAmount(1f); // หายตัวสมบูรณ์ 100%
                    
                    // ทำการวาร์ปและเซ็ตแอนิเมชันตอนที่บอสมองไม่เห็นแล้ว
                    TeleportAway();
                    Animator anim = fsm.bossAnimator.GetComponent<Animator>();
                    if (anim != null) anim.SetTrigger("Idle");

                    step = 1;
                    fadeTimer = 0f;
                }
            }
            else if (step == 1) // เฟสค่อยๆ ปรากฏตัวในจุดใหม่
            {
                fadeTimer += Time.deltaTime;
                float dissolveValue = 1f - Mathf.Clamp01(fadeTimer / fadeDuration);
                fsm.SetDissolveAmount(dissolveValue);

                if (fadeTimer >= fadeDuration)
                {
                    fsm.SetDissolveAmount(0f); // ปรากฏตัวสมบูรณ์ 100%
                    step = 2;
                }
            }
            else if (step == 2) // เฟสเดิมของคุณ (รอ delay)
            {
                delayTimer += Time.deltaTime;
            
                // รอ 0.5 วินาทีหลังวาร์ป ก่อนที่จะให้ FSM ตัดสินใจกลับเข้าสู่ลูปปกติ
                if (delayTimer >= 0.5f)
                {
                    fsm.NextState = new BossIdleState(fsm);
                    StateStage = StateEvent.EXIT;
                }
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