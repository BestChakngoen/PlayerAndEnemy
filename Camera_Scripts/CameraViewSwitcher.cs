using UnityEngine;
using Unity.Cinemachine;
using PlayerInputs;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private string weaponKeyID = "WeaponKey";

    [Header("Scene Settings")]
    [SerializeField] private string forcedThirdPersonScene = "Act 3 Boss Map";
    [SerializeField] private string[] forcedFirstPersonScenes = { "Act 1 Map", "Act 2 Map" };

    private const int ActivePriority = 20;
    private const int InactivePriority = 10;

    public CinemachineCamera FirstPersonCam => firstPersonCam;
    public CinemachineCamera ThirdPersonCam => thirdPersonCam;

    private void Start()
    {
        ApplyView();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains(forcedThirdPersonScene))
        {
            ForceThirdPersonView();
            return;
        }

        if (forcedFirstPersonScenes != null)
        {
            foreach (string fpScene in forcedFirstPersonScenes)
            {
                if (scene.name.Contains(fpScene))
                {
                    ForceFirstPersonView();
                    break;
                }
            }
        }
    }

    public void SwitchView()
    {
        isFirstPerson = !isFirstPerson;
        ApplyView();
    }

    public void ForceThirdPersonView()
    {
        isFirstPerson = false;
        ApplyView();
    }

    public void ForceFirstPersonView()
    {
        isFirstPerson = true;
        ApplyView();
    }

    public void RefreshView()
    {
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
            bool hasWeapon = true;
            if (playerInventory != null)
            {
                hasWeapon = playerInventory.HasKey(weaponKeyID);
            }

            bool showWeapon = !isFirstPerson && hasWeapon;
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