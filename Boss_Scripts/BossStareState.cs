using UnityEngine;
using BasicEnemy;

namespace Boss.scripts
{
    public class BossStareState : State
    {
        private BossFSM fsm;
        private bool isTurning = false;
        private float turnCooldownTimer = 0f;
        private float actionTimer = 0f;
        
        private Quaternion startRotation;
        private Quaternion targetRotation;

        public BossStareState(FiniteStateMachine fsm) : base(fsm) => this.fsm = (BossFSM)fsm;

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();

            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            if (animator != null) animator.SetTrigger("Idle");

            isTurning = false; 
            turnCooldownTimer = 0f;
            actionTimer = 0f;
        }

        public override void Update()
        {
            if (fsm.playerTransform == null) return;

            float distance = Vector3.Distance(fsm.transform.position, fsm.playerTransform.position);
            if (distance <= fsm.meleeTriggerDistance && fsm.meleeAttackTimer <= 0f)
            {
                FSM.NextState = new BossMeleeAttackState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
            float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;

            if (isTurning)
            {
                actionTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(actionTimer / currentAnimLength);
                fsm.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);

                if (actionTimer >= currentAnimLength)
                {
                    fsm.transform.rotation = targetRotation;
                    OnTurnSequenceEnd();
                }
                return;
            }

            Vector3 dirToPlayer = fsm.playerTransform.position - fsm.transform.position;
            dirToPlayer.y = 0;

            if (dirToPlayer == Vector3.zero) return;

            float angleToPlayer = Vector3.SignedAngle(fsm.transform.forward, dirToPlayer, Vector3.up);
            float absAngle = Mathf.Abs(angleToPlayer);

            if (!isTurning && absAngle <= 15f)
            {
                FSM.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            if (turnCooldownTimer > 0)
            {
                turnCooldownTimer -= Time.deltaTime;
            }

            if (!isTurning && turnCooldownTimer <= 0f && absAngle > 15f)
            {
                isTurning = true;
                actionTimer = 0f;
                
                startRotation = fsm.transform.rotation;
                targetRotation = Quaternion.LookRotation(dirToPlayer);

                if (angleToPlayer > 0f)
                {
                    if (absAngle > 60f) animator.SetTrigger("TurnRight90");
                    else animator.SetTrigger("TurnRight45");
                }
                else
                {
                    if (absAngle > 60f) animator.SetTrigger("TurnLeft90");
                    else animator.SetTrigger("TurnLeft45");
                }
            }
        }

        private void OnTurnSequenceEnd()
        {
            isTurning = false;
            turnCooldownTimer = 1.5f; 

            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            if (animator != null) animator.SetTrigger("Idle");
        }
    }
}