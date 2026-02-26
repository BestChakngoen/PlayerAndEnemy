using UnityEngine;

namespace PlayerInputs
{
    public class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private int maxCombo = 4;
        [SerializeField] private float comboResetTime = 0.8f;
        [SerializeField] private PlayerAnimationFacade anim;
        
        private int currentCombo;
        private bool isAttacking;
        private float comboResetTimer;
        private bool canMove = true;
        
        public bool CanMove => canMove;

        void Awake()
        {
            anim.OnAttackEnd += () => { isAttacking = false; comboResetTimer = comboResetTime; };
            anim.OnCanMove += () => canMove = true;
        }

        public void Attack()
        {
            if (isAttacking || !PlayerStateController.CanControl) return;

            canMove = false;
            isAttacking = true;
            currentCombo = (currentCombo % maxCombo) + 1;
            anim.PlayAttack(currentCombo);
        }

        void Update()
        {
            if (isAttacking) return;

            if (comboResetTimer > 0)
            {
                comboResetTimer -= Time.deltaTime;
                if (comboResetTimer <= 0) currentCombo = 0;
            }
        }
    }
}