using BasicEnemy;
using UnityEngine;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossAnimator : MonoBehaviour
    {
        private Animator animator;
        public Weapon enemyWeapon; 
        
        [Header("Dependencies")] 
        public BossFSM aiController;

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (aiController == null) 
                aiController = GetComponent<BossFSM>();
        }

        public void SetSpeed(float speed) => animator.SetFloat("Speed", speed);
        public void SetRunning(bool isRunning) => animator.SetBool("IsRunning", isRunning);

        // --- Original Triggers ---
        public void TriggerAttack() => animator.SetTrigger("Attack");
        public void TriggerJumpAttack() => animator.SetTrigger("JumpAttack");
        public void TriggerRoar() => animator.SetTrigger("Roar");
        public void TriggerDie() => animator.SetTrigger("Die");

        // --- New Triggers for Flowchart Logic ---
        public void TriggerTurn45() => animator.SetTrigger("Turn45");
        public void TriggerTurn90() => animator.SetTrigger("Turn90");
        public void TriggerMutantPunch() => animator.SetTrigger("MutantPunch");
        public void TriggerGetAway(bool goLeft) 
        {
            if(goLeft) animator.SetTrigger("GetAwayLeft");
            else animator.SetTrigger("GetAwayRight");
        }
        public void TriggerScream() => animator.SetTrigger("Scream");
        public void TriggerSwiping() => animator.SetTrigger("Swiping");

        // --- Animation Events ---
        public void AnimationEvent_OnDeathEnd() => aiController.OnDeathAnimationEnd();
        public void AnimationEvent_OnRoarEnd() => aiController.OnRoarAnimationEnd();
        
        // Event พิเศษสำหรับบอกว่า Sequence จบแล้ว (เช่น จบ Scream, จบ Swiping, จบ Turn)
        public void AnimationEvent_OnActionEnd() 
        {
            aiController.OnActionSequenceEnd();
        }

        public void EnableWeaponCollider()
        {
            if (enemyWeapon != null) enemyWeapon.EnableCollider();
        }

        public void DisableWeaponCollider()
        {
            if (enemyWeapon != null) enemyWeapon.DisableCollider();
        }
        
        public void OnJumpAttackAnimationStart()
        {
            if (GetComponent<BossSkills>() != null) GetComponent<BossSkills>().OnJumpAttackAnimationStart();
        }
    
        public void OnJumpAttackAnimationEnd()
        {
            if (GetComponent<BossSkills>() != null) GetComponent<BossSkills>().OnJumpAttackAnimationEnd();
            if (aiController != null) aiController.OnAttackAnimationEnd(); 
        }
    }
}