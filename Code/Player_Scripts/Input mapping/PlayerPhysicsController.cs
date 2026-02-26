using UnityEngine;

namespace PlayerInputs
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        private Vector3 velocity;
        
        [Header("Physics Settings")]
        [SerializeField] private float gravity = -9.81f;
        //[SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float jumpHeight = 2.0f;

        private bool isGrounded;

        void Awake()
        {
            if (characterController == null)
            {
                Debug.LogError("characterController not found in children!", this);
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
            // ตรวจสอบว่าอยู่บนพื้นหรือไม่
            isGrounded = characterController.isGrounded;

            if (isGrounded && velocity.y < 0)
            {
                // ใส่ค่าติดลบเล็กน้อยเพื่อให้ตัวละครติดกับพื้นเสมอ (Grounded)
                velocity.y = -2f; 
            }

            // คำนวณแรงโน้มถ่วง: Δv = g * t
            velocity.y += gravity * Time.deltaTime;

            // เคลื่อนที่ตามแรง Velocity (y)
            // ใช้สูตร d = v * t
            characterController.Move(velocity * Time.deltaTime);
        }

        public void Jump()
        {
            if (isGrounded)
            {
                // สูตรฟิสิกส์สำหรับการหาความเร็วเริ่มต้นเพื่อกระโดดให้ได้ความสูงที่ต้องการ: v = sqrt(h * -2 * g)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        public bool IsGrounded() => isGrounded;
    }
}