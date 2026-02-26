using UnityEngine;
using System.Collections;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossMeleeSequenceState : State
    {
        private BossFSM fsm;
        private Coroutine sequenceCoroutine;

        public BossMeleeSequenceState(FiniteStateMachine fsm) : base(fsm) => this.fsm = (BossFSM)fsm;

        public override void Enter()
        {
            base.Enter();
            fsm.StopMovement();
            fsm.LookAtPlayerImmediate(); 
            sequenceCoroutine = fsm.StartCoroutine(ExecuteMeleeSequence());
        }

        private IEnumerator ExecuteMeleeSequence()
        {
            fsm.bossAnimator.TriggerMutantPunch();
            yield return new WaitForSeconds(1.2f); 

            bool goLeft = Random.value > 0.5f;
            fsm.bossAnimator.TriggerGetAway(goLeft);
            yield return new WaitForSeconds(1.0f);

            fsm.bossAnimator.TriggerScream();
            yield return new WaitForSeconds(1.5f);

            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public override void Update() { }

        public override void Exit()
        {
            if (sequenceCoroutine != null) fsm.StopCoroutine(sequenceCoroutine);
            base.Exit();
        }
    }
}