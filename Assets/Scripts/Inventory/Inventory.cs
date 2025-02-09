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
    public int inventoryWidth;
    public int inventoryHeight;
    public int cellDistance; //since im usihng the lay out group I can probably remove this, but for now it stays
    public GridLayoutGroup gridLayout;

    [Space]
    public RectTransform inventoryPanel;
    public GameObject inventorySlot;
    public Item testItem;
    [Space]
    public bool isPlayerInventory = true;

    public List<List<InventorySlot>> inventory = new List<List<InventorySlot>>(); //lists because later we will change the size
    //public List<List<InventorySlot>> emptyInventory = new List<List<InventorySlot>>(); //lists because later we will change the size

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
            cells = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
            
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
        gridLayout.cellSize.Set(cellWidth, cellHeight);

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

  

    public void DecreaseItemStackAmount(InventorySlot itemToDecrease, int amount)
    {
        itemToDecrease.GetHeldItem().DecreaseStackAmount(amount);
    }
    public void IncreaseItemStackAmount(InventorySlot itemToIncrease, int amount)
    {
        itemToIncrease.GetHeldItem().IncreaseStackAmount(amount);
    }

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

    public void AddItemToInventory(Item itemToFind, int amount)
    {
        InventorySlot findSlot = FindPartialyFilledItemOrEmptySlot(itemToFind);
        if (findSlot == null) { return; }
        if (findSlot.GetHeldItem() != null) { IncreaseItemStackAmount(findSlot, amount); }
        else
        {
            GameObject added = Instantiate(testItem.gameObject, findSlot.transform.position, findSlot.transform.rotation, itemToFind.parent.transform);
            added.transform.localScale = Vector3.one;
            Item addedItem = added.GetComponent<Item>();
            addedItem.SearchAndMoveToNearestInventorySlot();
            if (!inventoryPanel.gameObject.activeSelf)
                added.SetActive(false);
        }
    }
    public void RemoveItemFromInventory(Item itemToFind, int amount)
    {
        InventorySlot findSlot = FindTopSlotWithItem(itemToFind);
        if (findSlot == null) { return; }
        if (findSlot.GetHeldItem() != null) { DecreaseItemStackAmount(findSlot, amount); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        { 
            if (!inventoryPanel.gameObject.activeSelf)
            {
                ShowInventory();
            }
            else
            {
                HideInventory();
            }
        }


        if (Input.GetKeyDown(KeyCode.E) && inventoryPanel.gameObject.activeSelf)
        {
            GenerateInventory(inventoryWidth, inventoryHeight, 50, 50);
            SetInventory(null);
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
