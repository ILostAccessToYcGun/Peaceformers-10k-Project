using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{
    //This is the base Item class, all items will derrive from this class

    public enum Type { Resource, Weapon, Ammunition, Supplies };
    public Type itemType = Type.Resource;
    [Space]
    public List<List<bool>> itemShape = new List<List<bool>>();
    public List<Item> itemComponents = new List<Item>();
    public bool unitIsOrigin = false;
    [Space]
    public int stackAmount;
    public int stackLimit;

    #region _Item_Pickup_in_Menu_

    private bool itemIsPickedUpByMouse;
    public void OnMouseDown()
    {
        itemIsPickedUpByMouse = !itemIsPickedUpByMouse;
        if (!itemIsPickedUpByMouse)
        {
            SearchAndMoveToNearestInventorySlot();
        }
    }
    public void MoveToMouse()
    {
        if (itemIsPickedUpByMouse)
            transform.position = Input.mousePosition;
    }

    #endregion

    public void SearchAndMoveToNearestInventorySlot()
    {
        //Search
        InventorySlot[] inventorySlots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
        if (inventorySlots.Length < 1) { return; }

        InventorySlot nearestInventorySlot = inventorySlots[0];
        float nearestDistance = (inventorySlots[0].transform.position - transform.position).magnitude;

        if (inventorySlots.Length > 1) //if there is more than one inventory slot on screen, find the closest
        {
            float compareDistance;
            foreach (InventorySlot slot in inventorySlots)
            {
                compareDistance = (slot.transform.position - transform.position).magnitude;
                if (compareDistance < nearestDistance)
                {
                    nearestDistance = compareDistance;
                    nearestInventorySlot = slot;
                }
            }
        }

        //Move
        if (nearestDistance > 60f) //if the item is really far away from the inventory slot, probably dont do anything
            Debug.Log(nearestDistance);
        else
        {
            transform.position = nearestInventorySlot.transform.position;
            nearestInventorySlot.isOccupied = true; //need to do more with this
        }

        

    }

    public void GenerateItemShape()
    {

    }

    private void Awake()
    {
        unitIsOrigin = true;
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            MoveToMouse();
        }
    }

}
