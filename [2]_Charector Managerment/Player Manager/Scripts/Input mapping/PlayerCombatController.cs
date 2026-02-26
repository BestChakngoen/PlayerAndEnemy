using UnityEngine;

namespace PlayerInputs
{
    public class PlayerCombatController : MonoBehaviour
    {
        [Header("Combo Settings")]
        [SerializeField] private int maxCombo = 4;
        [SerializeField] private float comboResetTime = 0.8f;
        [SerializeField] private FPSMouseLook mouseLook;
        [SerializeField] private PlayerAnimationFacade anim;
        private int currentCombo;
        private bool isAttacking;
        private float comboResetTimer;
        private bool canMove = true;
        public bool CanMove => canMove;
        public bool IsAttacking => isAttacking;

        private void Awake()
        {
            if (anim == null)
            {
                Debug.LogError("PlayerAnimationFacade not found in children!", this);
                enabled = false; 
                return;
            }
            anim.OnAttackEnd += OnAttackEnd;
            anim.OnCanMove += OnCanMove;
        }

        private void OnDestroy()
        {
            anim.OnAttackEnd -= OnAttackEnd;
            anim.OnCanMove -= OnCanMove;
        }

        // =======================
        // Public API
        // =======================
        public void Attack()
        {
            // ถ้าไม่อยู่ในสถานะ idle → เล่น attack ต่อทันที
            canMove = false;
            if (!isAttacking)
            {
                currentCombo++;

                if (currentCombo > maxCombo)
                    currentCombo = 0;

                //mouseLook.LockRotation(true);
                isAttacking = true;
                comboResetTimer = 0f;
                anim.PlayAttack(currentCombo);
            }
        }

        // =======================
        // Animation Event
        // =======================
        private void OnAttackEnd()
        {
            //mouseLook.LockRotation(false);
            isAttacking = false;
            comboResetTimer = comboResetTime;
        }

        private void OnCanMove()
        {
            canMove = true;
        }

        private void Update()
        {
            //Debug.Log(canMove);
            if (!PlayerStateController.CanControl) return;

            HandleComboReset();
        }

        private void HandleComboReset()
        {
            if (isAttacking || comboResetTimer <= 0f)
                return;

            comboResetTimer -= Time.deltaTime;
            //Debug.Log("comboResetTimer" + comboResetTimer);

            if (comboResetTimer <= 0f)
            {
                ResetCombo();
            }
        }

        private void ResetCombo()
        {
            currentCombo = 0;
            comboResetTimer = 0f;
            anim.SetAttackCombo(currentCombo);
        }
    }
}