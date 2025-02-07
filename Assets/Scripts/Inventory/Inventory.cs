using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    //TODO: change access modifiers at some point
    public int inventoryWidth;
    public int inventoryHeight;
    public int cellDistance;
    [Space]
    public Image inventoryPanel;
    public GameObject inventorySlot;
    public Item SetupItem;

    public List<List<InventorySlot>> inventory = new List<List<InventorySlot>>(); //lists because later we will change the size
    //public List<List<InventorySlot>> emptyInventory = new List<List<InventorySlot>>(); //lists because later we will change the size

    public Item itemToTest;
    public void ShowInventory()
    {
        InventoryToggle(true);
    }
    public void HideInventory()
    {
        InventoryToggle(false);
    }

    private void InventoryToggle(bool toggle)
    {
        inventoryPanel.gameObject.SetActive(toggle);
        for (int j = 0; j < inventoryHeight; j++)
        {
            for (int i = 0; i < inventoryWidth; i++)
            {
                if (inventory[j][i].GetHeldItem() != null)
                {
                    inventory[j][i].GetHeldItem().gameObject.SetActive(toggle);
                }
            }
        }
    }

    public void SetInventory(InventorySlot[] cells)
    {
        if (cells == null)
            cells = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
            
        foreach (InventorySlot cell in cells)
        {
            inventory[(int)cell.inventoryPosition.y][(int)cell.inventoryPosition.x] = cell;
        }
    }
    public void GenerateInventory(int width, int height)
    {
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

        for (int y = 0; y < tempHeight; y++)
        {

            //List<InventorySlot> Row = new List<InventorySlot>();
            //emptyInventory.Add(Row);

            for (int x = 0; x < tempWidth; x++) //Infinite LOOP!!!!
            {
                Vector3 slotLocation = inventoryPanel.transform.position + new Vector3(-350 + cellDistance * x, 350 - cellDistance * y, inventoryPanel.transform.position.z);
                //Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
                GameObject inventorySlotGameObject = Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
                InventorySlot invSlot = inventorySlotGameObject.GetComponent<InventorySlot>();

                invSlot.SetInventoryPosition(new Vector2(x, y));
                //emptyInventory[y].Add(invSlot);
            }
            
                
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
                    if (slot.GetHeldItem().name == itemToFind.name)
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
                    if (slot.GetHeldItem().name == itemToFind.name && slot.GetHeldItem().stackAmount < slot.GetHeldItem().stackLimit)
                    {
                        Debug.Log("item name matches and amount is less than stack limit");
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
                //if (item != null)
                //{
                //    for (int h = j; h < (j + item.itemHeight >= inventory.Capacity ? inventory.Capacity - 1 : j + item.itemHeight); h++)
                //    {
                //        for (int w = i; w < (i + item.itemWidth >= inventory[j].Capacity ? inventory[j].Capacity - 1 : i + item.itemWidth); w++)
                //        {
                //            Debug.Log("h: " + h);
                //            Debug.Log("w: " + w);
                //            Item itemComponents = inventory[h][w].GetHeldItem();
                //            //if there is something inside the cell, then it collides with the item if it were to be placed there
                //            //so its not a valid spot
                //            if (itemComponents != null)
                //            {
                //                Debug.Log(itemComponents);
                //                isValidSpot = true;
                //            }
                //        }
                //    }
                //}
                //if (isValidSpot)
                //{
                //    Debug.Log("Valid spot found");
                //    return inventory[j][i]; /
                //}

                if (item == null)
                {
                    for (int h = j; h < (j + itemToFind.itemHeight >= inventory.Capacity ? inventory.Capacity - 1 : j + itemToFind.itemHeight); h++)
                    {
                        for (int w = i; w < (i + itemToFind.itemWidth >= inventory[j].Capacity ? inventory[j].Capacity - 1 : i + itemToFind.itemWidth); w++)
                        {
                            Debug.Log("h: " + h);
                            Debug.Log("w: " + w);
                            Item itemComponents = inventory[h][w].GetHeldItem();
                            //if there is something inside the cell, then it collides with the item if it were to be placed there
                            //so its not a valid spot
                            if (itemComponents != null)
                            {
                                Debug.Log(itemComponents);
                                isValidSpot = false;
                            }
                        }
                    }
                }
                if (isValidSpot)
                {
                    Debug.Log("Valid spot found");
                    return inventory[j][i];
                }

            }
        }
        return null;
    }

    public void AddItemToInventory(Item itemToFind, int amount)
    {
        InventorySlot findSlot = FindPartialyFilledItemOrEmptySlot(itemToFind);
        Debug.Log(findSlot);
        if (findSlot != null && findSlot.GetHeldItem() != null) { IncreaseItemStackAmount(findSlot, amount); }
        else
        {
            Debug.Log("new slot");
            //adds the item to a new slot
            GameObject added = Instantiate(itemToFind.gameObject, findSlot.transform.position, findSlot.transform.rotation, itemToFind.parent.transform);
            added.transform.localScale = Vector3.one;

            Item addedItem = added.GetComponent<Item>();
            addedItem.SearchAndMoveToNearestInventorySlot();
            if (!inventoryPanel.gameObject.activeSelf)
            {
                added.SetActive(false);
            }


            //Item droppedItem = dropped.GetComponent<Item>();
            //droppedItem.SetStackAmount(1);
            //droppedItem.UpdateStackText();
            //if (droppedItem.SearchForNearestValidInventorySlot())
            //    droppedItem.SearchAndMoveToNearestInventorySlot();
            //this.ItemComponentSetSiblingLast();
        }
    }
    public void RemoveItemFromInventory(Item itemToFind, int amount)
    {
        InventorySlot findSlot = FindTopSlotWithItem(itemToFind);
        Debug.Log(findSlot);
        if (findSlot != null) { DecreaseItemStackAmount(findSlot, amount); }
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
            if (inventory != null)
            {
                GenerateInventory(inventoryWidth, inventoryHeight);
                SetInventory(null);
            } 
        }
        
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AddItemToInventory(itemToTest, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RemoveItemFromInventory(itemToTest, 1);
            Debug.Log("Remove");
        }
    }
}
