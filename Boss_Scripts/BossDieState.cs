using UnityEngine;
using BasicEnemy;
using GameSystem;

namespace Boss.scripts
{
    public class BossDieState : State
    {
        private BossFSM fsm;
        private float actionTimer;
        private bool isDeadProcessed;

        public BossDieState(FiniteStateMachine fsm) : base(fsm)
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            actionTimer = 0f;
            isDeadProcessed = false;

            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            if (animator != null)
            {
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    if (param.type == AnimatorControllerParameterType.Trigger)
                    {
                        animator.ResetTrigger(param.name);
                    }
                    else if (param.type == AnimatorControllerParameterType.Bool)
                    {
                        animator.SetBool(param.name, false);
                    }
                }
                animator.SetFloat("Speed", 0f);
            }

            fsm.bossAnimator.TriggerDie();
        }

        public override void Update()
        {
            if (isDeadProcessed) return;

            Animator animator = fsm.bossAnimator.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.IsInTransition(0) 
                ? animator.GetNextAnimatorStateInfo(0) 
                : animator.GetCurrentAnimatorStateInfo(0);
            
            float currentAnimLength = stateInfo.length > 0 ? stateInfo.length : 3f;

            actionTimer += Time.deltaTime;
            
            if (actionTimer >= currentAnimLength + 0.2f)
            {
                isDeadProcessed = true;
                UIManager.IsWin = true;
                
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.SetState(GameState.GameOver);
                }
                
                Object.Destroy(fsm.gameObject);
            }
        }
    }
}