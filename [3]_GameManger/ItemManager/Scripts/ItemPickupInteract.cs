using UnityEngine;

public class ItemPickupInteract : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    public void Interact(GameObject interactor)
    {
        Inventory inventory = interactor.GetComponentInParent<Inventory>();
        if (inventory == null) return;

        inventory.AddItem(itemData);
        Destroy(gameObject);
    }
}