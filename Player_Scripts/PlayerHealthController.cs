using UnityEngine;
using CoreSystem;
using System;
using GameSystem;
using Photon.Pun;

namespace PlayerInputs
{
    [RequireComponent(typeof(Health))]
    public class PlayerHealthController : MonoBehaviour, IDeathSequenceNotifier
    {
        private Health health;
        private PlayerAnimationFacade animationFacade;
        private PlayerInputController inputController;
        private float previousHealth;

        private PlayerCombatController combatController;
        private PlayerMovementController movementController;
        private PlayerManaSkillController manaSkillController;
        private PlayerStaminaController staminaController;
        private MonoBehaviour unityPlayerInput;
        private PlayerStateController stateController;

        private bool isIncapacitated = false;

        public event Action OnDeathSequenceComplete;

        private void Awake()
        {
            health = GetComponent<Health>();
            if (animationFacade == null) animationFacade = GetComponentInChildren<PlayerAnimationFacade>();
            inputController = GetComponent<PlayerInputController>();
            stateController = GetComponentInParent<PlayerStateController>();
            
            previousHealth = health.GetMaxHealth();

            combatController = GetComponentInChildren<PlayerCombatController>();
            movementController = GetComponentInChildren<PlayerMovementController>();
            manaSkillController = GetComponentInChildren<PlayerManaSkillController>();
            staminaController = GetComponentInChildren<PlayerStaminaController>();

            var playerInputType = System.Type.GetType("UnityEngine.InputSystem.PlayerInput, Unity.InputSystem");
            if (playerInputType != null)
            {
                unityPlayerInput = GetComponent(playerInputType) as MonoBehaviour;
            }
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.OnHealthChanged += HandleHealthChanged;
                health.OnDeath += HandleDeath;
            }

            if (animationFacade != null)
            {
                animationFacade.OnDeathAnimationComplete += HandleDeathSequenceComplete;
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnHealthChanged -= HandleHealthChanged;
                health.OnDeath -= HandleDeath;
            }

            if (animationFacade != null)
            {
                animationFacade.OnDeathAnimationComplete -= HandleDeathSequenceComplete;
            }
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            if (currentHealth < previousHealth && currentHealth > 0 && !isIncapacitated)
            {
                if (animationFacade != null)
                {
                    animationFacade.PlayHit();
                }
            }
            previousHealth = currentHealth;
        }

        private void HandleDeath()
        {
            if (isIncapacitated) return;
            
            isIncapacitated = true;
            SetPlayerActiveState(false);
            
            if (animationFacade != null)
            {
                animationFacade.PlayDie();
            }
        }

        private void HandleDeathSequenceComplete()
        {
            OnDeathSequenceComplete?.Invoke();

            // หากอยู่ในโหมด Offline หรือไม่มีตัวจัดการออนไลน์ใน Scene ให้แสดง Loss UI ทันที
            if (!PhotonNetwork.IsConnected || FindObjectOfType<OnlineSystem.NetworkGameStateManager>() == null)
            {
                ExecuteLocalLoss();
            }
        }

        private void ExecuteLocalLoss()
        {
            UIManager.IsWin = false;
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.GameOver);
            }
        }

        private void SetPlayerActiveState(bool isActive)
        {
            if (stateController != null) stateController.SetControl(isActive);
            
            if (inputController != null) inputController.enabled = isActive;
            
            if (combatController != null) combatController.enabled = isActive;
            if (movementController != null) movementController.enabled = isActive;
            if (manaSkillController != null) manaSkillController.enabled = isActive;
            if (staminaController != null) staminaController.enabled = isActive;
            
            if (unityPlayerInput != null) unityPlayerInput.enabled = isActive;

            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                if (!(col is CharacterController))
                {
                    col.enabled = isActive;
                }
            }

            CharacterController cc = GetComponentInChildren<CharacterController>();
            if (cc != null)
            {
                cc.enabled = isActive;
            }
        }

        public void Respawn(Vector3 spawnPosition)
        {
            isIncapacitated = false;

            if (health != null)
            {
                health.ResetHealth();
                previousHealth = health.GetMaxHealth();
            }

            if (staminaController != null) staminaController.ResetStamina();
            if (manaSkillController != null) manaSkillController.ResetMana();
            if (combatController != null) combatController.ResetCombatState();

            CharacterController cc = GetComponentInChildren<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                transform.position = spawnPosition;
                cc.enabled = true;
            }
            else
            {
                transform.position = spawnPosition;
            }

            SetPlayerActiveState(true);

            if (animationFacade != null)
            {
                animationFacade.ResetDeathState();
                Animator anim = animationFacade.GetComponent<Animator>();
                if (anim != null)
                {
                    // Disable animator before rebinding to prevent UnityEditor.Graphs errors
                    anim.enabled = false;
                    anim.Rebind();
                    anim.Update(0f);
                    anim.enabled = true;
                }
            }
        }
    }
}