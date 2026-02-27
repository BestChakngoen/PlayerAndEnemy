using BasicEnemy;
using UnityEngine;
using Boss.core;

namespace Boss.scripts
{
    public class BossWalkBackState : State
    {
        private BossFSM fsm;
        private float actionTimer;
        private bool isGoingLeft;
        private float speed = 1f;
        
        private int currentDodgeCount = 0;
        private int maxDodgeCount = 4;

        public BossWalkBackState(BossFSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }

        public override void Enter()
        {
            base.Enter();
            currentDodgeCount = 0;
            isGoingLeft = Random.value > 0.5f;
            TriggerDodgeAction();
        }

        private void TriggerDodgeAction()
        {
            actionTimer = 0f;
            fsm.bossAnimator.TriggerGetAway(isGoingLeft);
        }

        public override void Update()
        {
            if (fsm.playerTransform != null)
            {
                float distance = Vector3.Distance(fsm.BossTransform.position, fsm.playerTransform.position);
                if (distance <= fsm.meleeTriggerDistance && fsm.meleeAttackTimer <= 0f)
                {
                    fsm.NextState = new BossMeleeAttackState(fsm);
                    StateStage = StateEvent.EXIT;
                    return;
                }
            }

            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
                float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;

                fsm.RotateToPlayerSmoothly(5f);

                Vector3 backDirection = -fsm.BossTransform.forward;
                Vector3 sideDirection = isGoingLeft ? -fsm.BossTransform.right : fsm.BossTransform.right;
                
                Vector3 moveDirection = (backDirection + sideDirection * 0.5f).normalized;
                
                fsm.BossTransform.position += moveDirection * (speed * fsm.baseSpeedMultiplier) * Time.deltaTime;

                actionTimer += Time.deltaTime;
                if (actionTimer >= currentAnimLength + 0.1f)
                {
                    OnDodgeSequenceEnd();
                }
            }
        }

        private void OnDodgeSequenceEnd()
        {
            currentDodgeCount++;
            
            if (currentDodgeCount < maxDodgeCount)
            {
                isGoingLeft = !isGoingLeft;
                TriggerDodgeAction();
            }
            else
            {
                fsm.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
            }
        }
    }
}