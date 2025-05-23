using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public enum Name { Wood, Stone, Scrap, AmmoCrate };

    [Header("Categories")]
    //[SerializeField] Type itemType = Type.Resource;
    [SerializeField] public Name itemName;

    [Space]
    [Header("Dimensions")]
    [SerializeField] public int itemWidth; //we NOT doing Ls
    [SerializeField] public  int itemHeight;
    [SerializeField] public Image img;
    [SerializeField] List<ItemComponent> itemComponents = new List<ItemComponent>();
    [SerializeField] ItemComponent singleComponent;
    [SerializeField] int componentDistance;
    [SerializeField] GridLayoutGroup gridLayout;

    [Space]
    [Header("Inventory")]
    [SerializeField] InventorySlot currentInventorySlot;
    [SerializeField] InventorySlot previousInventorySlot;
    [SerializeField] List<InventorySlot> componentSlots;

    [Space]
    [Header("Connected Objects")]
    [SerializeField] PlayerMovement player;
    [SerializeField] public GameObject parentInventory;
    [SerializeField] public Inventory currentInventory;
    [SerializeField] WorldItem worldItem;

    #region _Item_Menu_Pickup_ 
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
                if (itemIsPickedUpByMouse) //if we right click with item in hand DROP
                {
                    DecreaseStackAmount(1);
                    GameObject dropped = Instantiate(this.gameObject, this.transform.position, this.transform.rotation, parentInventory.transform);
                    dropped.transform.localScale = Vector3.one;
                    Item droppedItem = dropped.GetComponent<Item>();
                    droppedItem.currentInventory = currentInventory;

                    //I need to find where the inventory is hovering and set the correct one.

                    Inventory otherInv = currentInventory;

                    Inventory[] invs = FindObjectsByType<Inventory>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                    foreach (Inventory inv in invs)
                    {
                        if (inv != currentInventory)
                            otherInv = inv;
                    }

                    if (droppedItem.transform.position.x > otherInv.inventoryPanel.gameObject.transform.position.x - otherInv.inventoryPanel.sizeDelta.x / 2
                        && droppedItem.transform.position.x < otherInv.inventoryPanel.gameObject.transform.position.x + otherInv.inventoryPanel.sizeDelta.x / 2
                        && droppedItem.transform.position.y > otherInv.inventoryPanel.gameObject.transform.position.y - otherInv.inventoryPanel.sizeDelta.y / 2
                        && droppedItem.transform.position.y < otherInv.inventoryPanel.gameObject.transform.position.y + otherInv.inventoryPanel.sizeDelta.y / 2)
                    {
                        droppedItem.currentInventory = otherInv;
                    }

                    droppedItem.SetStackAmount(1);
                    droppedItem.UpdateStackText();
                    if (droppedItem.SearchForNearestValidInventorySlot() == 1)
                        droppedItem.SearchAndMoveToNearestInventorySlot();
                    else if (droppedItem.SearchForNearestValidInventorySlot() == 2)
                    {
                        if (player != null)
                        {
                            InstantiateWorldObject(1, false);
                            Destroy(dropped.gameObject);
                        }
                    }
                    else
                    {
                        IncreaseStackAmount(1);
                        Destroy(dropped.gameObject);
                    }
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

                    GameObject dropped = Instantiate(this.gameObject, this.transform.position, this.transform.rotation, parentInventory.transform);
                    dropped.transform.localScale = Vector3.one;
                    Item droppedItem = dropped.GetComponent<Item>();
                    droppedItem.SetStackAmount(equalAmounts);
                    droppedItem.UpdateStackText();
                    droppedItem.SearchAndMoveToNearestInventorySlot();

                    this.ItemComponentSetSiblingLast();
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
        
        
        if (itemIsPickedUpByMouse)
        {
            if (currentInventorySlot != null)
            {
                currentInventorySlot.ClearHeldItem();
                ClearComponentSlots();
                SetParentInventoryObject(null);
                this.ItemComponentSetSiblingLast();
            }
        }
        else
            SearchAndMoveToNearestInventorySlot();

        
    }

    public void MoveToMouse()
    {
        if (itemIsPickedUpByMouse)
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }
    #endregion

    #region _Inventory_
    public int SearchForNearestValidInventorySlot()
    {
        //Search
        InventorySlot[] inventorySlots = FindObjectsByType<InventorySlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (inventorySlots.Length < 1) { return 0; }

        float nearestDistance = 300f;

        if (inventorySlots.Length > 1) //if there is more than one inventory slot on screen, find the closest
        {
            float compareDistance;
            foreach (InventorySlot slot in inventorySlots)
            {
                Vector2 slotPosition = slot.inventoryPosition;
                compareDistance = (slot.transform.position - this.transform.position).magnitude;
                if (compareDistance < nearestDistance)
                {
                    if (!slot.isTrashSlot)
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
                                    if (currentInventory.inventory != null)
                                    {
                                        Item currentItem = slot.parentInventory.inventory[h][w].GetHeldItem();
                                        if (currentItem != null) //if there is something inside the checking cells
                                        {
                                            if (w == (int)slotPosition.x && h == (int)slotPosition.y)
                                            //if (h == (int)startPos.y)
                                            {
                                                if (currentItem.itemName == this.itemName && currentItem.stackAmount < currentItem.stackLimit) //hmmm will need some work
                                                    firstCellisLikeItem = true;
                                            }
                                            else
                                                itemDoesNotCollideWithOtherItems = false;
                                        }
                                    }
                                }
                            }
                        }


                        if (itemDoesFitInsideInventoryFrame && (itemDoesNotCollideWithOtherItems || firstCellisLikeItem))
                        {
                            if (slot.GetHeldItem() == null) //if the slot isnt holding anything
                                nearestDistance = compareDistance;
                            else if (slot.GetHeldItem().itemName == itemName) //if the slot is holding another of the same item
                            {
                                if (slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit && stackAmount < stackLimit) //if 
                                    nearestDistance = compareDistance;
                            }
                        }
                    }
                    else
                        nearestDistance = compareDistance;
                }
            }
        }

        if (nearestDistance > 125f) //if the item is really far away from the inventory slot, probably dont do anything
        {
            Inventory secondInv = currentInventory;

            Inventory[] invs = FindObjectsByType<Inventory>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (Inventory inv in invs)
            {
                if (inv != currentInventory)
                {
                    secondInv = inv;
                }
            }


            if (this.transform.position.x > currentInventory.inventoryPanel.gameObject.transform.position.x - currentInventory.inventoryPanel.sizeDelta.x / 2
                && this.transform.position.x < currentInventory.inventoryPanel.gameObject.transform.position.x + currentInventory.inventoryPanel.sizeDelta.x / 2
                && this.transform.position.y > currentInventory.inventoryPanel.gameObject.transform.position.y - currentInventory.inventoryPanel.sizeDelta.y / 2
                && this.transform.position.y < currentInventory.inventoryPanel.gameObject.transform.position.y + currentInventory.inventoryPanel.sizeDelta.y / 2)
            {

                return 0;
            }
            else if (this.transform.position.x > secondInv.inventoryPanel.gameObject.transform.position.x - secondInv.inventoryPanel.sizeDelta.x / 2
                && this.transform.position.x < secondInv.inventoryPanel.gameObject.transform.position.x + secondInv.inventoryPanel.sizeDelta.x / 2
                && this.transform.position.y > secondInv.inventoryPanel.gameObject.transform.position.y - secondInv.inventoryPanel.sizeDelta.y / 2
                && this.transform.position.y < secondInv.inventoryPanel.gameObject.transform.position.y + secondInv.inventoryPanel.sizeDelta.y / 2)
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }
        else
             return 1;
    }

    public int SearchAndMoveToNearestInventorySlot()
    {
        InventorySlot[] inventorySlots = FindObjectsByType<InventorySlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (inventorySlots.Length < 1) { return -1; }

        InventorySlot nearestInventorySlot = inventorySlots[0];
        float nearestDistance = 300f;

        bool isStacking = false;
        bool isKeepHolding = false;
        bool isTrashing = false;
        Item stackingItem = inventorySlots[0].GetHeldItem();

        int overspillAmount = 0;
        if (inventorySlots.Length > 1) //if there is more than one inventory slot on screen, find the closest
        {
            float compareDistance; 
            foreach (InventorySlot slot in inventorySlots)
            {
                Vector2 slotPosition = slot.inventoryPosition;
                compareDistance = (slot.transform.position - this.transform.position).magnitude;
                if (compareDistance < nearestDistance)
                {
                    if (!slot.isTrashSlot)
                    {
                        bool itemDoesFitInsideInventoryFrame = true; //check if the item size will fit in that spot
                        
                        if (slotPosition.x + itemWidth > slot.parentInventory.inventoryWidth)
                            itemDoesFitInsideInventoryFrame = false;
                        if (slotPosition.y + itemHeight > slot.parentInventory.inventoryHeight)
                            itemDoesFitInsideInventoryFrame = false;

                        bool itemDoesNotCollideWithOtherItems = true;
                        bool firstCellisLikeItem = false;

                        ////check if the item placed in that spot will collide
                        if (itemDoesFitInsideInventoryFrame) //if its not within frame, dont bother
                        {
                            for (int h = (int)slotPosition.y; h < (int)slotPosition.y + itemHeight; h++)
                            {
                                for (int w = (int)slotPosition.x; w < (int)slotPosition.x + itemWidth; w++)
                                {
                                    Item currentItem = slot.parentInventory.inventory[h][w].GetHeldItem();
                                    if (currentItem != null) //if there is something inside the checking cells
                                    {
                                        if (w == (int)slotPosition.x && h == (int)slotPosition.y)
                                        {
                                            if (currentItem.itemName == this.itemName && currentItem.stackAmount < currentItem.stackLimit) //hmmm will need some work
                                                firstCellisLikeItem = true;
                                        }
                                        else
                                            itemDoesNotCollideWithOtherItems = false;
                                    }
                                }
                            }
                        }

                        if (itemDoesFitInsideInventoryFrame && (itemDoesNotCollideWithOtherItems || firstCellisLikeItem))
                        {
                            if (slot.GetHeldItem() == null) //if the slot isnt holding anything
                            {
                                isStacking = false;
                                isKeepHolding = false;
                                isTrashing = false;
                                nearestDistance = compareDistance;
                                nearestInventorySlot = slot;
                                stackingItem = null;
                            }
                            else if (slot.GetHeldItem().itemName == itemName) //if the slot is holding another of the same item
                            {
                                if (slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit && stackAmount < stackLimit) //if 
                                {
                                    if (slot.GetHeldItem().stackAmount + stackAmount > slot.GetHeldItem().stackLimit)
                                    {
                                        isStacking = false;
                                        isKeepHolding = true;
                                        isTrashing = false;
                                        nearestDistance = compareDistance;
                                        stackingItem = slot.GetHeldItem();
                                        overspillAmount = (stackingItem.stackAmount + stackAmount) - stackingItem.stackLimit;
                                        DecreaseStackAmount(stackingItem.stackLimit - stackingItem.stackAmount);
                                        UpdateStackText();

                                        stackingItem.SetStackAmount(stackingItem.stackLimit);
                                        stackingItem.UpdateStackText();
                                    }
                                    else
                                    {
                                        isStacking = true;
                                        isKeepHolding = false;
                                        isTrashing = false;
                                        nearestDistance = compareDistance;
                                        nearestInventorySlot = slot;
                                        stackingItem = slot.GetHeldItem();
                                    }
                                }
                                else if (slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit && stackAmount == stackLimit)
                                {
                                    isStacking = false;
                                    isKeepHolding = true;
                                    isTrashing = false;
                                    nearestDistance = compareDistance;
                                    stackingItem = slot.GetHeldItem();
                                    int tempHold = stackingItem.stackAmount; //2

                                    stackingItem.SetStackAmount(stackAmount);
                                    SetStackAmount(tempHold);
                                    overspillAmount = tempHold;

                                    stackingItem.UpdateStackText();
                                    UpdateStackText();
                                }
                            }
                        }
                    }
                    else //trash slot
                    {
                        isStacking = false;
                        isKeepHolding = false;
                        isTrashing = true;
                        nearestDistance = compareDistance;
                        nearestInventorySlot = slot;
                        stackingItem = null;
                    }
                }
            }
        }

        //Move
        if (nearestDistance > 125f) //if the item is really far away from the inventory slot, probably dont do anything
        {  
            if (this.transform.position.x > currentInventory.inventoryPanel.gameObject.transform.position.x - currentInventory.inventoryPanel.sizeDelta.x / 2 
                && this.transform.position.x < currentInventory.inventoryPanel.gameObject.transform.position.x + currentInventory.inventoryPanel.sizeDelta.x / 2
                && this.transform.position.y > currentInventory.inventoryPanel.gameObject.transform.position.y - currentInventory.inventoryPanel.sizeDelta.y / 2
                && this.transform.position.y < currentInventory.inventoryPanel.gameObject.transform.position.y + currentInventory.inventoryPanel.sizeDelta.y / 2)
            {
                if (previousInventorySlot == null)
                    previousInventorySlot = currentInventory.FindPartialyFilledItemOrEmptySlot(this);

                if (previousInventorySlot != null)
                {
                    if (previousInventorySlot.GetHeldItem() != null)
                    {
                        if (previousInventorySlot.GetHeldItem().itemName == itemName)
                        {
                            stackingItem = previousInventorySlot.GetHeldItem();
                            stackingItem.IncreaseStackAmount(stackAmount);
                            stackingItem.AddComponentSlots(inventorySlots);
                            stackingItem.currentInventorySlot.SetHeldItem(stackingItem);
                            Destroy(this.gameObject);
                        }
                    }
                    else
                    {
                        currentInventorySlot = previousInventorySlot;
                        transform.position = currentInventorySlot.transform.position;
                        currentInventorySlot.currentHeldItem = this; //need to do more with this
                        currentInventorySlot.SetHeldItem(this);
                        AddComponentSlots(inventorySlots);
                    }
                }
            }
            else //outside of the current inventory
            {
                if (player != null)
                    InstantiateWorldObject(stackAmount);
            } 
        }
        else
        {
            currentInventorySlot = nearestInventorySlot;

            if (currentInventorySlot != null)
                previousInventorySlot = currentInventorySlot;

            transform.position = currentInventorySlot.transform.position;
            currentInventorySlot.currentHeldItem = this; //need to do more with this
            currentInventorySlot.SetHeldItem(this);
            currentInventory = currentInventorySlot.parentInventory;
            SetParentInventoryObject(currentInventory.parent);

            if (isTrashing)
            {
                OnDestroy();
                return overspillAmount;
            }
            if (isStacking)
            {
                stackingItem.IncreaseStackAmount(stackAmount);
                stackingItem.AddComponentSlots(inventorySlots);
                stackingItem.currentInventorySlot.SetHeldItem(stackingItem);
                Destroy(this.gameObject);
            }
            else
            {
                if (isKeepHolding)
                    PickUpItemInInventory(); 
                //normally when we stack and we have overspill we pick it back up, but when generating inventories we cant pick it back up
                //i need a feedback loop to return a value to tell the method if we had overspill, and if we did, we reroll a new item with that value
                else
                    AddComponentSlots(inventorySlots);
            }
        }
        return overspillAmount;
    }

    #endregion

    #region _Inventory_Slots_

    public InventorySlot GetCurrentInventorySlot() { return currentInventorySlot; }

    public void AddComponentSlots(InventorySlot[] inventorySlots)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.parentInventory == currentInventory)
            {
                if (slot.inventoryPosition.x >= currentInventorySlot.inventoryPosition.x && slot.inventoryPosition.x < currentInventorySlot.inventoryPosition.x + itemWidth)
                {
                    if (slot.inventoryPosition.y >= currentInventorySlot.inventoryPosition.y && slot.inventoryPosition.y < currentInventorySlot.inventoryPosition.y + itemHeight)
                    {
                        slot.SetHeldItem(this);
                        componentSlots.Add(slot);
                    }
                }
            }
        }
        PlaceDownItem();
    }

    public void ClearComponentSlots()
    {
        foreach (InventorySlot slot in componentSlots)
        {
            if (slot.inventoryPosition.x >= currentInventorySlot.inventoryPosition.x && slot.inventoryPosition.x <= currentInventorySlot.inventoryPosition.x + itemWidth)
            {
                if (slot.inventoryPosition.y >= currentInventorySlot.inventoryPosition.y && slot.inventoryPosition.y <= currentInventorySlot.inventoryPosition.y + itemHeight)
                    slot.ClearHeldItem();
            }
        }
        componentSlots.Clear();
    }

    #endregion

    #region _Stack_
    [Space]
    [SerializeField] public int stackAmount;
    [SerializeField] public int stackLimit;
    public TextMeshProUGUI stackText;

    public void SetStackLimit(int newLimit) { stackLimit = newLimit; UpdateStackText(); }
    public void IncreaseStackAmount(int increase) 
    { 
        if (stackAmount + increase <= stackLimit)
        {
            stackAmount += increase;
            UpdateStackText();
        }
        else
            SetStackAmount(stackLimit);
    }
    public void DecreaseStackAmount(int decrease) 
    {
        if (decrease >= stackAmount)
            stackAmount -= stackLimit;
        else
            stackAmount -= decrease;
        UpdateStackText();
        if (stackAmount <= 0)
            OnDestroy();
    }
    public void SetStackAmount(int amount) 
    { 
        if (amount > stackLimit)
            stackAmount = stackLimit;
        else
            stackAmount = amount; 

        UpdateStackText(); 
    }
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
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = itemWidth;
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
            component.transform.position = new Vector3(component.transform.position.x, component.transform.position.y, 1f);
        }
        this.transform.SetAsLastSibling();
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 1f);
    }

    public void PlaceDownItem()
    {
        foreach (ItemComponent component in itemComponents)
        {
            component.transform.position = new Vector3(component.transform.position.x, component.transform.position.y, -1f);
        }
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1f);
    }


    #endregion

    #region _Other_Objects_

    public void SetParentInventoryObject(GameObject newParent)
    {
        
        if (newParent != null)
        {
            Inventory[] invs = FindObjectsByType<Inventory>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (Inventory inv in invs)
            {
                if (inv.isPlayerInventory)
                    inv.playerQuestBoard.UpdateQuests();
            }
            parentInventory = newParent;
        }
            
        else
            parentInventory = FindAnyObjectByType<PlayerUIToggler>().gameObject;

        transform.SetParent(parentInventory.transform);

        


    }

    public void InstantiateWorldObject(int worldStackAmount, bool isDestroy = true)
    {
        GameObject newDroppedItem = Instantiate(worldItem.gameObject, player.transform.position + new Vector3(0, 2f, 0), player.transform.rotation);
        WorldItem newWorldItem = newDroppedItem.GetComponent<WorldItem>();

        newWorldItem.InitializeWorldObject(worldStackAmount, 0, 0);
        if (isDestroy)
            OnDestroy();
    }

    #endregion


    public void FixText()
    {
        stackText.GetComponent<Canvas>().sortingOrder = 1;
    }

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        currentInventory = FindAnyObjectByType<Inventory>();
        img = GetComponent<Image>();

        SetParentInventoryObject(currentInventory.parent);
        this.gameObject.name = itemName.ToString();
        stackAmount = 1;
        if (itemComponents.Count < (itemWidth * itemHeight) - 1)
            GenerateItem();

        stackText.GetComponent<Canvas>().sortingOrder = 2;
        Invoke("FixText", 0.1f);
    }


    private void Update()
    {
        if (this.gameObject.activeSelf)
            MoveToMouse();
    }

    private void OnDestroy()
    {
        if (currentInventorySlot != null)
        {
            if (currentInventorySlot.GetHeldItem() != null)
            {
                if (currentInventorySlot.GetHeldItem() == this)
                {
                    currentInventorySlot.ClearHeldItem();
                    ClearComponentSlots();
                }
            }
        }

        if (currentInventory.playerQuestBoard != null)
            currentInventory.playerQuestBoard.UpdateQuests();

        Destroy(gameObject);
    }

}
