using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossStareState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;
        private bool isTurning = false;
        private float turnCooldownTimer = 0f;

        public BossStareState(FiniteStateMachine fsm) : base(fsm) => this.fsm = (BossFSM)fsm;

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            fsm.bossAnimator.SetSpeed(0f);
            isTurning = false; 
            turnCooldownTimer = 0f;
        }

        public override void Update()
        {
            if (fsm.playerTransform == null) return;

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

            if (turnCooldownTimer > 0) turnCooldownTimer -= Time.deltaTime;

            if (!isTurning && turnCooldownTimer <= 0f && absAngle > 15f)
            {
                isTurning = true;
                Animator animator = fsm.bossAnimator.GetComponent<Animator>();

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

            if (isTurning) fsm.RotateToPlayerSmoothly(6f);
        }

        public void OnActionSequenceEnd()
        {
            fsm.LookAtPlayerImmediate();
            isTurning = false;
            turnCooldownTimer = 15f; 
        }

        public void OnAttackAnimationEnd() { }
        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }
    }
}