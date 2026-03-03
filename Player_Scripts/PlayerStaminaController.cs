using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PlayerInputs.Core;

namespace PlayerInputs
{
    public class PlayerStaminaController : MonoBehaviour, IActionLockable
    {
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float rollCost = 25f;
        [SerializeField] private float regenRate = 15f;
        [SerializeField] private float regenDelay = 1.0f;
        [SerializeField] private PlayerAnimationFacade animationFacade;
        
        [Header("UI Settings")]
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Image staminaFillImage;
        [SerializeField] private Color normalColor = Color.yellow;
        [SerializeField] private Color flashColor = Color.red;
        [SerializeField] private float flashDuration = 0.2f;

        public event Action<float, float> OnStaminaChanged;

        private float stamina;
        private float cooldown;
        private PlayerMovementController movementController;
        private PlayerStateController stateController;
        private bool isActionLocked = false;
        private Coroutine flashCoroutine;

        void Awake()
        {
            if (animationFacade == null)
            {
                enabled = false; 
                return;
            }
            
            movementController = GetComponent<PlayerMovementController>();
            stateController = GetComponentInParent<PlayerStateController>();

            if (staminaSlider != null)
            {
                staminaSlider.maxValue = maxStamina;
            }
            if (staminaFillImage != null)
            {
                staminaFillImage.color = normalColor;
            }
        }

        void Start()
        {
            stamina = maxStamina;
            UpdateUI();
            OnStaminaChanged?.Invoke(stamina, maxStamina);
            if (animationFacade != null)
            {
                animationFacade.OnRollEnd += HandleRollEnd;
            }
        }

        void OnDestroy()
        {
            if (animationFacade != null)
            {
                animationFacade.OnRollEnd -= HandleRollEnd;
            }
        }

        void Update()
        {
            if (stateController != null && !stateController.CanControl) return;

            HandleRegen();
            HandleCooldown();
        }

        public void LockAction()
        {
            isActionLocked = true;
        }

        public void UnlockAction()
        {
            isActionLocked = false;
        }

        public void ResetStamina()
        {
            stamina = maxStamina;
            cooldown = 0f;
            isActionLocked = false;
            UpdateUI();
            OnStaminaChanged?.Invoke(stamina, maxStamina);
        }

        public bool TryRoll()
        {
            if (isActionLocked) return false;
            if (stateController != null && !stateController.CanControl) return false;

            if (stamina < rollCost || cooldown > 0f)
            {
                FlashStaminaBar();
                return false;
            }

            stamina -= rollCost;
            cooldown = regenDelay;
            
            UpdateUI();
            OnStaminaChanged?.Invoke(stamina, maxStamina);
            
            animationFacade.PlayRoll();
            return true;
        }

        private void HandleRegen()
        {
            if (cooldown <= 0f && stamina < maxStamina)
            {
                stamina += regenRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
                UpdateUI();
                OnStaminaChanged?.Invoke(stamina, maxStamina);
            }
        }

        private void HandleCooldown()
        {
            if (cooldown > 0f)
            {
                cooldown -= Time.deltaTime;
            }
        }
        
        private void UpdateUI()
        {
            if (staminaSlider != null)
            {
                staminaSlider.value = stamina;
            }
        }

        private void FlashStaminaBar()
        {
            if (staminaFillImage == null) return;
            
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            staminaFillImage.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            staminaFillImage.color = normalColor;
        }
        
        private void HandleRollEnd()
        {
        }
    }
}