using UnityEngine;
using PlayerInputs.Core;
using Boss.core;

namespace PlayerInputs
{
    public class PlayerMovementController : MonoBehaviour, IPlayerMovement, ISpeedModifiable
    {
        public enum MovementMode { FirstPerson, ThirdPerson }

        [SerializeField] private MovementMode currentMode = MovementMode.ThirdPerson;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController controller;
        [SerializeField] private PlayerAnimationFacade animationFacade;
        
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;

        [Header("Roll Settings")]
        public float rollSpeed = 8f; // กำหนดความเร็วในการกลิ้งได้ที่ Inspector
        private bool isRolling = false;
        private Vector3 rollDirection;

        private Vector2 moveInput;
        private IPlayerCombat combatState;
        private Transform playerRoot;
        
        private float speedMultiplier = 1.0f;

        private void Awake()
        {
            combatState = GetComponentInChildren<IPlayerCombat>();
            if (controller != null) playerRoot = controller.transform;
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;

            if (animationFacade != null)
            {
                animationFacade.OnRollEnd += EndRoll;
            }
        }

        private void OnDestroy()
        {
            if (animationFacade != null)
            {
                animationFacade.OnRollEnd -= EndRoll;
            }
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

        public void StartRoll()
        {
            if (isRolling) return;
            
            isRolling = true;
            Vector3 moveDir = CalculateMoveDirection();
            
            // ตรวจสอบว่าผู้เล่นกดทิศทางอยู่หรือไม่ ถ้ากดให้กลิ้งไปทางนั้น ถ้าไม่ได้กดให้กลิ้งไปด้านหน้าตัวละคร
            if (moveDir.sqrMagnitude > 0.01f)
            {
                rollDirection = moveDir.normalized;
            }
            else
            {
                rollDirection = playerRoot.forward;
            }
        }

        private void EndRoll()
        {
            isRolling = false;
        }

        private void HandleMovementAndRotation()
        {
            if (playerRoot == null) return;

            // หากอยู่สถานะกลิ้ง ให้บังคับขยับตัวและหันหน้าตามทิศทางกลิ้งอย่างเดียว
            if (isRolling)
            {
                controller.Move(rollDirection * rollSpeed * Time.deltaTime);
                RotatePlayerTowards(rollDirection);
                return; 
            }

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
                controller.Move(moveDirection.normalized * (moveSpeed * speedMultiplier) * Time.deltaTime);
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

        public void MultiplySpeed(float multiplier)
        {
            speedMultiplier *= multiplier;
        }

        public void DivideSpeed(float multiplier)
        {
            if (multiplier != 0)
            {
                speedMultiplier /= multiplier;
            }
        }
    }
}