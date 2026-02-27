using UnityEngine;

namespace Boss.scripts
{
    public class BossAnimator : MonoBehaviour
    {
        private Animator animator;
        public BossFSM aiController;

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (aiController == null) aiController = GetComponent<BossFSM>();
        }

        public void SetSpeed(float speed) => animator.SetFloat("Speed", speed);
        public void TriggerAttack() => animator.SetTrigger("Attack");
        public void TriggerDie() => animator.SetTrigger("Die");
        public void TriggerMutantPunch() => animator.SetTrigger("MutantPunch");
        public void TriggerScream() => animator.SetTrigger("Scream");
        public void TriggerSwiping() => animator.SetTrigger("Swiping");
        
        public void TriggerGetAway(bool goLeft) 
        {
            if(goLeft) animator.SetTrigger("GetAwayLeft");
            else animator.SetTrigger("GetAwayRight");
        }
        
        
    }
}