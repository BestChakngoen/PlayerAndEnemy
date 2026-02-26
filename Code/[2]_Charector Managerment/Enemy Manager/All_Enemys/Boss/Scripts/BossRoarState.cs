using UnityEngine;
using System.Collections;
using BasicEnemy; 

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossRoarState : State, BossFSM.IAnimationEventHandler
    {
        private BossFSM fsm;
        private Coroutine roarCoroutine;
        
        private bool isRoarAnimationFinished = false;

        public BossRoarState(FiniteStateMachine fsm) : base(fsm) 
        {
            this.fsm = (BossFSM)fsm;
        }

        public override void Enter()
        {
            base.Enter();
            isRoarAnimationFinished = false; 
            fsm.StopMovement();
            
            fsm.bossAnimator.TriggerRoar();
            fsm.skillController.roarSkill.TriggerCooldown();
            
            if (fsm.skillController.roarSkill.roarVFX != null)
            {
                Object.Instantiate(fsm.skillController.roarSkill.roarVFX, fsm.transform.position, fsm.transform.rotation);
            }
            //if (fsm.audioManager != null && fsm.bossSkills.roarSFX != null)
            roarCoroutine = fsm.StartCoroutine(RoarLogic());
        }

        private IEnumerator RoarLogic()
        {
            Transform player = fsm.GetPlayerTransform();
            if (player == null) 
            {
                FSM.NextState = new BossIdleState(fsm);
                StateStage = StateEvent.EXIT;
                yield break;
            }
            
            for (int i = 0; i < 3; i++)
            {
                if (i == 0) yield return new WaitForSeconds(0.2f); 
                
                fsm.skillController.roarSkill.PerformHit(player);
                yield return new WaitForSeconds(fsm.skillController.roarSkill.roarHitInterval);
            }
            
            fsm.skillController.roarSkill.ApplyDebuff(player);
            
            yield return new WaitUntil(() => isRoarAnimationFinished);
            
            FSM.NextState = new BossIdleState(fsm);
            StateStage = StateEvent.EXIT;
        }

        public override void Update()
        {
            fsm.StopMovement(); 
        }

        public override void Exit()
        {
            base.Exit();
            if (roarCoroutine != null) fsm.StopCoroutine(roarCoroutine);
        }
        public void OnRoarAnimationEnd()
        {
            isRoarAnimationFinished = true;
        }
        public void OnAttackAnimationEnd() { }
        public void OnDeathAnimationEnd() { }
        
        public void OnActionSequenceEnd() { }
    }
}