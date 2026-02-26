/*using UnityEngine;
using GameManger;

namespace PlayerInputs
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator animator;

        [Header("Dependencies")] public PlayerInputController controller;

        void Awake()
        {
            animator = GetComponent<Animator>();

            if (controller == null)
            {
                controller = GetComponent<PlayerInputController>();
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

        public void SetComboState(bool isCombo)
        {
            animator.SetBool("IsCombo", isCombo);
        }

        public void SetAttackCombo(int combo)
        {
            animator.SetInteger("AttackCombo", combo);
        }

        public void TriggerRoll()
        {
            animator.SetTrigger("Roll");
        }

        public void TriggerCastSkill()
        {
            animator.SetTrigger("Cast");
        }

        public void TriggerAttack(int comboCounter)
        {
            animator.SetInteger("AttackCombo", comboCounter);
            animator.SetTrigger("Attack");
        }

        public void TriggerDie()
        {
            animator.SetTrigger("Die");
        }

        public void OnDeathAnimationEnd()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.GameOver)
            {
                GameStateManager.Instance.SetState(GameState.GameOver);
            }
        }
        public bool IsInNonCombatState()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Idle") || stateInfo.IsName("Walk") ||stateInfo.IsName("Roll");
        }
        public void AnimationEvent_OnComboWindowOpen()
        {
            controller.AnimationEvent_OnComboWindowOpen();
        }
        public void AnimationEvent_OnAttackEnd()
        {
            controller.OnAttackAnimationEnd();
        }

        public void AnimationEvent_EnableWeapon()
        {
            controller.EnableWeaponCollider();
        }

        public void AnimationEvent_DisableWeapon()
        {
            controller.DisableWeaponCollider();
        }
        public void AnimationEvent_EnableIFrame()
        {
            controller.AnimationEvent_EnableIFrame();
        }
        
        public void AnimationEvent_DisableIFrame()
        {
            controller.AnimationEvent_DisableIFrame();
        }

    }
}*/