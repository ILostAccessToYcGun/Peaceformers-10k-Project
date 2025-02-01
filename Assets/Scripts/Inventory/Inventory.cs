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

    public List<List<Item>> inventory = new List<List<Item>>(); //lists because later we will change the size

    public void ShowInventory()
    {
        inventoryPanel.gameObject.SetActive(true);
    }
    public void HideInventory()
    {
        inventoryPanel.gameObject.SetActive(false);
    }


    public void GenerateInventory(int width, int height)
    {
        #region _List_Size_
        //look at milinote for a diagram of the lists
        inventory.Capacity = height;

        //Adding an Item, so the inner parts of the inventory can be reached

        List<Item> origin = new List<Item>();
        origin.Add(SetupItem);
        inventory.Add(origin);

        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].Capacity = width;
            Debug.Log(inventory[i].Capacity);
        }
        


        #endregion

        #region _Displaying_Inventory_Slots_

        for (int y = 0; y < inventory.Capacity; y++)
        {
            for (int x = 0; x < inventory[0].Capacity; x++)
            {
                Vector3 slotLocation = inventoryPanel.transform.position + new Vector3(-350 + cellDistance * x, 350 - cellDistance * y, inventoryPanel.transform.position.z);
                Instantiate(inventorySlot, slotLocation, inventoryPanel.transform.rotation, inventoryPanel.transform);
            }
            
                
        }
        inventory.Remove(origin);
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
        }
    }
}
