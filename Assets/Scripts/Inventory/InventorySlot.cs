using UnityEngine;
//using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour
{

    public Inventory parentInventory;
    public Item currentHeldItem = null;
    public Vector2 inventoryPosition;
    public bool isTrashSlot = false;

    public void SetInventoryPosition(Vector2 pos)
    {
        inventoryPosition = pos;
    }

    public Item GetHeldItem() { return currentHeldItem; }
    public void SetHeldItem(Item item)
    {
        currentHeldItem = item;
        //parentInventory.inventory[(int)inventoryPosition.x][(int)inventoryPosition.y].SetHeldItem(item);
    }
    public void ClearHeldItem()
    {
        currentHeldItem = null;
        //parentInventory.inventory[(int)inventoryPosition.x][(int)inventoryPosition.y] = null ;
    }
    private void Awake()
    {
        //parentInventory = FindAnyObjectByType<Inventory>();
    }
}
