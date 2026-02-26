using UnityEngine;

namespace PlayerInputs
{
    public class PlayerInputController : MonoBehaviour
    {
        private PlayerControls controls;
        private PlayerMovementController movement;
        private PlayerCombatController combat;
        private PlayerStaminaController stamina;
        private PlayerManaSkillController skill;
        private PlayerInteractController interact;
        private CameraViewSwitcher cameraViewSwitcher;
        private FPSMouseLook mouseLook;

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
        }

        void OnEnable()
        {
            // ===== Move (ใช้ได้ทุกโหมด) =====
            controls.Player.Move.performed += ctx =>
            {
                movement.SetMoveInput(ctx.ReadValue<Vector2>());
            };
            controls.Player.Move.canceled += _ =>
            {
                movement.SetMoveInput(Vector2.zero);
            };

            // ===== Look (ใช้ได้ทุกโหมด) =====
            controls.Player.Look.performed += ctx => mouseLook.OnLook(ctx);
            controls.Player.Look.canceled  += ctx => mouseLook.OnLook(ctx);

            // ===== Interact (ใช้ได้ทุกโหมด) =====
            controls.Player.Interact.performed += _ => interact.TryInteract();
            controls.Player.Interact.canceled  += _ => interact.StopInteract();

            // ===== Parry (ใช้ได้ทุกโหมด) =====
            controls.Player.Parry.performed += _ =>
            {
                JumpScareParryManager.Instance?.OnParryInput();
            };

            // ===== Third Person ONLY =====
            controls.Player.Attack.performed += _ =>
            {
                if (cameraViewSwitcher.IsFirstPerson) return;
                combat.Attack();
            };

            controls.Player.Roll.performed += _ =>
            {
                if (cameraViewSwitcher.IsFirstPerson) return;
                stamina.TryRoll();
            };

            controls.Player.Ability1.performed += _ =>
            {
                if (cameraViewSwitcher.IsFirstPerson) return;
                skill.CastSkill(0);
            };

            controls.Player.Ability2.performed += _ =>
            {
                if (cameraViewSwitcher.IsFirstPerson) return;
                skill.CastSkill(1);
            };

            controls.Player.Ability3.performed += _ =>
            {
                if (cameraViewSwitcher.IsFirstPerson) return;
                skill.CastSkill(2);
            };

            // ===== Switch View =====
            controls.Player.SwitchView.performed += _ =>
            {
                cameraViewSwitcher.SwitchView();
            };

            controls.Enable();
        }


        void OnDisable() => controls.Disable();
    }

}