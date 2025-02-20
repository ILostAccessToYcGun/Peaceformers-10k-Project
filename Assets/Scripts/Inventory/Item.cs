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
    public enum Name { Wood, Stone, Scrap };

    [Header("Categories")]
    [SerializeField] Type itemType = Type.Resource;
    [SerializeField] public Name itemName;

    [Space]
    [Header("Dimensions")]
    [SerializeField] public int itemWidth; //we NOT doing Ls
    [SerializeField] public  int itemHeight;
    [SerializeField] List<ItemComponent> itemComponents = new List<ItemComponent>();
    [SerializeField] ItemComponent singleComponent;
    [SerializeField] public Image img;
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
    [SerializeField] public GameObject parent;
    [SerializeField] Inventory currentInventory;
    [SerializeField] WorldItem worldItem;
    
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
                if (itemIsPickedUpByMouse) //if we right click with item in hand DROP
                {
                    DecreaseStackAmount(1);
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
        this.ItemComponentSetSiblingLast();
        if (itemIsPickedUpByMouse)
        {
            if (currentInventorySlot != null)
            {
                currentInventorySlot.ClearHeldItem();
                ClearComponentSlots();
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
                                    Item currentItem = currentInventory.inventory[h][w].GetHeldItem();
                                    if (currentItem != null) //if there is something inside the checking cells
                                    {
                                        if (w == (int)slotPosition.x && h == (int)slotPosition.y)
                                        //if (h == (int)startPos.y)
                                        {
                                            if (currentItem.itemName == this.itemName && currentItem.stackAmount < currentItem.stackLimit) //hmmm will need some work
                                            {
                                                firstCellisLikeItem = true;
                                            }
                                        }
                                        else
                                        {
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
                    {
                        nearestDistance = compareDistance;
                    }
                }
            }
        }

        if (nearestDistance > 200f) //if the item is really far away from the inventory slot, probably dont do anything
            return false;
        else
        {
            return true;
        }
    }

    public void SearchAndMoveToNearestInventorySlot()
    {
        //Search
        //InventorySlot[] inventorySlots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
        InventorySlot[] inventorySlots = FindObjectsByType<InventorySlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (inventorySlots.Length < 1) { return; }
        

        InventorySlot nearestInventorySlot = inventorySlots[0]; ////
        float nearestDistance = 300f;////

        bool isStacking = false;///
        bool isKeepHolding = false;
        bool isTrashing = false;
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
                                    Item currentItem = currentInventory.inventory[h][w].GetHeldItem();
                                    if (currentItem != null) //if there is something inside the checking cells
                                    {
                                        if (w == (int)slotPosition.x && h == (int)slotPosition.y)
                                        //if (h == (int)startPos.y)
                                        {
                                            if (currentItem.itemName == this.itemName && currentItem.stackAmount < currentItem.stackLimit) //hmmm will need some work
                                            {
                                                firstCellisLikeItem = true;
                                            }
                                        }
                                        else
                                        {
                                            itemDoesNotCollideWithOtherItems = false;
                                        }
                                    }
                                }
                            }
                        }


                        if (itemDoesFitInsideInventoryFrame && (itemDoesNotCollideWithOtherItems || firstCellisLikeItem))
                        {
                            //Debug.Log("itemDoesFitInsideInventoryFrame: " + itemDoesFitInsideInventoryFrame);
                            //Debug.Log("itemDoesNotCollideWithOtherItems: " + itemDoesNotCollideWithOtherItems);
                            //Debug.Log("firstCellisLikeItem: " + firstCellisLikeItem);
                            if (slot.GetHeldItem() == null) //if the slot isnt holding anything
                            {
                                isStacking = false;
                                isKeepHolding = false;
                                isTrashing = false;
                                nearestDistance = compareDistance;
                                nearestInventorySlot = slot;
                                stackingItem = null;
                                //Debug.Log("empty " + nearestDistance);
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
                                        isTrashing = false;
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
                                    isTrashing = false;
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
                    else //trash slot
                    {
                        isStacking = false;
                        isKeepHolding = false;
                        isTrashing = true;
                        nearestDistance = compareDistance;
                        nearestInventorySlot = slot;
                        stackingItem = null;
                        //Debug.Log("trash " + nearestDistance);
                    }
                    
                }
            }
        }

        //Move
        if (nearestDistance > 200f)// * ( itemHeight > itemWidth ? itemHeight : itemWidth)) //if the item is really far away from the inventory slot, probably dont do anything
        {
            //check if the item's location is still inside the current inventory   
            if (this.transform.position.x > currentInventory.inventoryPanel.gameObject.transform.position.x - currentInventory.inventoryPanel.sizeDelta.x 
                && this.transform.position.x < currentInventory.inventoryPanel.gameObject.transform.position.x + currentInventory.inventoryPanel.sizeDelta.x 
                && this.transform.position.y > currentInventory.inventoryPanel.gameObject.transform.position.y - currentInventory.inventoryPanel.sizeDelta.y 
                && this.transform.position.y < currentInventory.inventoryPanel.gameObject.transform.position.y + currentInventory.inventoryPanel.sizeDelta.y)
            {
                Debug.Log("Previous");
                if (previousInventorySlot == null)
                    return;
                currentInventorySlot = previousInventorySlot;
                transform.position = currentInventorySlot.transform.position;
                currentInventorySlot.currentHeldItem = this; //need to do more with this
                currentInventorySlot.SetHeldItem(this);

                AddComponentSlots(inventorySlots);
            }
            else //outside of the current inventory
            {
                if (player != null)
                {
                    Instantiate(worldItem.gameObject, player.transform.position + new Vector3(0, 0, 2f), player.transform.rotation);
                    OnDestroy();
                }
                
            } 
        }
        else
        {
            Debug.Log("next");
            currentInventorySlot = nearestInventorySlot;
            if (currentInventorySlot != null)
                previousInventorySlot = currentInventorySlot;
            transform.position = currentInventorySlot.transform.position;
            currentInventorySlot.currentHeldItem = this; //need to do more with this
            currentInventorySlot.SetHeldItem(this);
            currentInventory = currentInventorySlot.parentInventory;
            //Debug.Log(currentInventorySlot.inventoryPosition);

            if (isTrashing)
            {
                OnDestroy();
                return;
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
                {
                    PickUpItemInInventory();
                }
                else
                {
                    AddComponentSlots(inventorySlots);
                }
                
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
                    //Debug.Log("component slot added");
                }
            }
        }
    }

    public void ClearComponentSlots()
    {
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

    #endregion

    #region _Stack_
    [Space]
    public int stackAmount;
    public int stackLimit = 5;
    public TextMeshProUGUI stackText;

    public void SetStackLimit(int newLimit) { stackLimit = newLimit; UpdateStackText(); }
    public void IncreaseStackAmount(int increase) 
    { 
        if (stackAmount + increase <= stackLimit)
        {
            stackAmount += increase;
            UpdateStackText();
        }
            
    }
    public void DecreaseStackAmount(int decrease) 
    {
        stackAmount -= decrease;
        UpdateStackText();
        if (stackAmount == 0)
        {
            
            OnDestroy();
        }
            
    }
    public void SetStackAmount(int amount) { stackAmount = amount; UpdateStackText(); }
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
        }
        this.transform.SetAsLastSibling();
    }


    #endregion

    private void OnDestroy()
    {
        if (currentInventorySlot != null)
            currentInventorySlot.ClearHeldItem();
        ClearComponentSlots();
        Destroy(gameObject);
    }

    private void Awake()
    {
        this.gameObject.name = itemName.ToString();
        currentInventory = FindAnyObjectByType<Inventory>();
        parent = GetComponentInParent<Canvas>().gameObject; //huh i
        img = GetComponent<Image>();
        stackAmount = 1;
        SetStackLimit(5);
        player = FindAnyObjectByType<PlayerMovement>();
        //componentDistance = 100;
        if (itemComponents.Count < (itemWidth * itemHeight) - 1)
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
