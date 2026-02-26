using UnityEngine;
using System.Collections;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossTeleportSwipeState : State
    {
        private BossFSM fsm;
        private Coroutine sequenceCoroutine;

        public BossTeleportSwipeState(FiniteStateMachine fsm) : base(fsm) 
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            sequenceCoroutine = fsm.StartCoroutine(ExecuteTeleportSequence());
        }

        private IEnumerator ExecuteTeleportSequence()
        {
            float warningDelay = Random.Range(0.1f, 0.5f);
            yield return new WaitForSeconds(warningDelay);

            fsm.bossAnimator.TriggerSwiping();
            yield return new WaitForSeconds(1.5f);
            
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public override void Update() { }

        public override void Exit()
        {
            if (sequenceCoroutine != null)
            {
                fsm.StopCoroutine(sequenceCoroutine);
            }
            base.Exit();
        }
    }
}