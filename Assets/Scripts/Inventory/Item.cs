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
    public int itemWidth; //we NOT doing Ls
    public int itemHeight;
    public bool unitIsOrigin = false;
    public List<Item> itemComponents = new List<Item>();

    [Space]
    public InventorySlot currentInventorySlot;
    public GameObject parent;
    #region _Item_Pickup_in_Menu_
    private bool itemIsPickedUpByMouse;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            if (!unitIsOrigin)
            {
                Item origin = this;
                foreach (Item component in itemComponents)
                {
                    if (component.unitIsOrigin)
                    {
                        origin = component;
                    }
                }
                origin.PickUpItemInInventory();
            }
            else
            {
                PickUpItemInInventory();
            }
        }
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            if (!unitIsOrigin)
            {
                Item origin = this;
                foreach (Item component in itemComponents)
                {
                    if (component.unitIsOrigin)
                    {
                        origin = component;
                    }
                }
                
                if (stackAmount > 1)
                {
                    if (itemIsPickedUpByMouse) //if we right click with item in hand
                    {
                        origin.DecreaseStackAmount(1);
                        origin.UpdateStackText();
                        GameObject dropped = Instantiate(origin.gameObject, origin.transform.position, origin.transform.rotation, parent.transform);
                        dropped.transform.localScale = Vector3.one;
                        Item droppedItem = dropped.GetComponent<Item>();
                        droppedItem.SetStackAmount(1);
                        droppedItem.UpdateStackText();
                        if (droppedItem.SearchForNearestValidInventorySlot())
                            droppedItem.SearchAndMoveToNearestInventorySlot();
                        origin.ItemComponentSetSiblingLast();
                    }
                    else //if we right click without item in hand
                    {
                        int equalAmounts = stackAmount / 2;
                        if (stackAmount % 2 == 1) //Handles odd numbers
                            origin.SetStackAmount(equalAmounts + 1);
                        else
                            origin.SetStackAmount(equalAmounts);
                        origin.UpdateStackText();

                        origin.PickUpItemInInventory();

                        GameObject dropped = Instantiate(origin.gameObject, origin.transform.position, origin.transform.rotation, parent.transform);
                        dropped.transform.localScale = Vector3.one;
                        Item droppedItem = dropped.GetComponent<Item>();
                        droppedItem.SetStackAmount(equalAmounts);
                        droppedItem.UpdateStackText();
                        droppedItem.SearchAndMoveToNearestInventorySlot();

                        origin.ItemComponentSetSiblingLast();

                        Debug.Log("uhhhh");
                    }
                }
                else
                {
                    origin.PickUpItemInInventory();
                }
            }
            else
            {
                if (stackAmount > 1)
                {
                    if (itemIsPickedUpByMouse) //if we right click with item in hand
                    {
                        DecreaseStackAmount(1);
                        UpdateStackText();
                        GameObject dropped = Instantiate(this.gameObject, this.transform.position, this.transform.rotation, parent.transform);
                        dropped.transform.localScale = Vector3.one;
                        Item droppedItem = dropped.GetComponent<Item>();
                        droppedItem.SetStackAmount(1);
                        droppedItem.UpdateStackText();
                        if (droppedItem.SearchForNearestValidInventorySlot())
                            droppedItem.SearchAndMoveToNearestInventorySlot();
                        this.ItemComponentSetSiblingLast();
                    }
                    else //if we right click without item in hand
                    {
                        int equalAmounts = stackAmount / 2;
                        if (stackAmount % 2 == 1) //Handles odd numbers
                            SetStackAmount(equalAmounts + 1);
                        else
                            SetStackAmount(equalAmounts);
                        UpdateStackText();

                        PickUpItemInInventory();

                        GameObject dropped = Instantiate(this.gameObject, this.transform.position, this.transform.rotation, parent.transform);
                        dropped.transform.localScale = Vector3.one;
                        Item droppedItem = dropped.GetComponent<Item>();
                        droppedItem.SetStackAmount(equalAmounts);
                        droppedItem.UpdateStackText();
                        droppedItem.SearchAndMoveToNearestInventorySlot();

                        this.ItemComponentSetSiblingLast();

                        Debug.Log("uhhhh");
                    }
                }
                else
                {
                    PickUpItemInInventory();
                }
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

        this.ItemComponentSetSiblingLast();

        if (itemIsPickedUpByMouse)
        {
            if (currentInventorySlot != null)
            {
                currentInventorySlot.ClearHeldItem();
                Debug.Log("Clear Item");
            }
                ;
        }
        else
            SearchAndMoveToNearestInventorySlot();
    }

    public void MoveToMouse()
    {
        if (itemIsPickedUpByMouse)
            transform.position = Input.mousePosition;
    }
    #endregion

    #region _Inventory_
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
        float nearestDistance = 200f;////

        bool isStacking = false;///
        bool isKeepHolding = false;
        Item stackingItem = inventorySlots[0].GetHeldItem();///

        if (inventorySlots.Length > 1) //if there is more than one inventory slot on screen, find the closest
        {
            float compareDistance;
            foreach (InventorySlot slot in inventorySlots)
            { 
                compareDistance = (slot.transform.position - this.transform.position).magnitude;
                if (compareDistance < nearestDistance)
                {
                    if (slot.GetHeldItem() == null) //if the slot isnt holding anything
                    {
                        isStacking = false;
                        isKeepHolding = false;
                        nearestDistance = compareDistance;
                        nearestInventorySlot = slot;
                        stackingItem = null;
                        Debug.Log("empty " + nearestDistance);
                    }
                    else if (slot.GetHeldItem().itemName == itemName) //if the slot is holding another of the same item
                    {  
                        if (slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit && stackAmount < stackLimit) //if 
                        {
                            if (slot.GetHeldItem().stackAmount + stackAmount > slot.GetHeldItem().stackLimit)
                            {
                                //if the item we are holding is not full and we are wanting to merge into a not full item,
                                //and merging the items would cause the stack to excede the limit
                                //we transfer some into the item on the ground, but dont stack, keep holding onto the item
                                isStacking = false;
                                isKeepHolding = true;
                                stackingItem = slot.GetHeldItem();

                                DecreaseStackAmount(stackingItem.stackLimit - stackingItem.stackAmount);
                                UpdateStackText();

                                stackingItem.SetStackAmount(stackingItem.stackLimit);
                                stackingItem.UpdateStackText();
                                Debug.Log("top up " + nearestDistance);
                            }
                            else
                            {
                                isStacking = true;
                                isKeepHolding = false;
                                nearestDistance = compareDistance;
                                nearestInventorySlot = slot;
                                stackingItem = slot.GetHeldItem();
                                Debug.Log("merge " + nearestDistance);
                            }
                            
                        }
                        else if (slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit && stackAmount == stackLimit)
                        {
                            //if the item we are holding is full and we are wanting to merge into a not full item,
                            //swap the values


                            isStacking = false;
                            isKeepHolding = true;
                            stackingItem = slot.GetHeldItem();
                            int tempHold = stackingItem.stackAmount;

                            stackingItem.SetStackAmount(stackAmount);
                            SetStackAmount(tempHold);

                            stackingItem.UpdateStackText();
                            UpdateStackText();

                            Debug.Log("swap" + nearestDistance);
                        }
                            
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
            currentInventorySlot.SetHeldItem(this);

            if (isStacking)
            {
                Debug.Log("au haifliausdkjfn");
                stackingItem.IncreaseStackAmount(stackAmount);
                stackingItem.UpdateStackText();
                currentInventorySlot.SetHeldItem(stackingItem);
                Destroy(this.gameObject);
            }
            else if (isKeepHolding)
            {
                PickUpItemInInventory();
            }
        }
    }
    #endregion

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


    #region _Size_

    public void GenerateItem()
    {
        for (int y = 0; y < itemHeight; y++)
        {
            for (int x = 0; x < itemWidth; x++)
            {
                Vector3 positionRelativeToOrigin = this.transform.position + new Vector3(50 * y, 50 * x, transform.position.z);
                GameObject itemComponentGameObject = Instantiate(this.gameObject, positionRelativeToOrigin, transform.rotation, transform);
                Item itemComponentItem = itemComponentGameObject.GetComponent<Item>();
                itemComponentItem.unitIsOrigin = false;
            }
        }
    }

    public void ItemComponentSetSiblingLast()
    {
        Item origin = this;
        foreach (Item component in itemComponents)
        {
            component.transform.SetAsLastSibling();
        }
        origin.transform.SetAsLastSibling();
    }


    #endregion

    //TODO: Add item decrease / item increase methods (maybe in the stack)

    private void Awake()
    {
        this.gameObject.name = itemName.ToString();
        parent = GetComponentInParent<Canvas>().gameObject; //huh i
        stackAmount = 1;
        SetStackLimit(5);
        UpdateStackText();
        if (unitIsOrigin)
        {
            GenerateItem();
        }
    }


    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            MoveToMouse();
        }
    }

}
