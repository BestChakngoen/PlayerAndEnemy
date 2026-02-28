using UnityEngine;
using Unity.Cinemachine;
using PlayerInputs;

public class CameraViewSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private FPSMouseLook mouseLook;
    [SerializeField] private bool isFirstPerson = false;

    [Header("UI Settings")]
    [SerializeField] private GameObject[] playerUIElements;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject[] playerWeapons;

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
        if (firstPersonCam != null)
            firstPersonCam.Priority = isFirstPerson ? ActivePriority : InactivePriority;

        if (thirdPersonCam != null)
            thirdPersonCam.Priority = isFirstPerson ? InactivePriority : ActivePriority;

        if (movementController != null)
            movementController.SetMovementMode(isFirstPerson ? PlayerMovementController.MovementMode.FirstPerson : PlayerMovementController.MovementMode.ThirdPerson);

        if (mouseLook != null)
            mouseLook.SetCameraMode(isFirstPerson ? FPSMouseLook.CameraMode.FirstPerson : FPSMouseLook.CameraMode.ThirdPerson);

        if (playerUIElements != null)
        {
            bool showUI = !isFirstPerson;
            foreach (var ui in playerUIElements)
            {
                if (ui != null)
                {
                    ui.SetActive(showUI);
                }
            }
        }

        if (playerWeapons != null)
        {
            bool showWeapon = !isFirstPerson;
            foreach (var weapon in playerWeapons)
            {
                if (weapon != null)
                {
                    weapon.SetActive(showWeapon);
                }
            }
        }
    }

    public bool IsFirstPerson => isFirstPerson;
}