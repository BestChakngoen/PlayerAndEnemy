using UnityEngine;
using UnityEngine.UI;
using GameManger;

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

        [Header("Audio")]
        [SerializeField] private AudioClip[] rollSounds;

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
            if (!PlayerStateController.CanControl) return false;

            if (stamina < rollCost || cooldown > 0f) return false;

            stamina -= rollCost;
            cooldown = rollCooldown;

            UpdateStaminaUI();

            if (rollSounds != null && rollSounds.Length > 0 && AudioManager.Instance != null)
            {
                AudioClip clip = rollSounds[Random.Range(0, rollSounds.Length)];
                AudioManager.Instance.PlaySFX(clip, transform.position);
            }

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