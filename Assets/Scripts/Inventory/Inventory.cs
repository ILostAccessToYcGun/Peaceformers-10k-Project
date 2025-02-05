using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public void ShowInventory()
    {
        inventoryPanel.gameObject.SetActive(true);
    }
    public void HideInventory()
    {
        inventoryPanel.gameObject.SetActive(false);
    }

    public void SetInventory()
    {
        InventorySlot[] cells = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);

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

        ////Adding an Item, so the inner parts of the inventory can be reached

        //List<Item> temp = new List<Item>();
        //temp.Add(SetupItem);
        //inventory.Add(temp);

        //for (int i = 0; i < inventory.Count; i++)
        //{
            
        //    Debug.Log(inventory[i].Capacity);
        //}
        //inventory.Remove(temp);

        
        
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
            GenerateInventory(inventoryWidth, inventoryHeight);
            SetInventory();
        }
    }
}
