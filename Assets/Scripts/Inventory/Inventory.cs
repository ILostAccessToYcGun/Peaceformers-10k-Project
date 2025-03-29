using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.Actions.MenuPriority;
using Unity.VisualScripting;
//using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    //TODO: change access modifiers at some point
    [SerializeField] PlayerUIToggler playerUIToggler;
    [Space]
    public int inventoryWidth;
    public int inventoryHeight;
    public int cellWidth;
    public int cellHeight;
    public int cellDistance; //since im usihng the lay out group I can probably remove this, but for now it stays
    public GridLayoutGroup gridLayout;

    [Space]
    public RectTransform inventoryPanel;
    public GameObject parent;
    public GameObject inventorySlot;
    public Item testItem;
    [Space]
    public bool isPlayerInventory;
    public List<List<InventorySlot>> inventory = new List<List<InventorySlot>>(); //lists because later we will change the size
    [Space]
    [SerializeField] public QuestBoard playerQuestBoard; //this is set manually


    #region _General_Inventory_

    public void ShowInventory()
    {
        ToggleInventoryVisiblity(true);
    }
    public void HideInventory()
    {
        ToggleInventoryVisiblity(false);
    }

    private void ToggleInventoryVisiblity(bool toggle)
    {
        inventoryPanel.gameObject.SetActive(toggle);
        for (int j = 0; j < inventory.Count; ++j)
        {
            for (int i = 0; i < inventory[j].Count; ++i)
            {
                Item item = inventory[j][i].GetHeldItem();
                if (item != null)
                    item.gameObject.SetActive(toggle);
            }
        }
    }

    public void SetInventory(InventorySlot[] cells)
    {
        if (cells == null)
        {
            //cells = FindObjectsByType<InventorySlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            cells = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        }
            

        

        foreach (InventorySlot cell in cells)
        {
            if (!cell.isTrashSlot)
            {
                if (cell.transform.position.x > inventoryPanel.gameObject.transform.position.x - inventoryPanel.sizeDelta.x / 2
                && cell.transform.position.x < inventoryPanel.gameObject.transform.position.x + inventoryPanel.sizeDelta.x / 2
                && cell.transform.position.y > inventoryPanel.gameObject.transform.position.y - inventoryPanel.sizeDelta.y / 2
                && cell.transform.position.y < inventoryPanel.gameObject.transform.position.y + inventoryPanel.sizeDelta.y / 2)
                {
                    //Debug.Log("Height " + inventory.Count + " Width " + inventory[0].Count);
                    //Debug.Log("inventoryPosY " + cell.inventoryPosition.y + " inventoryPosX " + cell.inventoryPosition.x);
                    
                    if ((int)cell.inventoryPosition.y < inventoryHeight && (int)cell.inventoryPosition.x < inventoryWidth)
                    {
                        inventory[(int)cell.inventoryPosition.y][(int)cell.inventoryPosition.x] = cell;
                    }
                }
            }

        }
    }
    public void GenerateInventory(int width, int height, int cellWidth, int cellHeight)
    {
        #region _Grid_Layout_
        gridLayout.padding.left = 10;
        gridLayout.padding.right = 10;
        gridLayout.padding.top = 10;
        gridLayout.padding.bottom = 10;
        gridLayout.constraintCount = width;
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);

        #endregion

        #region _List_Size_
        //look at milinote for a diagram of the lists
        inventory.Capacity = height;
        //Debug.Log("height = " + inventory.Capacity);

        for (int j = 0; j < inventory.Capacity; j++)
        {
            inventory.Add(new List<InventorySlot>());
            inventory[j].Capacity = width;

            for (int i = 0; i < inventory[j].Capacity; i++)
            {
                inventory[j].Add(null);
            }
        }
        #endregion

        #region _Displaying_Inventory_Slots_

        int tempHeight = inventory.Capacity;
        int tempWidth = inventory[0].Capacity;

        Vector3 slotLocation;

        for (int y = 0; y < tempHeight; y++)
        {
            for (int x = 0; x < tempWidth; x++)
            {
                slotLocation = inventoryPanel.transform.position + new Vector3(-350 + cellDistance * x, 350 - cellDistance * y, inventoryPanel.transform.position.z);
                GameObject inventorySlotGameObject = Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
                InventorySlot invSlot = inventorySlotGameObject.GetComponent<InventorySlot>();
                invSlot.parentInventory = this;

                invSlot.SetInventoryPosition(new Vector2(x, y));
            }
        }

        if (isPlayerInventory)
        {
            slotLocation = inventoryPanel.transform.position + new Vector3(-350 + cellDistance * tempWidth, 350 - cellDistance * (tempHeight - 1), inventoryPanel.transform.position.z);
            //Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
            GameObject trashSlotGameObject = Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
            InventorySlot trashSlot = trashSlotGameObject.GetComponent<InventorySlot>();
            trashSlot.parentInventory = this;
            trashSlot.isTrashSlot = true;
            Image trashImg = trashSlot.GetComponent<Image>();
            trashImg.color = new Color(1f, 60f / 255f, 60f / 255f, 1f);
            trashSlot.SetInventoryPosition(new Vector2(tempWidth, tempHeight - 1));
        }
        #endregion
    }

    public void DestroyInventory()
    {
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[0].Capacity; i++)
            {
                Destroy(inventory[j][i].gameObject);
                //if (i == inventory[j].Capacity - 1)
                //{
                //    inventory[i].Clear();
                //    inventory[i].Capacity = 0;
                //}
            }
        }
        inventory = new List<List<InventorySlot>>();

    }

    #endregion

    #region _Item_Stacking_

    public void DecreaseItemStackAmount(InventorySlot itemToDecrease, int amount)
    {
        itemToDecrease.GetHeldItem().DecreaseStackAmount(amount);
    }
    public void IncreaseItemStackAmount(InventorySlot itemToIncrease, int amount)
    {
        itemToIncrease.GetHeldItem().IncreaseStackAmount(amount);
    }

    #endregion

    #region _Search_Inventory_

    public InventorySlot FindTopSlotWithItem(Item itemToFind)
    {
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[j].Capacity; i++)
            {
                InventorySlot slot = inventory[j][i];
                if (slot.GetHeldItem() != null)
                {
                    if (slot.GetHeldItem().itemName == itemToFind.itemName)
                        return slot;
                }
            }
        }
        return null;
    }

    public InventorySlot FindTopSlotWithItem(Item.Name itemToFind)
    {
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[j].Capacity; i++)
            {
                InventorySlot slot = inventory[j][i];
                if (slot.GetHeldItem() != null)
                {
                    if (slot.GetHeldItem().itemName == itemToFind)
                        return slot;
                }
            }
        }
        return null;
    }

    public InventorySlot FindPartialyFilledItemOrEmptySlot(Item itemToFind)
    {
        //Debug.Log(itemToFind.itemName);
        //finding partially filled, has prio over empty slot
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[j].Capacity; i++)
            {
                InventorySlot slot = inventory[j][i];
                if (slot.GetHeldItem() != null)
                {
                    if (slot.GetHeldItem().itemName == itemToFind.itemName && slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit)
                    {
                        //Debug.Log("Existing Item found");
                        return slot;
                    }
                }
            }
        }
        //finding empty slot
        //looping through inventory
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[j].Capacity; i++)
            {
                Item item = inventory[j][i].GetHeldItem();
                bool isValidSpot = true;
                //looping through item dimensions
                if (item == null)
                {
                    if (j + itemToFind.itemHeight > inventory.Capacity || i + itemToFind.itemWidth > inventory[j].Capacity)
                        isValidSpot = false;
                    else
                    {
                        for (int h = j; h < j + itemToFind.itemHeight; h++)
                        {
                            for (int w = i; w < i + itemToFind.itemWidth; w++)
                            {
                                Item itemComponents = inventory[h][w].GetHeldItem();
                                //if there is something inside the cell, then it collides with the item if it were to be placed there
                                //so its not a valid spot
                                if (itemComponents != null)
                                {
                                    isValidSpot = false;
                                }
                            }
                        }
                    }

                    if (isValidSpot)
                        return inventory[j][i];
                }
            }
        }
        return null;
    }

    public int FindItemCountOfName(Item.Name nameToFind)
    {
        int itemSearchCount = 0;
        List<Item> itemSearchHistory = new List<Item>();
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[j].Capacity; i++) //loop though the inventory 
            {
                InventorySlot slot = inventory[j][i];
                Item currentItem = slot.GetHeldItem();

                bool currentItemIsAlreadyCounted = false;
                if (itemSearchHistory.Count != 0)
                {
                    foreach (Item item in itemSearchHistory)
                    {
                        if (item == currentItem)
                        {
                            currentItemIsAlreadyCounted = true;
                        }
                    }
                }
                if (currentItem != null)
                {
                    if (currentItem.itemName == nameToFind && !currentItemIsAlreadyCounted) //make sure to ignore item components, if item = item from before ignore
                    {
                        itemSearchCount += currentItem.stackAmount; //count the number of items of that type
                        itemSearchHistory.Add(currentItem);
                    }
                }
                
            }
        }
        //Debug.Log("itemSearchCount=" + itemSearchCount); /
        return itemSearchCount;
    }

    public InventorySlot FindInventorySlotByGrid(int x, int y)
    {
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[j].Capacity; i++)
            {
                InventorySlot slot = inventory[j][i];
                if (slot.inventoryPosition == new Vector2(x, y))
                {
                    return slot;
                }
            }
        }

        return null;
    }

    #endregion

    #region _Inventory_Management_

    public int AddItemToInventory(Item itemToAdd, int amount)
    {
        int decreaseAmount = amount;
        for (int i = amount; i > 0;)
        {
            InventorySlot findSlot = FindPartialyFilledItemOrEmptySlot(itemToAdd);
            if (findSlot == null) 
            {
                if (amount > itemToAdd.stackLimit)
                {
                    decreaseAmount = itemToAdd.stackLimit;
                    //itemToAdd.InstantiateWorldObject(itemToAdd.stackLimit); //this is bugging
                    return itemToAdd.stackLimit;
                }
                else
                {
                    decreaseAmount = amount;
                    //itemToAdd.InstantiateWorldObject(amount); //this is bugging
                    return amount;
                }
            }

            if (findSlot.GetHeldItem() != null)
            {
                
                if (findSlot.currentHeldItem.stackAmount + amount > findSlot.currentHeldItem.stackLimit)
                {
                    //Debug.Log(decreaseAmount);

                    //findSlot.currentHeldItem.stackLimit - findSlot.currentHeldItem.stackAmount;
                    decreaseAmount = findSlot.currentHeldItem.stackLimit - findSlot.currentHeldItem.stackAmount;
                    Debug.Log(decreaseAmount);
                    IncreaseItemStackAmount(findSlot, findSlot.currentHeldItem.stackLimit);
                }
                else
                {
                    IncreaseItemStackAmount(findSlot, amount);
                    decreaseAmount = amount;
                }
                
                
            }
            else
            {
                GameObject added = Instantiate(itemToAdd.gameObject, findSlot.transform.position, findSlot.transform.rotation, parent.transform);
                added.transform.localScale = Vector3.one;
                //added.transform.localPosition = new Vector3(added.transform.localPosition.x, added.transform.localPosition.y, -1f);
                Debug.Log("added.transform.position = " + added.transform.position);
                Item addedItem = added.GetComponent<Item>();
                addedItem.currentInventory = this;

                if (amount > addedItem.stackLimit)
                {
                    decreaseAmount = addedItem.stackLimit;
                    addedItem.SetStackAmount(addedItem.stackLimit);
                }
                else
                {
                    addedItem.SetStackAmount(amount);
                    decreaseAmount = amount;
                }

                
                addedItem.SearchAndMoveToNearestInventorySlot();
                if (!inventoryPanel.gameObject.activeSelf)
                    added.SetActive(false);
            }

            if (playerQuestBoard != null)
            {
                Debug.Log("we should be updating the quests");
                playerQuestBoard.UpdateQuests();
            }

            amount -= decreaseAmount;
            i -= decreaseAmount;
        }
        return 0;
    }

    public int AddItemToInventory(Item itemToAdd, int amount, int x, int y)
    {
        int returnOverspill = -1;
        InventorySlot findSlot = FindInventorySlotByGrid(x, y);
        if (findSlot == null)
        {
            AddItemToInventory(itemToAdd, amount);
            return returnOverspill;
        }

        GameObject added = Instantiate(itemToAdd.gameObject, findSlot.transform.position + new Vector3(0, 0, 1), findSlot.transform.rotation, parent.transform);
        added.transform.localScale = Vector3.one;
        Item addedItem = added.GetComponent<Item>();
        addedItem.currentInventory = this;

        addedItem.SetStackAmount(amount);
        

        if (addedItem.SearchForNearestValidInventorySlot() == 0 || addedItem.SearchForNearestValidInventorySlot() == 2)
        {
            //Debug.Log("bad spot try again");
            Destroy(added);
            return returnOverspill;
        }

        int searchOverspill = addedItem.SearchAndMoveToNearestInventorySlot();

        //if this returns a value that is greater than 0 should return the overspill value
        if (searchOverspill == 0)
        {
            returnOverspill = searchOverspill;
        }
        else if (searchOverspill > 0)
        {
            returnOverspill = searchOverspill;
            Destroy(added);
        }

        if (amount > addedItem.stackLimit)
        {
            returnOverspill = amount - addedItem.stackLimit;
        }

        if (!inventoryPanel.gameObject.activeSelf)
            added.SetActive(false);

        if (playerQuestBoard != null)
        {
            //Debug.Log("we should be updating the quests");
            playerQuestBoard.UpdateQuests();
        }
        return returnOverspill;
    }


    public int RemoveItemFromInventory(Item itemToFind, int amount)
    {
        int decreaseAmount = 0;
        int removeCount = 0;
        for (int i = amount; i > 0;)
        {
            InventorySlot findSlot = FindTopSlotWithItem(itemToFind);
            if (findSlot == null) { return removeCount; }
            if (findSlot.GetHeldItem() != null)
            {
                if (amount > findSlot.GetHeldItem().stackAmount)
                {
                    decreaseAmount = findSlot.GetHeldItem().stackAmount;
                }
                else
                {
                    decreaseAmount = amount;
                }
                DecreaseItemStackAmount(findSlot, amount);
                amount -= decreaseAmount;
                removeCount += decreaseAmount;

                if (playerQuestBoard != null)
                    playerQuestBoard.UpdateQuests();
            }

            i -= decreaseAmount;
        }
        return removeCount;
    }

    public int RemoveItemFromInventory(Item.Name itemToFind, int amount)
    {
        int decreaseAmount = 0;
        int removeCount = 0;
        for (int i = amount; i > 0;)
        {
            InventorySlot findSlot = FindTopSlotWithItem(itemToFind);
            if (findSlot == null) { return removeCount; }
            if (findSlot.GetHeldItem() != null)
            {
                if (amount > findSlot.GetHeldItem().stackAmount)
                {
                    decreaseAmount = findSlot.GetHeldItem().stackAmount;
                }
                else
                {
                    decreaseAmount = amount;
                }
                DecreaseItemStackAmount(findSlot, amount);
                amount -= decreaseAmount;
                removeCount += decreaseAmount;

                if (playerQuestBoard != null)
                    playerQuestBoard.UpdateQuests();
            }

            i -= decreaseAmount;
        }
        return removeCount;
    }

    public void RemoveHalfInventory()
    {
        int totalAmount = FindItemCountOfName(Item.Name.Wood) + FindItemCountOfName(Item.Name.Stone) + FindItemCountOfName(Item.Name.Scrap) + (FindItemCountOfName(Item.Name.AmmoCrate) / 4);
        if (totalAmount == 0) return;

        if (totalAmount % 2 == 1)
            totalAmount--;
        int halfAmount = totalAmount / 2;
        //loop untill this number is 0;
        //choose a random item to remove
        //if the item is found in the inventory, reduce the stack amount by one
        //reduce the half amount by 1

        while (halfAmount > 0)
        {
            Item.Name choice = (Item.Name)Random.Range(0, 4);
            InventorySlot removeSlot = FindTopSlotWithItem(choice);
            if (removeSlot != null)
            {
                removeSlot.GetHeldItem().DecreaseStackAmount((removeSlot.GetHeldItem().itemName == Item.Name.AmmoCrate ? 4 : 1));
                --halfAmount;
            }
        }
    }


    //this feature is pretty much only for the secondary inventory
    public void CopyInventory(List<Item> inventoryToCopy, List<Vector3> itemDetails)
    {
        //I could work on this some more, so the location of the items are remembered but idc bro
        for (int k = 0; k < inventoryToCopy.Count; k++)
        {
            AddItemToInventory(inventoryToCopy[k], (int)itemDetails[k].x, (int)itemDetails[k].y, (int)itemDetails[k].z);
        }
    }

    public void ClearInventory()
    {
        for (int j = 0; j < inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory[j].Capacity; i++)
            {
                InventorySlot slot = inventory[j][i];
                Item currentItem = slot.GetHeldItem();
                if (currentItem != null)
                {
                    currentItem.DecreaseStackAmount(currentItem.stackAmount);
                }
            }
        }
    }

    #endregion


    private void Awake()
    {
        GenerateInventory(inventoryWidth, inventoryHeight, cellWidth, cellHeight);
        SetInventory(null);
    }
}
