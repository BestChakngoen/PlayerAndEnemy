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

        void Awake()
        {
            if (animationFacade == null)
            {
                enabled = false; 
                return;
            }
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

            if (animationFacade != null) animationFacade.PlayRoll();

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