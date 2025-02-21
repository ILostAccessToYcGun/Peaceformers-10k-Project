using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Progress;
//using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    //TODO: change access modifiers at some point
    [SerializeField] UIManager uiManager;
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
    public bool isPlayerInventory = true;
    public List<List<InventorySlot>> inventory = new List<List<InventorySlot>>(); //lists because later we will change the size
    [Space]
    [SerializeField] QuestBoard playerQuestBoard; //this is set manually
    

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
        uiManager.SetUIOpenBool(toggle);
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
            cells = FindObjectsByType<InventorySlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (InventorySlot cell in cells)
        {
            if (!cell.isTrashSlot)
            {
                if (cell.transform.position.x > inventoryPanel.gameObject.transform.position.x - inventoryPanel.sizeDelta.x
                && cell.transform.position.x < inventoryPanel.gameObject.transform.position.x + inventoryPanel.sizeDelta.x
                && cell.transform.position.y > inventoryPanel.gameObject.transform.position.y - inventoryPanel.sizeDelta.y
                && cell.transform.position.y < inventoryPanel.gameObject.transform.position.y + inventoryPanel.sizeDelta.y)
                {
                    inventory[(int)cell.inventoryPosition.y][(int)cell.inventoryPosition.x] = cell;
                }
            }

        }
    }
    public void GenerateInventory(int width, int height, int cellWidth, int cellHeight)
    {
        //TODO: change this
        isPlayerInventory = true;

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

        for (int j = 0; j < inventory.Capacity; j++)
        {
            inventory.Add(new List<InventorySlot>());
            inventory[j].Capacity = width;

            for (int i = 0; i < inventory[0].Capacity; i++)
            {
                inventory[j].Add(null);
            }
        }



        #endregion

        #region _Displaying_Inventory_Slots_

        //InventorySlot setupSlot = inventorySlot.GetComponent<InventorySlot>();
        int tempHeight = inventory.Capacity;
        int tempWidth = inventory[0].Capacity;

        Vector3 slotLocation;

        for (int y = 0; y < tempHeight; y++)
        {
            for (int x = 0; x < tempWidth; x++) //Infinite LOOP!!!!
            {
                slotLocation = inventoryPanel.transform.position + new Vector3(-350 + cellDistance * x, 350 - cellDistance * y, inventoryPanel.transform.position.z);
                GameObject inventorySlotGameObject = Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
                InventorySlot invSlot = inventorySlotGameObject.GetComponent<InventorySlot>();

                invSlot.SetInventoryPosition(new Vector2(x, y));
            }
        }

        if (isPlayerInventory)
        {
            slotLocation = inventoryPanel.transform.position + new Vector3(-350 + cellDistance * tempWidth, 350 - cellDistance * (tempHeight - 1), inventoryPanel.transform.position.z);
            //Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
            GameObject trashSlotGameObject = Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
            InventorySlot trashSlot = trashSlotGameObject.GetComponent<InventorySlot>();
            trashSlot.isTrashSlot = true;
            Image trashImg = trashSlot.GetComponent<Image>();
            trashImg.color = new Color(1f, 60f / 255f, 60f / 255f, 1f);
            trashSlot.SetInventoryPosition(new Vector2(tempWidth, tempHeight - 1));
        }


        #endregion
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
    public InventorySlot FindPartialyFilledItemOrEmptySlot(Item itemToFind)
    {
        Debug.Log(itemToFind.itemName);
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
                        Debug.Log("Existing Item found");
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
                        itemSearchCount++; //count the number of items of that type
                        itemSearchHistory.Add(currentItem);
                    }
                }
                
            }
        }
        Debug.Log("itemSearchCount=" + itemSearchCount);
        return itemSearchCount;
    }

    #endregion

    #region _Inventory_Management_

    public void AddItemToInventory(Item itemToFind, int amount)
    {
        int decreaseAmount = amount;
        for (int i = amount; i > 0;)
        {
            InventorySlot findSlot = FindPartialyFilledItemOrEmptySlot(itemToFind);
            if (findSlot == null) 
            {
                if (amount > itemToFind.stackLimit)
                {
                    decreaseAmount = itemToFind.stackLimit;
                    itemToFind.InstantiateWorldObject(itemToFind.stackLimit);
                }
                else
                {
                    decreaseAmount = amount;
                    itemToFind.InstantiateWorldObject(amount);
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
                GameObject added = Instantiate(itemToFind.gameObject, findSlot.transform.position + new Vector3(0, 0, 1), findSlot.transform.rotation, parent.transform);
                added.transform.localScale = Vector3.one;
                Item addedItem = added.GetComponent<Item>();

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

        
    }
    public void RemoveItemFromInventory(Item itemToFind, int amount)
    {
        InventorySlot findSlot = FindTopSlotWithItem(itemToFind);
        if (findSlot == null) { return; }
        if (findSlot.GetHeldItem() != null) { DecreaseItemStackAmount(findSlot, amount); }

        if (playerQuestBoard != null)
            playerQuestBoard.UpdateQuests();
    }

    #endregion



    //private void Start()
    //{
        
    //    //ShowInventory();
    //    HideInventory();
        
    //}

    private void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        GenerateInventory(inventoryWidth, inventoryHeight, cellWidth, cellHeight);
        SetInventory(null);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{ 
        //    if (!inventoryPanel.gameObject.activeSelf)
        //    {
        //        ShowInventory();
        //    }
        //    else
        //    {
        //        HideInventory();
        //    }
        //}


        if (Input.GetKeyDown(KeyCode.E) && inventoryPanel.gameObject.activeSelf)
        {
            if (inventory.Count == 0)
            {
                
                SetInventory(null);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AddItemToInventory(testItem, 1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RemoveItemFromInventory(testItem, 1);
        }
    }

}
