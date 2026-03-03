using UnityEngine;

namespace PlayerInputs
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float gravityMultiplier = 1.0f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float jumpHeight = 2.0f;

        private CharacterController controller;
        private Transform playerRoot;
        private PlayerHealthController healthController;
        private PlayerStateController stateController;

        private Vector3 velocity;
        private bool isDead = false;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (controller != null) playerRoot = controller.transform;

            healthController = GetComponent<PlayerHealthController>();
            stateController = GetComponentInParent<PlayerStateController>();
            
            if (healthController != null)
            {
                healthController.OnDeathSequenceComplete += HandleDeathSequenceComplete;
            }
        }

        private void OnDestroy()
        {
            if (healthController != null)
            {
                healthController.OnDeathSequenceComplete -= HandleDeathSequenceComplete;
            }
        }

        private void Update()
        {
            if (isDead) return;
            if (stateController != null && !stateController.CanControl) return;

            ApplyGravity();
        }

        private void ApplyGravity()
        {
            if (controller == null) return;

            bool grounded = isGrounded();

            if (grounded && velocity.y < 0)
            {
                velocity.y = -2f; 
            }
            else
            {
                velocity.y += gravity * gravityMultiplier * Time.deltaTime;
            }

            // เช็คก่อนเสมอว่า CharacterController เปิดใช้งานอยู่หรือไม่ก่อนสั่ง Move
            if (controller.enabled)
            {
                controller.Move(velocity * Time.deltaTime);
            }
        }

        public void Jump()
        {
            if (isGrounded())
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private bool isGrounded()
        {
            return controller != null && controller.isGrounded;
        }

        public bool IsGrounded() => isGrounded();

        private void HandleDeathSequenceComplete()
        {
            isDead = true;
        }

        // เพิ่มไว้สำหรับกรณีที่ต้องการรีเซ็ตสถานะเมื่อผู้เล่น Respawn ใหม่
        public void ResetPhysics()
        {
            isDead = false;
            velocity = Vector3.zero;
        }
    }
}