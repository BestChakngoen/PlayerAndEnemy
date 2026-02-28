using UnityEngine;
using UnityEngine.UI;

namespace PlayerInputs
{
    public class PlayerStaminaController : MonoBehaviour
    {
        public float maxStamina = 100f;
        public float regenRate = 10f;
        public float rollCost = 20f;
        public float rollCooldown = 2f;

        [SerializeField] private Slider staminaSlider;
        [SerializeField] private PlayerAnimationFacade animationFacade;

        private float stamina;
        private float cooldown;
        private PlayerMovementController movementController;

        void Awake()
        {
            if (animationFacade == null)
            {
                enabled = false; 
                return;
            }
            // ดึงค่า Controller ไว้สำหรับสั่งการกลิ้ง
            movementController = GetComponent<PlayerMovementController>();
        }

        void Start()
        {
            stamina = maxStamina;

            if (staminaSlider != null)
            {
                staminaSlider.maxValue = maxStamina;
                staminaSlider.value = stamina;
            }
        }

        void Update()
        {
            if (!PlayerStateController.CanControl) return;

            HandleRegen();
            HandleCooldown();
        }

        public bool TryRoll()
        {
            if (stamina < rollCost || cooldown > 0f) return false;

            stamina -= rollCost;
            cooldown = rollCooldown;

            UpdateStaminaUI();

            // สั่งเล่น Animation และเริ่มการคำนวณ Movement ใหม่ทันที
            if (animationFacade != null) animationFacade.PlayRoll();
            if (movementController != null) movementController.StartRoll();

            return true;
        }

        public float GetCurrentStamina() => stamina;

        public bool HasEnoughStamina(float cost) => stamina >= cost;

        private void HandleRegen()
        {
            if (stamina >= maxStamina) return;

            stamina += regenRate * Time.deltaTime;
            stamina = Mathf.Min(stamina, maxStamina);

            UpdateStaminaUI();
        }

        private void HandleCooldown()
        {
            if (cooldown > 0f)
            {
                cooldown -= Time.deltaTime;
                cooldown = Mathf.Max(cooldown, 0f);
            }
        }

        private void UpdateStaminaUI()
        {
            if (staminaSlider != null)
            {
                staminaSlider.value = stamina;
            }
        }
    }
}