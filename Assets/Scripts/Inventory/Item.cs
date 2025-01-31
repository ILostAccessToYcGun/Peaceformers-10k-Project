using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerClickHandler
{
    //This is the base Item class, all items will derrive from this class

    public enum Type { Resource, Weapon, Ammunition, Supplies };
    public Type itemType = Type.Resource;
    public enum Name { Wood };
    public Name itemName = Name.Wood;
    [Space]
    public List<List<bool>> itemShape;
    public List<Item> itemComponents;
    public bool unitIsOrigin = false;
    
    [Space]
    public InventorySlot currentInventorySlot;
    public GameObject parent;
    #region _Item_Pickup_in_Menu_

    private bool itemIsPickedUpByMouse;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            PickUpItemInInventory();
        }
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            if (stackAmount > 1)
            {
                if (itemIsPickedUpByMouse) //if we right click with item in hand
                {
                    DecreaseStackAmount(1);
                    UpdateStackText();
                    GameObject dropped = Instantiate(this.gameObject);
                    Item droppedItem = dropped.GetComponent<Item>();
                    droppedItem.SetStackAmount(1);
                    droppedItem.UpdateStackText();
                    if (droppedItem.SearchForNearestValidInventorySlot())
                    {
                        droppedItem.SearchAndMoveToNearestInventorySlot();
                    }
                }
                else //if we right click without item in hand
                {
                    int equalAmounts = stackAmount / 2;
                    if (stackAmount % 2 == 1) //Handles odd numbers
                        SetStackAmount(equalAmounts + 1);
                    else
                        SetStackAmount(equalAmounts);
                    UpdateStackText();

                    GameObject dropped = Instantiate(this.gameObject, this.transform.position, this.transform.rotation, parent.transform);
                    dropped.transform.localScale = Vector3.one;
                    Item droppedItem = dropped.GetComponent<Item>();
                    droppedItem.SetStackAmount(equalAmounts);
                    droppedItem.UpdateStackText();

                    PickUpItemInInventory();
                }
            }
            else
            {
                PickUpItemInInventory();
            }
            
        }
    }

    public void PickUpItemInInventory()
    {
        //system will prioritize releasing items over picking new ones up
        Item[] items = FindObjectsByType<Item>(FindObjectsSortMode.None);
        foreach (Item item in items)
        {
            if (item != this)
            {
                if (item.itemIsPickedUpByMouse)
                    return;
            }
        }
        itemIsPickedUpByMouse = !itemIsPickedUpByMouse;

        this.transform.SetAsLastSibling();

        if (itemIsPickedUpByMouse)
        {
            if (currentInventorySlot != null)
                currentInventorySlot.ClearHeldItem();
        }
        else
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


    public bool SearchForNearestValidInventorySlot()
    {
        //Search
        InventorySlot[] inventorySlots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
        if (inventorySlots.Length < 1) { return false; }

        float nearestDistance = (inventorySlots[0].transform.position - transform.position).magnitude;////

        if (inventorySlots.Length > 1) //if there is more than one inventory slot on screen, find the closest
        {
            float compareDistance;
            foreach (InventorySlot slot in inventorySlots)
            {
                compareDistance = (slot.transform.position - transform.position).magnitude;
                if (compareDistance < nearestDistance)
                {
                    if (slot.GetHeldItem() == null || slot.GetHeldItem().itemName == itemName)
                        nearestDistance = compareDistance;
                }
            }
        }

        if (nearestDistance > 100f) //if the item is really far away from the inventory slot, probably dont do anything
            return false;
        else
        {
            return true;
        }
    }

    public void SearchAndMoveToNearestInventorySlot()
    {
        //Search
        InventorySlot[] inventorySlots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
        if (inventorySlots.Length < 1) { return; }

        InventorySlot nearestInventorySlot = inventorySlots[0]; ////
        float nearestDistance = (inventorySlots[0].transform.position - transform.position).magnitude;////

        bool isStacking = false;///
        Item stackingItem = inventorySlots[0].GetHeldItem();///

        if (inventorySlots.Length > 1) //if there is more than one inventory slot on screen, find the closest
        {
            float compareDistance;
            foreach (InventorySlot slot in inventorySlots)
            {
                compareDistance = (slot.transform.position - transform.position).magnitude;
                if (compareDistance < nearestDistance)
                {
                    if (slot.GetHeldItem() == null)
                    {
                        Debug.Log("hur");
                        nearestDistance = compareDistance;
                        nearestInventorySlot = slot;
                    }
                    else if (slot.GetHeldItem().itemName == itemName)
                    {
                        Debug.Log("mergeee");
                        nearestDistance = compareDistance;
                        nearestInventorySlot = slot;
                        isStacking = true;
                        stackingItem = slot.GetHeldItem();
                    }
                }
            }
        }

        //Move
        if (nearestDistance > 100f) //if the item is really far away from the inventory slot, probably dont do anything
            return;
        else
        {
            
            currentInventorySlot = nearestInventorySlot;
            transform.position = currentInventorySlot.transform.position;
            currentInventorySlot.currentHeldItem = this; //need to do more with this

            if (isStacking)
            {
                stackingItem.IncreaseStackAmount(stackAmount);
                stackingItem.UpdateStackText();
                Destroy(this.gameObject);
            }
        }
    }

    #region _Stack_
    [Space]
    public int stackAmount;
    public int stackLimit = 5;
    public TextMeshProUGUI stackText;

    public void SetStackLimit(int newLimit) { stackLimit = newLimit; }
    public void IncreaseStackAmount(int increase) { stackAmount += increase; }
    public void DecreaseStackAmount(int decrease) { stackAmount -= decrease; }
    public void SetStackAmount(int amount) { stackAmount = amount; }
    public void UpdateStackText()
    {
        stackText.text = stackAmount.ToString();
        if (stackAmount == stackLimit)
            stackText.color = Color.red;
        else
            stackText.color = Color.black;
    }
    #endregion


    private void Awake()
    {
        parent = GetComponentInParent<Canvas>().gameObject; //huh i
        stackAmount = 1;
        SetStackLimit(5);
        unitIsOrigin = true;
        UpdateStackText();
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            MoveToMouse();
        }
    }

}
