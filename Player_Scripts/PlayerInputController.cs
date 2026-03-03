using UnityEngine;
using GameSystem;
using UnityEngine.SceneManagement;

namespace PlayerInputs
{
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Event Listening")]
        public GameStateEventSO GameStateChangedChannel;

        private PlayerControls controls;
        private PlayerMovementController movement;
        private PlayerCombatController combat;
        private PlayerStaminaController stamina;
        private PlayerManaSkillController skill;
        private PlayerInteractController interact;
        private CameraViewSwitcher cameraViewSwitcher;
        private FPSMouseLook mouseLook;
        private PlayerStateController stateController;

        void Awake()
        {
            controls = new PlayerControls();
            movement = GetComponentInChildren<PlayerMovementController>();
            combat   = GetComponentInChildren<PlayerCombatController>();
            stamina  = GetComponentInChildren<PlayerStaminaController>();
            skill    = GetComponentInChildren<PlayerManaSkillController>();
            interact = GetComponentInChildren<PlayerInteractController>();
            cameraViewSwitcher = GetComponentInChildren<CameraViewSwitcher>();
            mouseLook = GetComponentInChildren<FPSMouseLook>();
            
            // ดึงคอมโพเนนต์ PlayerStateController ที่อยู่ในตัวแม่หรือตัวมันเอง
            stateController = GetComponentInParent<PlayerStateController>();

            SetupInputEvents();
        }

        private void SetupInputEvents()
        {
            controls.Player.Move.performed += ctx =>
            {
                if (movement != null) movement.SetMoveInput(ctx.ReadValue<Vector2>());
            };
            controls.Player.Move.canceled += _ =>
            {
                if (movement != null) movement.SetMoveInput(Vector2.zero);
            };

            controls.Player.Look.performed += ctx => { if (mouseLook != null) mouseLook.OnLook(ctx); };
            controls.Player.Look.canceled  += ctx => { if (mouseLook != null) mouseLook.OnLook(ctx); };

            controls.Player.Interact.performed += _ => 
            { 
                if (stateController != null && !stateController.CanControl) return;
                if (interact != null) interact.TryInteract(); 
            };
            controls.Player.Interact.canceled  += _ => 
            { 
                if (stateController != null && !stateController.CanControl) return;
                if (interact != null) interact.StopInteract(); 
            };

            controls.Player.Parry.performed += _ =>
            {
                if (JumpScareParryManager.Instance != null) JumpScareParryManager.Instance.OnParryInput();
            };

            controls.Player.Attack.performed += _ =>
            {
                if (stateController != null && !stateController.CanControl) return;
                if (cameraViewSwitcher != null && cameraViewSwitcher.IsFirstPerson) return;
                if (combat != null) combat.Attack();
            };

            controls.Player.Roll.performed += _ =>
            {
                if (stateController != null && !stateController.CanControl) return;
                if (cameraViewSwitcher != null && cameraViewSwitcher.IsFirstPerson) return;
                if (stamina != null) stamina.TryRoll();
            };

            controls.Player.SwitchView.performed += _ =>
            {
                if (cameraViewSwitcher != null) cameraViewSwitcher.SwitchView();
            };
        }

        void OnEnable()
        {
            if (GameStateChangedChannel != null)
            {
                GameStateChangedChannel.OnEventRaised.AddListener(HandleGameStateChanged);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;

            bool isGameplay = true;
            if (GameStateManager.Instance != null)
            {
                isGameplay = GameStateManager.Instance.CurrentState == GameState.Gameplay;
            }

            if (isGameplay)
            {
                controls.Enable();
            }
        }

        void OnDisable()
        {
            if (GameStateChangedChannel != null)
            {
                GameStateChangedChannel.OnEventRaised.RemoveListener(HandleGameStateChanged);
            }
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            controls.Disable();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (movement != null) movement.enabled = true;
            if (mouseLook != null) mouseLook.LockRotation(false);
            if (combat != null) combat.enabled = true;
            if (stamina != null) stamina.enabled = true;
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState == GameState.Gameplay)
            {
                controls.Enable();
            }
            else
            {
                controls.Disable();
                
                if (movement != null)
                {
                    movement.SetMoveInput(Vector2.zero);
                }
            }
        }
    }
}