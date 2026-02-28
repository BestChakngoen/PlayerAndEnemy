using UnityEngine;
using BasicEnemy;
using GameManger;

namespace Boss.scripts
{
    public class BossDieState : State
    {
        private BossFSM fsm;
        private float actionTimer;

        public BossDieState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            actionTimer = 0f;
            fsm.bossAnimator.TriggerDie();
        }

        public override void Update()
        {
            Animator animator = fsm.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
            float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 1f;

            actionTimer += Time.deltaTime;
            if (actionTimer >= currentAnimLength)
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
        }
    }
}