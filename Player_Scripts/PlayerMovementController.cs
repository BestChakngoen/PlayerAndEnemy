using UnityEngine;

namespace PlayerInputs
{
    public class PlayerMovementController : MonoBehaviour
    {
        public enum MovementMode { FirstPerson, ThirdPerson }

        [SerializeField] private MovementMode currentMode = MovementMode.ThirdPerson;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController controller;
        [SerializeField] private PlayerAnimationFacade animationFacade;
        
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;

        private Vector2 moveInput;
        private PlayerCombatController combat;
        private Transform playerRoot;

        void Awake()
        {
            combat = GetComponentInChildren<PlayerCombatController>();
            if (controller != null) playerRoot = controller.transform;
            if (cameraTransform == null) cameraTransform = Camera.main.transform;
        }

        void Update()
        {
            if (!PlayerStateController.CanControl) return;

            if (combat != null && !combat.CanMove)
            {
                animationFacade.SetMovementSpeed(0f);
                return;
            }

            HandleMovement();
            animationFacade.SetMovementSpeed(moveInput.magnitude);
        }

        public void SetMoveInput(Vector2 input) => moveInput = input;

        public void SetMovementMode(MovementMode mode)
        {
            currentMode = mode;
        }

        private void HandleMovement()
        {
            if (moveInput.sqrMagnitude < 0.01f || playerRoot == null) return;

            Vector3 moveDirection;

            if (currentMode == MovementMode.FirstPerson)
            {
                moveDirection = playerRoot.forward * moveInput.y + playerRoot.right * moveInput.x;
            }
            else
            {
                Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
                moveDirection = camForward * moveInput.y + camRight * moveInput.x;

                if (moveDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                    playerRoot.rotation = Quaternion.Slerp(playerRoot.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }

            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
    }
}