using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public Item currentHeldItem = null;

    public Item GetHeldItem() { return currentHeldItem; }
    public void SetHeldItem(Item item)
    {
        currentHeldItem = item;
    }
    public void ClearHeldItem()
    {
        currentHeldItem = null;
    }

}
