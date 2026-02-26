using UnityEngine;
using BasicEnemy;
using GameManger;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossDieState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;

        public BossDieState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            fsm.bossAnimator.TriggerDie();
        }

        public override void Update() { }

        public void OnAttackAnimationEnd() { }

        public void OnDeathAnimationEnd()
        {
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.MonsterDied();
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.AddKill();
            }

            Object.Destroy(fsm.gameObject);
        }

        public void OnRoarAnimationEnd() { }
        public void OnActionSequenceEnd() { }
    }
}