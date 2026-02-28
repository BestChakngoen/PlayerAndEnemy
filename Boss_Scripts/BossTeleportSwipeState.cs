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

        public override void Update()
        {
            actionTimer += Time.deltaTime;

            if (!hasSwiped && actionTimer >= warningDelay)
            {
                hasSwiped = true;
                actionTimer = 0f; 
                fsm.bossAnimator.TriggerSwiping();
                return;
            }

            if (hasSwiped)
            {
                Animator animator = fsm.bossAnimator.GetComponent<Animator>();
                
                if (animator != null)
                {
                    AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
                    float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;

                    if (actionTimer >= currentAnimLength + 0.1f)
                    {
                        TeleportAway();
                        fsm.NextState = new BossIdleState(fsm);
                        StateStage = StateEvent.EXIT;
                    }
                }
                else
                {
                    if (actionTimer >= 2.0f)
                    {
                        TeleportAway();
                        fsm.NextState = new BossIdleState(fsm);
                        StateStage = StateEvent.EXIT;
                    }
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