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
    public enum Name { Wood, Steel};
    public Name itemName = Name.Wood;
    [Space]
    public int itemWidth; //we NOT doing Ls
    public int itemHeight;
    public List<ItemComponent> itemComponents = new List<ItemComponent>();
    public ItemComponent singleComponent;
    public Image img;
    public int componentDistance;

    [Space]
    public InventorySlot currentInventorySlot;
    public InventorySlot previousInventorySlot;
    public List<InventorySlot> componentSlots;
    public GameObject parent;

    [Space]
    public Inventory currentInventory;
    #region _Item_Pickup_in_Menu_
    private bool itemIsPickedUpByMouse;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            PickUpItemInInventory();
            //if (!unitIsOrigin)
            //{
            //    Item origin = this;
            //    foreach (Item component in itemComponents)
            //    {
            //        if (component.unitIsOrigin)
            //        {
            //            origin = component;
            //        }
            //    }
            //    origin.PickUpItemInInventory();
            //}
            //else
            //{

            //}
        }
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            if (stackAmount > 1)
            {
                if (itemIsPickedUpByMouse) //if we right click with item in hand DROP
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
                else //if we right click without item in hand SPLIT
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

                    //Debug.Log("uhhhh");
                }
            }
            else
            {
                PickUpItemInInventory();
            }
            //if (!unitIsOrigin)
            //{
            //    Item origin = this;
            //    foreach (Item component in itemComponents)
            //    {
            //        if (component.unitIsOrigin)
            //        {
            //            origin = component;
            //        }
            //    }

            //    if (stackAmount > 1)
            //    {
            //        if (itemIsPickedUpByMouse) //if we right click with item in hand
            //        {
            //            origin.DecreaseStackAmount(1);
            //            origin.UpdateStackText();
            //            GameObject dropped = Instantiate(origin.gameObject, origin.transform.position, origin.transform.rotation, parent.transform);
            //            dropped.transform.localScale = Vector3.one;
            //            Item droppedItem = dropped.GetComponent<Item>();
            //            droppedItem.SetStackAmount(1);
            //            droppedItem.UpdateStackText();
            //            if (droppedItem.SearchForNearestValidInventorySlot())
            //                droppedItem.SearchAndMoveToNearestInventorySlot();
            //            origin.ItemComponentSetSiblingLast();
            //        }
            //        else //if we right click without item in hand
            //        {
            //            int equalAmounts = stackAmount / 2;
            //            if (stackAmount % 2 == 1) //Handles odd numbers
            //                origin.SetStackAmount(equalAmounts + 1);
            //            else
            //                origin.SetStackAmount(equalAmounts);
            //            origin.UpdateStackText();

            //            origin.PickUpItemInInventory();

            //            GameObject dropped = Instantiate(origin.gameObject, origin.transform.position, origin.transform.rotation, parent.transform);
            //            dropped.transform.localScale = Vector3.one;
            //            Item droppedItem = dropped.GetComponent<Item>();
            //            droppedItem.SetStackAmount(equalAmounts);
            //            droppedItem.UpdateStackText();
            //            droppedItem.SearchAndMoveToNearestInventorySlot();

            //            origin.ItemComponentSetSiblingLast();

            //            Debug.Log("uhhhh");
            //        }
            //    }
            //    else
            //    {
            //        origin.PickUpItemInInventory();
            //    }
            //}
            //else
            //{

            //}



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
                //Debug.Log("Clear Item");
                foreach (InventorySlot slot in componentSlots)
                {
                    if (slot.inventoryPosition.x >= currentInventorySlot.inventoryPosition.x && slot.inventoryPosition.x <= currentInventorySlot.inventoryPosition.x + itemWidth)
                    {
                        if (slot.inventoryPosition.y >= currentInventorySlot.inventoryPosition.y && slot.inventoryPosition.y <= currentInventorySlot.inventoryPosition.y + itemWidth)
                        {
                            slot.ClearHeldItem();
                        }
                    }
                }
                componentSlots.Clear();
            }
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
        float nearestDistance = 300f;////

        bool isStacking = false;///
        bool isKeepHolding = false;
        Item stackingItem = inventorySlots[0].GetHeldItem();///

        if (inventorySlots.Length > 1) //if there is more than one inventory slot on screen, find the closest
        {
            float compareDistance;
            foreach (InventorySlot slot in inventorySlots)
            {
                Vector2 slotPosition = slot.inventoryPosition;
                compareDistance = (slot.transform.position - this.transform.position).magnitude;
                if (compareDistance < nearestDistance)
                {


                    bool itemDoesFitInsideInventoryFrame = true; //check if the item size will fit in that spot

                    if (slotPosition.x + itemWidth > currentInventory.inventoryWidth)
                    {
                        itemDoesFitInsideInventoryFrame = false;
                    }
                    if (slotPosition.y + itemHeight > currentInventory.inventoryHeight)
                    {
                        itemDoesFitInsideInventoryFrame = false;
                    }

                    bool itemDoesNotCollideWithOtherItems = true;
                    bool firstCellisLikeItem = false;

                    ////check if the item placed in that spot will collide
                    if (itemDoesFitInsideInventoryFrame) //if its not within frame, dont bother
                    {
                        for (int h = (int)slotPosition.y; h < (int)slotPosition.y + itemHeight; h++)
                        {
                            for (int w = (int)slotPosition.x; w < (int)slotPosition.x + itemWidth; w++)
                            {
                                if (currentInventory.inventory[h][w].GetHeldItem() != null) //if there is something inside the checking cells
                                {


                                    if (w == (int)slotPosition.x && h == (int)slotPosition.y)
                                    //if (h == (int)startPos.y)
                                    {

                                        if (currentInventory.inventory[h][w].GetHeldItem().itemName == this.itemName)
                                        {
                                            Debug.Log("h: " + h + " | w: " + w);
                                            Debug.Log("item name in top left cell" + currentInventory.inventory[h][w]);
                                            firstCellisLikeItem = true;
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("item Collision");
                                        itemDoesNotCollideWithOtherItems = false;
                                    }
                                }

                            }
                            
                        }
                    }

                    //for (int w = (int)startPos.x; w < (int)startPos.x + itemWidth; w++)
                    //{
                      
                     
                    //}



                    if (itemDoesFitInsideInventoryFrame && (itemDoesNotCollideWithOtherItems || firstCellisLikeItem))
                    {
                        //Debug.Log("itemDoesFitInsideInventoryFrame: " + itemDoesFitInsideInventoryFrame);
                        Debug.Log("itemDoesNotCollideWithOtherItems: " + itemDoesNotCollideWithOtherItems);
                        Debug.Log("firstCellisLikeItem: " + firstCellisLikeItem);
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
                                    isStacking = false;
                                    isKeepHolding = true;
                                    stackingItem = slot.GetHeldItem();

                                    DecreaseStackAmount(stackingItem.stackLimit - stackingItem.stackAmount);
                                    UpdateStackText();

                                    stackingItem.SetStackAmount(stackingItem.stackLimit);
                                    stackingItem.UpdateStackText();
                                    //Debug.Log("top up " + nearestDistance);
                                }
                                else
                                {
                                    isStacking = true;
                                    isKeepHolding = false;
                                    nearestDistance = compareDistance;
                                    nearestInventorySlot = slot;
                                    stackingItem = slot.GetHeldItem();
                                    //Debug.Log("merge " + nearestDistance);
                                }

                            }
                            else if (slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit && stackAmount == stackLimit)
                            {
                                isStacking = false;
                                isKeepHolding = true;
                                stackingItem = slot.GetHeldItem();
                                int tempHold = stackingItem.stackAmount;

                                stackingItem.SetStackAmount(stackAmount);
                                SetStackAmount(tempHold);

                                stackingItem.UpdateStackText();
                                UpdateStackText();

                                //Debug.Log("swap" + nearestDistance);
                            }
                        }
                    }
                }
            }
        }

        //Move
        if (nearestDistance > 200f)// * ( itemHeight > itemWidth ? itemHeight : itemWidth)) //if the item is really far away from the inventory slot, probably dont do anything
        {
            //Debug.Log("Previous");
            if (previousInventorySlot == null)
                return;
            currentInventorySlot = previousInventorySlot;
            transform.position = currentInventorySlot.transform.position;
            currentInventorySlot.currentHeldItem = this; //need to do more with this
            currentInventorySlot.SetHeldItem(this);

            AddComponentSlots(inventorySlots);
        }
        else
        {
            //Debug.Log("next");
            currentInventorySlot = nearestInventorySlot;
            if (currentInventorySlot != null)
                previousInventorySlot = currentInventorySlot;
            transform.position = currentInventorySlot.transform.position;
            currentInventorySlot.currentHeldItem = this; //need to do more with this
            currentInventorySlot.SetHeldItem(this);
            Debug.Log(currentInventorySlot.inventoryPosition);


            AddComponentSlots(inventorySlots);


            if (isStacking)
            {
                //Debug.Log("au haifliausdkjfn");
                stackingItem.IncreaseStackAmount(stackAmount);
                stackingItem.UpdateStackText();
                stackingItem.currentInventorySlot.SetHeldItem(stackingItem);

                //stackingItem.AddComponentSlots(inventorySlots);

                Destroy(this.gameObject);
            }
            else if (isKeepHolding)
            {
                PickUpItemInInventory();
            }
        }
    }

    public void AddComponentSlots(InventorySlot[] inventorySlots)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.inventoryPosition.x >= currentInventorySlot.inventoryPosition.x && slot.inventoryPosition.x < currentInventorySlot.inventoryPosition.x + itemWidth)
            {
                if (slot.inventoryPosition.y >= currentInventorySlot.inventoryPosition.y && slot.inventoryPosition.y < currentInventorySlot.inventoryPosition.y + itemWidth)
                {
                    slot.SetHeldItem(this);
                    componentSlots.Add(slot);
                    Debug.Log("component slot added");
                }
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
        Debug.Log("generate");
        for (int y = 0; y < itemHeight; y++)
        {
            for (int x = 0; x < itemWidth; x++)
            {
                if (!(y == 0 && x == 0))
                {
                    Vector3 positionRelativeToOrigin = this.transform.position + new Vector3(componentDistance * x, componentDistance * -y, transform.position.z);
                    GameObject generatedItemGameObject = Instantiate(singleComponent.gameObject, positionRelativeToOrigin, transform.rotation, transform);
                    ItemComponent generatedItemComponent = generatedItemGameObject.GetComponent<ItemComponent>();
                    generatedItemComponent.originalItem = this;
                    generatedItemComponent.SetImage();
                    itemComponents.Add(generatedItemComponent);
                }
            }
        }
    }

    public void ItemComponentSetSiblingLast()
    {
        foreach (ItemComponent component in itemComponents)
        {
            component.transform.SetAsLastSibling();
        }
        this.transform.SetAsLastSibling();
    }


    #endregion

    //TODO: Add item decrease / item increase methods (maybe in the stack)

    private void Awake()
    {
        this.gameObject.name = itemName.ToString();
        currentInventory = FindAnyObjectByType<Inventory>();
        parent = GetComponentInParent<Canvas>().gameObject; //huh i
        img = GetComponent<Image>();
        stackAmount = 1;
        SetStackLimit(5);
        UpdateStackText();
        GenerateItem();
    }


    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            MoveToMouse();
        }
    }

}
