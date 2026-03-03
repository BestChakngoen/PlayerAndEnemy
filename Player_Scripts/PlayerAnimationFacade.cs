using UnityEngine;
using System;
using GameSystem;

namespace PlayerInputs
{
    public class PlayerAnimationFacade : MonoBehaviour
    {
        public enum AnimationType
        {
            Locomotion,
            Action
        }

        private Animator animator;
        private AnimationType currentAnimationType = AnimationType.Locomotion;

        private bool isDead = false;
        public bool IsStunned { get; private set; }

        public event Action OnCanMove;
        public event Action OnCanNotMove;
        public event Action OnAttackEnd;
        public event Action OnRollStart;
        public event Action OnRollEnd;
        public event Action OnEnableWeapon;
        public event Action OnDisableWeapon;
        public event Action OnEnableIFrame;
        public event Action OnDisableIFrame;
        public event Action OnDealDamage;
        public event Action OnHitEnd;
        public event Action<bool> OnStunStateChanged;

        private static readonly int SpeedHash        = Animator.StringToHash("Speed");
        private static readonly int IsComboHash      = Animator.StringToHash("IsCombo");
        private static readonly int AttackComboHash  = Animator.StringToHash("AttackCombo");
        private static readonly int IsStunnedHash    = Animator.StringToHash("IsStunned");
        private static readonly int AttackTrigger    = Animator.StringToHash("Attack");
        private static readonly int RollTrigger      = Animator.StringToHash("Roll");
        private static readonly int CastTrigger      = Animator.StringToHash("Cast");
        private static readonly int DieTrigger       = Animator.StringToHash("Die");
        private static readonly int HitTrigger       = Animator.StringToHash("Hit");

        void Awake()
        {
            animator = GetComponent<Animator>();
            animator.applyRootMotion = false;
            currentAnimationType = AnimationType.Locomotion;
        }

        public void SetMovementSpeed(float speed)
        {
            if (currentAnimationType == AnimationType.Action || isDead) return;
            animator.applyRootMotion = false;
            animator.SetFloat(SpeedHash, speed);
        }

        public void SetComboState(bool isCombo)
        {
            if (isDead) return;
            animator.SetBool(IsComboHash, isCombo);
        }

        public void SetAttackCombo(int combo)
        {
            if (isDead) return;
            animator.SetInteger(AttackComboHash, combo);
        }

        public void SetStunState(bool isStunned)
        {
            if (isDead) return;

            IsStunned = isStunned;
            animator.SetBool(IsStunnedHash, isStunned);
            
            if (isStunned)
            {
                currentAnimationType = AnimationType.Action;
                animator.SetFloat(SpeedHash, 0f);

                animator.ResetTrigger(AttackTrigger);
                animator.ResetTrigger(RollTrigger);
                animator.ResetTrigger(CastTrigger);
            }
            else
            {
                currentAnimationType = AnimationType.Locomotion;
            }

            OnStunStateChanged?.Invoke(isStunned);
        }

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
            if (isDead) return;
            EnterActionState();
            animator.SetInteger(AttackComboHash, comboCounter);
            animator.SetTrigger(AttackTrigger);
        }

        public void PlayRoll()
        {
            if (isDead) return;
            animator.ResetTrigger(AttackTrigger);
            EnterActionState();
            animator.applyRootMotion = false;
            animator.SetTrigger(RollTrigger);
            OnRollStart?.Invoke();
        }

        public void PlayCastSkill()
        {
            if (isDead) return;
            EnterActionState();
            animator.SetTrigger(CastTrigger);
        }

        public void PlayDie()
        {
            isDead = true;
            EnterActionState();
            animator.SetTrigger(DieTrigger);
        }

        public void PlayHit()
        {
            if (isDead) return;
            
            animator.ResetTrigger(AttackTrigger);
            animator.ResetTrigger(RollTrigger);
            animator.ResetTrigger(CastTrigger);
            
            EnterActionState();
            animator.SetFloat(SpeedHash, 0f);
            animator.SetTrigger(HitTrigger);
        }

        public void ResetDeathState()
        {
            isDead = false;
            IsStunned = false;
        }

        public void EnableAnimator(bool enable)
        {
            animator.enabled = enable;
        }

        public bool IsInNonCombatState()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Idle") || stateInfo.IsName("Walk") || stateInfo.IsName("Roll");
        }

        public bool IsInLocomotion() => currentAnimationType == AnimationType.Locomotion;

        public bool IsInAction() => currentAnimationType == AnimationType.Action;

        public void AnimEvent_AttackEnd()
        {
            ExitActionState();
            OnAttackEnd?.Invoke();
        }

        public void AnimEvent_RollEnd()
        {
            ExitActionState();
            OnRollEnd?.Invoke();
        }

        public void AnimEvent_CanMove() => OnCanMove?.Invoke();
        public void AnimEvent_CanNotMove() => OnCanNotMove?.Invoke();
        public void AnimEvent_EnableWeapon() => OnEnableWeapon?.Invoke();
        public void AnimEvent_DisableWeapon() => OnDisableWeapon?.Invoke();
        public void AnimEvent_EnableIFrame() => OnEnableIFrame?.Invoke();
        
        public void AnimEvent_DealDamage() => OnDealDamage?.Invoke();

        public void AnimEvent_DisableIFrame()
        {
            OnDisableIFrame?.Invoke();
            ExitActionState();
        }

        public void AnimEvent_DeathAnimationEnd()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.GameOver)
            {
                GameStateManager.Instance.SetState(GameState.GameOver);
            }
        }

        public void AnimEvent_HitEnd()
        {
            ExitActionState();
            OnHitEnd?.Invoke();
        }

        public Animator GetAnimator() => animator;

        public void SetLayerWeight(string layerName, float weight)
        {
            int index = animator.GetLayerIndex(layerName);
            if (index >= 0) animator.SetLayerWeight(index, weight);
        }
    }
}