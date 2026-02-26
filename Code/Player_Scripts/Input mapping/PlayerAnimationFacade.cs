using UnityEngine;
using System;
using GameManger;

namespace PlayerInputs
{
    public class PlayerAnimationFacade : MonoBehaviour
    {
        private Animator animator;

        // =======================
        // Events (แทนการเรียก Controller ตรง ๆ)
        // =======================
        public enum AnimationType
        {
            Locomotion,
            Action
        }
        private AnimationType currentAnimationType = AnimationType.Locomotion;
        public event Action OnCanMove;
        public event Action OnAttackEnd;
        public event Action OnEnableWeapon;
        public event Action OnDisableWeapon;
        public event Action OnEnableIFrame;
        public event Action OnDisableIFrame;

        // =======================
        // Animator Hash
        // =======================
        private static readonly int SpeedHash        = Animator.StringToHash("Speed");
        private static readonly int IsComboHash      = Animator.StringToHash("IsCombo");
        private static readonly int AttackComboHash  = Animator.StringToHash("AttackCombo");
        private static readonly int AttackTrigger    = Animator.StringToHash("Attack");
        private static readonly int RollTrigger      = Animator.StringToHash("Roll");
        private static readonly int CastTrigger      = Animator.StringToHash("Cast");
        private static readonly int DieTrigger       = Animator.StringToHash("Die");

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogError("Animator component not found!");
            
            animator.applyRootMotion = false;
            currentAnimationType = AnimationType.Locomotion;
        }
        // =======================
        // Public API (Facade)
        // =======================

        public void SetMovementSpeed(float speed)
        {
            if (currentAnimationType == AnimationType.Action)
                return;
            animator.applyRootMotion = false;
            animator.SetFloat(SpeedHash, speed);
        }

        public void SetComboState(bool isCombo)
        {
            animator.SetBool(IsComboHash, isCombo);
        }

        public void SetAttackCombo(int combo)
        {
            animator.SetInteger(AttackComboHash, combo);
        }

        // =======================
        // Action (Use Root Motion)
        // =======================
        private void EnterActionState()
        {
            currentAnimationType = AnimationType.Action;
            animator.applyRootMotion = true;
        }

        private void ExitActionState()
        {
            currentAnimationType = AnimationType.Locomotion;
            animator.applyRootMotion = false;
        }
        public void PlayAttack(int comboCounter)
        {
            EnterActionState();
            animator.SetInteger(AttackComboHash, comboCounter);
            animator.SetTrigger(AttackTrigger);
        }

        public void PlayRoll()
        {
            EnterActionState();
            animator.SetTrigger(RollTrigger);
        }

        public void PlayCastSkill()
        {
            EnterActionState();
            animator.SetTrigger(CastTrigger);
        }

        public void PlayDie()
        {
            EnterActionState();
            animator.SetTrigger(DieTrigger);
        }

        public void EnableAnimator(bool enable)
        {
            animator.enabled = enable;
        }

        // =======================
        // State Query
        // =======================

        public bool IsInNonCombatState()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Idle")
                || stateInfo.IsName("Walk")
                || stateInfo.IsName("Roll");
        }
        public bool IsInLocomotion()
        {
            return currentAnimationType == AnimationType.Locomotion;
        }

        public bool IsInAction()
        {
            return currentAnimationType == AnimationType.Action;
        }

        // =======================
        // Animation Events
        // (ผูกใน Animation Clip)
        // =======================

        public void AnimEvent_AttackEnd()
        {
            ExitActionState();
            OnAttackEnd?.Invoke();
        }

        public void AnimEvent_CanMove()
        {
            OnCanMove?.Invoke();
        }

        public void AnimEvent_EnableWeapon()
        {
            OnEnableWeapon?.Invoke();
        }

        public void AnimEvent_DisableWeapon()
        {
            OnDisableWeapon?.Invoke();
        }

        public void AnimEvent_EnableIFrame()
        {
            OnEnableIFrame?.Invoke();
        }

        public void AnimEvent_DisableIFrame()
        {
            OnDisableIFrame?.Invoke();
            ExitActionState();
        }

        // =======================
        // Death Animation Event
        // =======================

        public void AnimEvent_DeathAnimationEnd()
        {
            if (GameStateManager.Instance != null &&
                GameStateManager.Instance.CurrentState != GameState.GameOver)
            {
                GameStateManager.Instance.SetState(GameState.GameOver);
            }
        }
        
        // =======================
        // Utility
        // =======================
        public Animator GetAnimator()
        {
            return animator;
        }
        public void SetLayerWeight(string layerName, float weight)
        {
            int index = animator.GetLayerIndex(layerName);
            if (index >= 0)
                animator.SetLayerWeight(index, weight);
        }


    }
}
