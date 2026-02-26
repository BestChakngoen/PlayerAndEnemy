using UnityEngine;
using Unity.Cinemachine;
using PlayerInputs;

public class CameraViewSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    [Header("Dependencies")]
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private FPSMouseLook _mouseLook;

    [Header("State")]
    [SerializeField] private bool isFirstPerson = false;

    private const int ActivePriority = 20;
    private const int InactivePriority = 10;

    private void Start()
    {
        ApplyView();
    }

    public void SwitchView()
    {
        isFirstPerson = !isFirstPerson;
        ApplyView();
    }

    private void ApplyView()
    {
        if (isFirstPerson)
        {
            firstPersonCam.Priority = ActivePriority;
            thirdPersonCam.Priority = InactivePriority;

            movementController.SetMovementMode(PlayerMovementController.MovementMode.FirstPerson);
            _mouseLook.SetCameraMode(FPSMouseLook.CameraMode.FirstPerson);
            
        }
        else
        {
            firstPersonCam.Priority = InactivePriority;
            thirdPersonCam.Priority = ActivePriority;

            movementController.SetMovementMode(PlayerMovementController.MovementMode.ThirdPerson);
            _mouseLook.SetCameraMode(FPSMouseLook.CameraMode.ThirdPerson);
        }
    }

    public bool IsFirstPerson => isFirstPerson;
}