using UnityEngine;
using GameManger;

namespace BasicEnemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        private Animator animator;

        [Header("Dependencies")] 
        public BasicEnemyAI aiController;

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (aiController == null)
            {
                aiController = GetComponent<BasicEnemyAI>();
            }

            if (animator == null)
            {
                Debug.LogError("Animator component not found!");
            }
        }

        public void SetMovementSpeed(float speed)
        {
            animator.SetFloat("Speed", speed);
        }
        public void SetChargeSpeed(float chargeSpeed)
        {
            animator.SetFloat("ChargeSpeed", chargeSpeed);
        }

        public void TriggerAttack()
        {
            animator.SetTrigger("Attack");
        }

        public void TriggerDie()
        {
            animator.SetTrigger("Die");
        }

        public void TriggerStun()
        {
            animator.SetTrigger("Stun");
        }

        public void AnimationEvent_OnDeathEnd()
        {
            aiController.OnDeathAnimationEnd();
        }

        public void AnimationEvent_OnAttackEnd()
        {
            aiController.OnAttackAnimationEnd();
        }

        public void AnimationEvent_EnableWeapon()
        {
            aiController.EnableWeaponCollider();
        }

        public void AnimationEvent_DisableWeapon()
        {
            aiController.DisableWeaponCollider();
        }
    }
}
