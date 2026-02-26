using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInputs
{
    public class FPSMouseLook : MonoBehaviour
    {
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Transform playerBody;
        [SerializeField] private float sensitivityX = 0.5f;
        [SerializeField] private float sensitivityY = 0.5f;
        [SerializeField] private bool invertX = false;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;
        
        public enum CameraMode { FirstPerson, ThirdPerson }

        [SerializeField] private CameraMode currentMode = CameraMode.FirstPerson;

        private Vector2 lookInput;
        private float pitch;
        private float yaw;
        private bool isRotationLocked = false;

        private void Start()
        {
            SyncRotation();
        }

        public void SetCameraMode(CameraMode mode)
        {
            currentMode = mode;
            if (currentMode == CameraMode.FirstPerson)
            {
                SyncRotation();
            }
        }

        private void SyncRotation()
        {
            if (playerBody != null)
            {
                yaw = playerBody.eulerAngles.y;
            }
            if (cameraRoot != null)
            {
                pitch = cameraRoot.localEulerAngles.x;
                if (pitch > 180f) pitch -= 360f;
            }
        }

        public void LockRotation(bool lockRotation) => isRotationLocked = lockRotation;

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        private void LateUpdate()
        {
            if (isRotationLocked || currentMode != CameraMode.FirstPerson) return;
            
            CalculateRotation();
            ApplyRotation();
        }

        private void CalculateRotation()
        {
            float mouseX = lookInput.x * sensitivityX;
            float mouseY = lookInput.y * sensitivityY;

            yaw += invertX ? -mouseX : mouseX;
            pitch += invertY ? mouseY : -mouseY;
            
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        private void ApplyRotation()
        {
            if (cameraRoot != null)
            {
                cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
            
            if (playerBody != null)
            {
                playerBody.localRotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
    }
}