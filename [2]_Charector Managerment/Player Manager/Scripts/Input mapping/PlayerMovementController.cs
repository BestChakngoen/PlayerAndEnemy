using UnityEngine;

namespace PlayerInputs
{
    public class PlayerMovementController : MonoBehaviour
    {
        public enum MovementMode { FirstPerson, ThirdPerson }

        [Header("Mode")]
        [SerializeField] private MovementMode currentMode = MovementMode.ThirdPerson;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private PlayerAnimationFacade animationFacade;

        [Header("Movement")]
        [SerializeField] public float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [SerializeField] private CharacterController controller;
        private Vector2 moveInput;
        private Vector3 moveDirection;
        private PlayerCombatController combat;
        private Vector3 lastMoveDirection;



        void Awake()
        {
            combat = GetComponentInChildren<PlayerCombatController>();
            if (controller == null)
            {
                Debug.LogError("CharacterController not found in children!", this);
                enabled = false; 
                return;
            }
            playerRoot = controller.transform;

            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;

            if (animationFacade == null)
            {
                Debug.LogError("PlayerAnimationFacade not found in children!", this);
                enabled = false; 
                return;
            }
        }

        void Update()
        {
            if (!PlayerStateController.CanControl)
                return;

            if (combat != null && combat.CanMove == false)
            {
                moveDirection  = Vector2.zero;
                animationFacade.SetMovementSpeed(0f);
                return;
            }

            HandleMovement();
            HandleAnimation();
        }

        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        private void HandleMovement()
        {
            if (moveInput.sqrMagnitude < 0.01f)
            {
                moveDirection = Vector3.zero;
                return;
            }

            if (currentMode == MovementMode.FirstPerson)
            {
                // ===== First Person =====
                moveDirection =
                    playerRoot.forward * moveInput.y +
                    playerRoot.right * moveInput.x;
            }
            else
            {
                // ===== Third Person =====
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight   = cameraTransform.right;

                camForward.y = 0f;
                camRight.y   = 0f;

                camForward.Normalize();
                camRight.Normalize();

                moveDirection =
                    camForward * moveInput.y +
                    camRight * moveInput.x;

                // หมุนตัวละครไปทิศทางที่เดิน 
                if (moveDirection.sqrMagnitude > 0.001f)
                {
                    lastMoveDirection = moveDirection; 
                    Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
                    playerRoot.rotation = Quaternion.Slerp(
                        playerRoot.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }

            controller.Move(
                moveDirection.normalized * moveSpeed * Time.deltaTime
            );
        }

        private void HandleAnimation()
        {
            // ใช้ Speed ตัวเดียว (Idle ↔ Walk)
            animationFacade.SetMovementSpeed(moveInput.magnitude);
        }

        public void SetMovementMode(MovementMode mode)
        {
            currentMode = mode;
        }
    }
}
