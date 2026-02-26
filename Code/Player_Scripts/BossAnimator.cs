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
        public void SetSpeed(float speed)
        {
            animator.SetFloat("Speed", speed);
        }
    
        public void SetRunning(bool isRunning)
        {
            animator.SetBool("IsRunning", isRunning);
        }

        public void TriggerAttack()
        {
            animator.SetTrigger("Attack");
        }

        public void TriggerJumpAttack()
        {
            animator.SetTrigger("JumpAttack");
        }
        public void TriggerDie()
        {
            animator.SetTrigger("Die");
        }
        public void AnimationEvent_OnDeathEnd()
        {
            aiController.OnDeathAnimationEnd();
        }
        
        public void EnableWeaponCollider()
        {
            if (enemyWeapon != null)
            {
                enemyWeapon.EnableCollider();
            }
        }

        public void DisableWeaponCollider()
        {
            if (enemyWeapon != null)
            {
                enemyWeapon.DisableCollider();
            }
        }
        
        public void OnJumpAttackAnimationStart()
        {
            if (GetComponent<BossSkills>() != null)
            {
                GetComponent<BossSkills>().OnJumpAttackAnimationStart();
            }
        }
    
        public void OnJumpAttackAnimationEnd()
        {
            if (GetComponent<BossSkills>() != null)
            {
                GetComponent<BossSkills>().OnJumpAttackAnimationEnd();
            }
            if (aiController != null)
            {
                aiController.OnAttackAnimationEnd(); 
            }
        }
        public void TriggerRoar()
        {
            animator.SetTrigger("Roar");
        }
        public void AnimationEvent_OnRoarEnd()
        {
            aiController.OnRoarAnimationEnd();
        }
    }
}