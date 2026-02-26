using UnityEngine;

namespace PlayerInputs
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float jumpHeight = 2.0f;

        private Vector3 velocity;
        private bool isGrounded;

        void Awake()
        {
            if (characterController == null)
            {
                enabled = false; 
                return;
            }
        }

        void Update()
        {
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            isGrounded = characterController.isGrounded;

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; 
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        public void Jump()
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        public bool IsGrounded() => isGrounded;
    }
}