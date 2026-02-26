using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossAttackState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;

        public BossAttackState(FiniteStateMachine fsm) : base(fsm) 
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            
            // แก้ไขจาก fsm.RotateToPlayer() เป็น LookAtPlayerImmediate()
            fsm.LookAtPlayerImmediate(); 
            fsm.bossAnimator.TriggerAttack();
        }

        public override void Update()
        {
            float distance = Vector3.Distance(fsm.transform.position, fsm.playerTransform.position);
            
        }

        public override void Exit()
        {
            base.Exit();
        }
        
        public void OnAttackAnimationEnd()
        {
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }

        // แก้ไข: เพิ่ม Interface ที่ขาดหายไป
        public void OnActionSequenceEnd() { }
    }
}