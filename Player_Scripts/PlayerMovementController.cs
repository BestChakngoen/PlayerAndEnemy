using UnityEngine;
using PlayerInputs.Core;

namespace PlayerInputs
{
    public class PlayerMovementController : MonoBehaviour, IPlayerMovement
    {
        public enum MovementMode { FirstPerson, ThirdPerson }

        [SerializeField] private MovementMode currentMode = MovementMode.ThirdPerson;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController controller;
        [SerializeField] private PlayerAnimationFacade animationFacade;
        
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;

        private Vector2 moveInput;
        private IPlayerCombat combatState;
        private Transform playerRoot;

        private void Awake()
        {
            combatState = GetComponentInChildren<IPlayerCombat>();
            if (controller != null) playerRoot = controller.transform;
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (!PlayerStateController.CanControl) return;

            HandleMovementAndRotation();
        }

        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        public void SetMovementMode(MovementMode mode) => currentMode = mode;

        private void HandleMovementAndRotation()
        {
            if (playerRoot == null) return;

            if (combatState != null && !combatState.CanMove)
            {
                animationFacade.SetMovementSpeed(0f);
                return;
            }

            Vector3 moveDirection = CalculateMoveDirection();

            if (moveInput.sqrMagnitude > 0.01f && currentMode == MovementMode.ThirdPerson && moveDirection.sqrMagnitude > 0.001f)
            {
                RotatePlayerTowards(moveDirection);
            }

            if (moveInput.sqrMagnitude > 0.01f)
            {
                controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            }

            animationFacade.SetMovementSpeed(moveInput.magnitude);
        }

        private Vector3 CalculateMoveDirection()
        {
            if (currentMode == MovementMode.FirstPerson)
            {
                return playerRoot.forward * moveInput.y + playerRoot.right * moveInput.x;
            }

            Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            return camForward * moveInput.y + camRight * moveInput.x;
        }

        private void RotatePlayerTowards(Vector3 direction)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            playerRoot.rotation = Quaternion.Slerp(playerRoot.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}