using UnityEngine;
using PlayerInputs.Core;

namespace PlayerInputs
{
    public class PlayerCombatController : MonoBehaviour, IPlayerCombat
    {
        [SerializeField] private int maxCombo = 4;
        [SerializeField] private float comboResetTime = 0.8f;
        [SerializeField] private PlayerAnimationFacade anim;
        
        private int currentCombo;
        private bool isAttacking;
        private float comboResetTimer;
        private bool canMove = true;
        
        public bool CanMove => canMove;

        private void Awake()
        {
            anim.OnAttackEnd += ResetAttackState;
            anim.OnCanMove += EnableMovement;
        }

        private void Update()
        {
            if (isAttacking) return;

            if (comboResetTimer > 0)
            {
                comboResetTimer -= Time.deltaTime;
                if (comboResetTimer <= 0) currentCombo = 0;
            }
        }

        public void Attack()
        {
            if (isAttacking || !PlayerStateController.CanControl) return;

            canMove = false;
            isAttacking = true;
            currentCombo = (currentCombo % maxCombo) + 1;
            anim.PlayAttack(currentCombo);
        }

        private void ResetAttackState()
        {
            isAttacking = false;
            comboResetTimer = comboResetTime;
        }

        private void EnableMovement()
        {
            canMove = true;
        }
    }
}