using UnityEngine;
using BasicEnemy.Enemy.Core;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossMeleeAttackState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;
        private float safetyTimer;
        private float maxDuration = 2.0f;

        public BossMeleeAttackState(BossFSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }

        public override void Enter()
        {
            safetyTimer = maxDuration;
            fsm.StopMovement();
            fsm.LookAtPlayerImmediate();
            
            Animator anim = fsm.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Attack"); 
            }
        }

        public override void Update()
        {
            safetyTimer -= Time.deltaTime;
            if (safetyTimer <= 0)
            {
                OnAttackAnimationEnd();
            }
        }

        public void OnAttackAnimationEnd()
        {
            if (StateStage != StateEvent.UPDATE) return;

            Animator anim = fsm.GetComponent<Animator>();
            if (anim != null) anim.ResetTrigger("MutantPunch");

            fsm.NextState = new BossWalkBackState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public void OnRoarAnimationEnd() { }
        public void OnDeathAnimationEnd() { }
        public void OnActionSequenceEnd() { }
    }
}