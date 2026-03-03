using UnityEngine;
using BasicEnemy;

namespace Boss.scripts
{
    public class BossTeleportSwipeState : State
    {
        private BossFSM fsm;
        private float teleportBehindOffset = 1.5f;
        private float minTeleportAwayOffset;
        private float maxTeleportAwayOffset;
        private float actionTimer = 0f;
        private bool hasSwiped = false;
        private float warningDelay;

        // ตัวแปรเสริมสำหรับการจัดการเฟสหายตัว
        private int step = 0;
        private float fadeTimer = 0f;
        private float fadeDuration = 0.3f; // ความเร็วในการละลายตัว

        public BossTeleportSwipeState(BossFSM fsm, float minAwayDistance, float maxAwayDistance) : base(fsm)
        {
            this.fsm = fsm;
            this.minTeleportAwayOffset = minAwayDistance;
            this.maxTeleportAwayOffset = maxAwayDistance;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            actionTimer = 0f;
            hasSwiped = false;
            warningDelay = Random.Range(0.1f, 0.5f);

            step = 0;
            fadeTimer = 0f;
            fsm.PlaySound(fsm.teleportSounds);
        }

        public override void Update()
        {
            if (step == 0) // 1. หายตัวก่อนวาร์ปไปหลังผู้เล่น
            {
                fadeTimer += Time.deltaTime;
                fsm.SetDissolveAmount(Mathf.Clamp01(fadeTimer / fadeDuration));
                
                if (fadeTimer >= fadeDuration)
                {
                    fsm.SetDissolveAmount(1f);
                    TeleportBehindPlayer(); // โค้ดเดิมของคุณที่ย้ายบอสไปหลังผู้เล่น
                    step = 1;
                    fadeTimer = 0f;
                }
            }
            else if (step == 1) // 2. โผล่มาด้านหลังผู้เล่น
            {
                fadeTimer += Time.deltaTime;
                fsm.SetDissolveAmount(1f - Mathf.Clamp01(fadeTimer / fadeDuration));
                
                if (fadeTimer >= fadeDuration)
                {
                    fsm.SetDissolveAmount(0f);
                    step = 2; // ไปรอเข้าเงื่อนไขตบ
                }
            }
            else if (step == 2) // 3. โค้ดเดิมของคุณ (รอดีเลย์และโจมตี)
            {
                actionTimer += Time.deltaTime;

                if (!hasSwiped && actionTimer >= warningDelay)
                {
                    hasSwiped = true;
                    actionTimer = 0f; 
                    fsm.PlaySound(fsm.teleportSwipeSounds);
                    fsm.bossAnimator.TriggerSwiping();
                }

                if (hasSwiped)
                {
                    Animator animator = fsm.bossAnimator.GetComponent<Animator>();
                    float currentAnimLength = 2.0f; // สำรองกรณีไม่มีอนิเมเตอร์
                    
                    if (animator != null)
                    {
                        AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
                        currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;
                    }

                    if (actionTimer >= currentAnimLength + 0.1f)
                    {
                        // โจมตีเสร็จแล้ว เข้าสู่เฟสหายตัวหนี
                        step = 3;
                        fadeTimer = 0f;
                        fsm.PlaySound(fsm.teleportSounds);
                    }
                }
            }
            else if (step == 3) // 4. หายตัวหลังจากโจมตีเสร็จ
            {
                fadeTimer += Time.deltaTime;
                fsm.SetDissolveAmount(Mathf.Clamp01(fadeTimer / fadeDuration));
                
                if (fadeTimer >= fadeDuration)
                {
                    fsm.SetDissolveAmount(1f);
                    TeleportAway(); // โค้ดเดิมของคุณที่สุ่มวาร์ปหนี
                    step = 4;
                    fadeTimer = 0f;
                }
            }
            else if (step == 4) // 5. ปรากฏตัวในจุดใหม่
            {
                fadeTimer += Time.deltaTime;
                fsm.SetDissolveAmount(1f - Mathf.Clamp01(fadeTimer / fadeDuration));
                
                if (fadeTimer >= fadeDuration)
                {
                    fsm.SetDissolveAmount(0f);
                    // ปรากฏตัวเสร็จสมบูรณ์ จบ State (ตามระบบเดิมของคุณเป๊ะๆ)
                    fsm.NextState = new BossIdleState(fsm);
                    StateStage = StateEvent.EXIT;
                }
            }
        }

        private void TeleportBehindPlayer()
        {
            if (fsm.playerTransform != null)
            {
                Vector3 playerPos = fsm.playerTransform.position;
                Vector3 playerForward = fsm.playerTransform.forward;
                
                Vector3 targetPosition = playerPos - (playerForward * teleportBehindOffset);
                targetPosition.y = fsm.BossTransform.position.y;

                fsm.BossTransform.position = targetPosition;

                Vector3 lookDirection = playerPos - fsm.BossTransform.position;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    fsm.BossTransform.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
        }

        private void TeleportAway()
        {
            if (fsm.playerTransform != null)
            {
                Vector3 playerPos = fsm.playerTransform.position;
                
                Vector2 randomCircle = Random.insideUnitCircle.normalized;
                Vector3 randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
                
                float randomDistance = Random.Range(minTeleportAwayOffset, maxTeleportAwayOffset);
                
                Vector3 targetPosition = playerPos + (randomDirection * randomDistance);
                targetPosition.y = fsm.BossTransform.position.y;
                
                fsm.BossTransform.position = targetPosition;
                fsm.LookAtPlayerImmediate();
            }
        }
    }
}