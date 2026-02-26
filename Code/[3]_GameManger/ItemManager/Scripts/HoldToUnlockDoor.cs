using UnityEngine;

[RequireComponent(typeof(Door))]
public class HoldToUnlockDoor : MonoBehaviour, IInteractable, IHoldInteractable
{
    [Header("Lock")]
    public bool isLocked = true;
    public string requiredKeyID;
    public bool RequiresHold => isLocked;

    [Header("Hold")]
    public float holdDuration = 2f;

    Door door;
    DoorProgressUI progressUI;

    float holdTimer;
    bool isHolding;
    GameObject interactor;

    void Awake()
    {
        door = GetComponent<Door>();
        progressUI = GetComponentInChildren<DoorProgressUI>();
    }

    // ===== Interact ปกติ (กรณีไม่ล็อค) =====
    public void Interact(GameObject actor)
    {
        if (!isLocked)
            door.Toggle();
    }
    
    // ===== Hold System =====
    public void BeginHold(GameObject actor)
    {
        if (!isLocked) return;

        Inventory inv = actor.GetComponentInParent<Inventory>();
        if (inv == null || !inv.HasKey(requiredKeyID))
        {
            Debug.Log("ไม่มีกุญแจ");
            return;
        }

        interactor = actor;
        holdTimer = 0f;
        isHolding = true;
        progressUI?.Show();
    }

    public void EndHold(GameObject actor)
    {
        if (!isHolding) return;

        isHolding = false;
        holdTimer = 0f;
        progressUI?.Hide();
    }

    void Update()
    {
        if (!isHolding) return;

        holdTimer += Time.deltaTime;
        progressUI?.UpdateProgress(holdTimer / holdDuration);

        if (holdTimer >= holdDuration)
            Unlock();
    }

    void Unlock()
    {
        Inventory inv = interactor.GetComponentInParent<Inventory>();
        if (!inv.RemoveKey(requiredKeyID)) return;

        isLocked = false;
        isHolding = false;
        progressUI?.Hide();

        door.Toggle();
    }
}