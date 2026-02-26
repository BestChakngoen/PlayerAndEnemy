using UnityEngine;
using BasicEnemy.Enemy.Core;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossScreamState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;

        public BossScreamState(BossFSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }

        public override void Enter()
        {
            fsm.StopMovement();
            Animator anim = fsm.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetFloat("Speed", 0f);
                anim.SetTrigger("Scream");
            }
        }

        public override void Update() { }

        public void OnRoarAnimationEnd()
        {
            if (StateStage != StateEvent.UPDATE) return;

            fsm.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public void OnAttackAnimationEnd() { }
        public void OnDeathAnimationEnd() { }
        public void OnActionSequenceEnd() { }
    }
}