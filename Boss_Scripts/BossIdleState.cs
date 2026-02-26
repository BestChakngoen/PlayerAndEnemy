using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossIdleState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;

        public BossIdleState(FiniteStateMachine fsm) : base(fsm) => this.fsm = (BossFSM)fsm;

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();              
            fsm.bossAnimator.SetSpeed(0f);
        }

        public override void Update()
        {
            if (fsm.playerTransform == null) return;

            float distance = Vector3.Distance(fsm.transform.position, fsm.playerTransform.position);

            if (distance <= fsm.meleeTriggerDistance)
            {
                StateStage = StateEvent.EXIT;
                return;
            }

            Vector3 dirToPlayer = fsm.playerTransform.position - fsm.transform.position;
            dirToPlayer.y = 0;
            
            if (dirToPlayer != Vector3.zero)
            {
                float angleToPlayer = Vector3.SignedAngle(fsm.transform.forward, dirToPlayer, Vector3.up);
                if (Mathf.Abs(angleToPlayer) > 15f)
                {
                    FSM.NextState = new BossStareState(fsm);
                    StateStage = StateEvent.EXIT;
                }
            }
        }

        public void OnActionSequenceEnd() { }
        public void OnAttackAnimationEnd() { }
        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }
    }
}