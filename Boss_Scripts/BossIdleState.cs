using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossIdleState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;

        public BossIdleState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

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

            // 2. เช็คระยะเข้าใกล้ (ถ้าผู้เล่นเดินเข้ามาหาเองจนใกล้มาก ให้เริ่มคอมโบ Melee)
            if (distance <= fsm.meleeTriggerDistance)
            {
                FSM.NextState = new BossMeleeSequenceState(fsm);
                StateStage = StateEvent.EXIT;
                return;
            }

            // --- เอาเงื่อนไข Chase (เดินตาม) ออกไปแล้ว ---
            // ทำให้บอสจะยืนรออยู่ที่เดิม และใช้แค่การวาร์ปในการเคลื่อนที่เท่านั้น

            // 3. เช็คการหันมองผู้เล่น (ถ้าผู้เล่นเดินวนรอบตัวเกิน 15 องศา จะหมุนตัวตาม)
            Vector3 dirToPlayer = fsm.playerTransform.position - fsm.transform.position;
            dirToPlayer.y = 0;
            
            if (dirToPlayer != Vector3.zero)
            {
                float angleToPlayer = Vector3.SignedAngle(fsm.transform.forward, dirToPlayer, Vector3.up);
                float absAngle = Mathf.Abs(angleToPlayer);

                if (absAngle > 15f)
                {
                    // ส่งต่อไปให้ BossStareState จัดการอนิเมชันการหมุนตัว
                    FSM.NextState = new BossStareState(fsm);
                    StateStage = StateEvent.EXIT;
                    return;
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        // --- Interface Methods ที่ไม่ได้ใช้แต่ต้องมีไว้ ---
        public void OnActionSequenceEnd() { }
        public void OnAttackAnimationEnd() { }
        public void OnDeathAnimationEnd() { }
        public void OnRoarAnimationEnd() { }
    }
}