using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMouseLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraRoot;

    [Header("Sensitivity")]
    [SerializeField] private float sensitivityX = 0.15f;
    [SerializeField] private float sensitivityY = 0.15f;

    [Header("Invert Axis")]
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    [Header("Human View Limits (Pitch)")]
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson
    }

    [SerializeField] private CameraMode currentMode = CameraMode.FirstPerson;

    public void SetCameraMode(CameraMode mode)
    {
        currentMode = mode;
    }


    private Vector2 lookInput;
    private float pitch;
    private bool isRotationLocked = false;


    public void LockRotation(bool lockRotation)
    {
        isRotationLocked = lockRotation;
    }

    // รับค่าจาก Input System (Event-based)
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    void Update()
    {

        if (isRotationLocked)
            return;

        if (currentMode == CameraMode.FirstPerson)
        {
            RotateYaw();
            RotatePitch();
        }
    }

    private void RotateYaw()
    {
        float mouseX = lookInput.x * sensitivityX;
        if (invertX) mouseX *= -1f;

        // หมุนตัวละครซ้าย-ขวา
        transform.Rotate(Vector3.up * mouseX);
    }

    private void RotatePitch()
    {
        float mouseY = lookInput.y * sensitivityY;
        if (!invertY) mouseY *= -1f;

        pitch += mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // หมุนกล้องขึ้น-ลง
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}