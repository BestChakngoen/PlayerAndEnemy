using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlot = 10;
    public List<ItemData> items = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        if (items.Count >= maxSlot)
        {
            Debug.Log("Inventory เต็ม");
            return;
        }

        items.Add(item);
        InventoryUI.Instance.Refresh(items);
    }
    public bool HasKey(string keyID)
    {
        foreach (var item in items)
        {
            if (item is KeyItemData key && key.keyID == keyID)
                return true;
        }
        return false;
    }
    public bool RemoveKey(string keyID)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] is KeyItemData key && key.keyID == keyID)
            {
                items.RemoveAt(i);
                InventoryUI.Instance.Refresh(items);
                return true;
            }
        }
        return false;
    }

    

}