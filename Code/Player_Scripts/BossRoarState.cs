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
            fsm.bossSkills.TriggerRoarSequence(fsm.GetPlayerTransform());
            
            if (fsm.bossSkills.roarVFX != null)
            {
                Object.Instantiate(fsm.bossSkills.roarVFX, fsm.transform.position, fsm.transform.rotation);
            }
            //if (fsm.audioManager != null && fsm.bossSkills.roarSFX != null)
            {
                //fsm.audioManager.PlaySFX(fsm.bossSkills.roarSFX);
            }
            
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
                
                fsm.bossSkills.PerformRoarHit(player);
                yield return new WaitForSeconds(fsm.bossSkills.roarHitInterval);
            }
            
            BuffManager playerBuffManager = player.GetComponent<BuffManager>();
            if (playerBuffManager != null && fsm.bossSkills.roarDebuffData != null)
            {
                playerBuffManager.AddBuff(fsm.bossSkills.roarDebuffData.CreateBuff());
            }
            
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
    }
}