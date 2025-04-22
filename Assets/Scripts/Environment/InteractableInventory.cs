using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class InteractableInventory : BaseInteractable
{
    /*
     * when we interact with the crate or something it SHOULD
         * add this interactable's inventory items to the second inventory 
         * open the second inventory and the player's inventory
         * 
     * we also need to be able to add items to this crate, but thats built into the inventory so we're fine
     *
     *
     *
    */
    [SerializeField] PlayerUIToggler playerUIToggler;

    [SerializeField] public List<GameObject> itemSelection;
    [SerializeField] public List<Item> itemInventory;
    [SerializeField] public List<Vector3> itemDet; //keep it simple
    [SerializeField] int TotalItemValue;
    [SerializeField] int currentValue;


    [SerializeField] Inventory inventory;
    [SerializeField] RectTransform inventoryPanel;
    [SerializeField] Vector2 inventorySize;
    [SerializeField] Vector2 inventorySizeRangeX;
    [SerializeField] Vector2 inventorySizeRangeY;
    [SerializeField] bool isRandomizingSize = true;
    [SerializeField] bool isRandomizingLoot = true;
    

    protected override void OpenPrompt()
    {
        Debug.Log("Open secondary inventory and player inventory");

        playerUIToggler.ToggleInventoryUIs();

        ToggleInventories();
    }

    public void ToggleInventories()
    {
        if (playerUIToggler.GetSecondaryInventoryShowingBool())
        {
            inventory.inventoryWidth = (int)inventorySize.x;
            inventory.inventoryHeight = (int)inventorySize.y;
            inventory.GenerateInventory(inventory.inventoryWidth, inventory.inventoryHeight, 100, 100);
            inventory.SetInventory(null);
            Invoke("CopyAndSetInventory", 0.05f);
        }
        else
        {
            if (playerUIToggler.GetSecondaryInventory() != null)
            {
                if (playerUIToggler.GetSecondaryInventory() == this)
                {
                    UpdateInventoryLoot();
                    playerUIToggler.SetSecondaryInventory(null);
                    Invoke("ClearAndDestroyInventory", 0.2f);
                }
                else
                {
                    playerUIToggler.GetSecondaryInventory().UpdateInventoryLoot();
                    playerUIToggler.SetSecondaryInventory(null);
                    Invoke("ClearAndDestroyInventory", 0.2f);
                    Invoke("OpenPrompt", 0.25f);
                }
            }

        }
    }

    public void CopyAndSetInventory()
    {
        inventory.CopyInventory(itemInventory, itemDet);
        playerUIToggler.SetSecondaryInventory(this);
    }

    public void ClearAndDestroyInventory()
    {
        inventory.ClearInventory();
        inventory.DestroyInventory();
    }

    public void RandomizeInventorySize(int minX, int minY, int maxX, int maxY)
    {
        if (minX <= 0) { minX = 1; }
        if (minY <= 0) { minY = 1; }

        if (maxX > 10) { maxX = 10; }
        if (maxY > 10) { maxY = 10; }
        inventorySize.x = (int)Random.Range(minX, maxX + 1);
        inventorySize.y = (int)Random.Range(minY, maxY + 1);
    }

    public void RandomizeInventoryLoot()
    {
        if (inventory == null)
        {
            Invoke("RandomizeInventoryLoot", 0.5f);
            return;
        }
            
        inventory.DestroyInventory();
        inventory.inventoryWidth = (int)inventorySize.x;
        inventory.inventoryHeight = (int)inventorySize.y;
        inventory.GenerateInventory(inventory.inventoryWidth, inventory.inventoryHeight, 100, 100);
        inventory.SetInventory(null);
        while (currentValue < TotalItemValue)
        {
            Item addingItem = itemSelection[0].GetComponent<Item>();
            int itemAmount = 0;
            int actualAmount = 0;
            int randX = 0;
            int randY = 0;

            int overspillValue = -1;

            do
            {
                int itemSelect = Random.Range(0, itemSelection.Count); //choose a random item from our selection to add
                addingItem = itemSelection[itemSelect].GetComponent<Item>();

                itemAmount = Random.Range(1, (TotalItemValue - currentValue > addingItem.stackLimit ? addingItem.stackLimit + 1 : TotalItemValue - currentValue + 1));

                randX = Random.Range(0, inventory.inventoryWidth);
                randY = Random.Range(0, inventory.inventoryHeight);

                
                if (addingItem.itemName == Item.Name.AmmoCrate)
                {
                    actualAmount = itemAmount * 4;
                }
                else
                {
                    actualAmount = itemAmount;
                }
                //Debug.Log("actualAmount: " + actualAmount);
                overspillValue = inventory.AddItemToInventory(addingItem, actualAmount, randX, randY);
                //Debug.Log(addingItem.itemName + "Amount: " + actualAmount + ". overspillValue: " + overspillValue);
                if (overspillValue > 0)
                {
                    if (addingItem.itemName == Item.Name.AmmoCrate)
                    {
                        currentValue -= (int)(overspillValue * 0.25f);
                    }
                    else
                    {
                        currentValue -= overspillValue; 
                        //debug the overspill more, we dont get the exact item value back, but it probably doesnt have to be exact
                    }
                    
                }
            }
            while (overspillValue < 0);
            
            currentValue += itemAmount;
            //Debug.Log("currentValue: " + currentValue);

            itemInventory.Add(addingItem);
            itemDet.Add(new Vector3(actualAmount, randX, randY));

            //i += itemValue;
        }
        //Debug.Log("Inventory is set up and should have items");
        inventory.ClearInventory();
        inventory.DestroyInventory();
    }


    public void AddInventoryLoot()
    {
        if (inventory == null)
        {
            Invoke("AddInventoryLoot", 0.5f);
            return;
        }

        inventory.DestroyInventory();
        inventory.inventoryWidth = (int)inventorySize.x;
        inventory.inventoryHeight = (int)inventorySize.y;
        inventory.GenerateInventory(inventory.inventoryWidth, inventory.inventoryHeight, 100, 100);
        inventory.SetInventory(null);


        Item addingItem = itemSelection[3].GetComponent<Item>();
        inventory.AddItemToInventory(addingItem, 60);
        itemInventory.Add(addingItem);
        itemDet.Add(new Vector3(60, 0, 0));
        inventory.AddItemToInventory(addingItem, 60);
        itemInventory.Add(addingItem);
        itemDet.Add(new Vector3(60, 1, 0));
        inventory.AddItemToInventory(addingItem, 60);
        itemInventory.Add(addingItem);
        itemDet.Add(new Vector3(60, 2, 0));

        inventory.ClearInventory();
        inventory.DestroyInventory();
    }

    public void UpdateInventoryLoot()
    {
        itemInventory.Clear();
        itemDet.Clear();

        currentValue = 0;
        List<Item> blackList = new List<Item>();

        for (int j = 0; j < inventory.inventory.Capacity; j++)
        {
            for (int i = 0; i < inventory.inventory[j].Capacity; i++)
            {
                InventorySlot slot = inventory.inventory[j][i];
                Item currentItem = slot.GetHeldItem();
                if (currentItem != null)
                {
                    Item addingItem = itemSelection[0].GetComponent<Item>(); ;
                    for (int k = 0; k < itemSelection.Count; k++)
                    {
                        if (currentItem.itemName == itemSelection[k].GetComponent<Item>().itemName)
                        {
                            addingItem = itemSelection[k].GetComponent<Item>();
                        }
                    }

                    if (blackList.Count > 0)
                    {
                        bool isInBlackList = false;
                        foreach (Item item in blackList)
                        {
                            if (currentItem == item)
                            {
                                isInBlackList = true;
                                break;
                            }
                        }

                        if (!isInBlackList)
                        {
                            blackList.Add(currentItem);
                            itemInventory.Add(addingItem);
                            itemDet.Add(new Vector3(currentItem.stackAmount, currentItem.GetCurrentInventorySlot().inventoryPosition.x, currentItem.GetCurrentInventorySlot().inventoryPosition.y));

                            currentValue += (int)((float)currentItem.stackAmount * (currentItem.itemName == Item.Name.AmmoCrate ? 0.25f : 1f));
                        }
                    }
                    else
                    {
                        blackList.Add(currentItem);
                        itemInventory.Add(addingItem);
                        itemDet.Add(new Vector3(currentItem.stackAmount, currentItem.GetCurrentInventorySlot().inventoryPosition.x, currentItem.GetCurrentInventorySlot().inventoryPosition.y));

                        //update the current item value
                        currentValue += (int)( (float)currentItem.stackAmount * (currentItem.itemName == Item.Name.AmmoCrate ? 0.25f : 1f ) );

                    }
                }
            }
        }
    }

    protected override void InteractDistanceCheck()
    {
        float distance = Vector3.Distance(transform.position, playerMovement.gameObject.transform.position);
        if (distance <= playerDetectionRange && !initializeAction)
        {
            player.AddInteractionPrompt(interactionPrompt, distance);
            requestedInteract = true;
            interactionPrompt.RequestInteraction(interactHoldTime, ref interactionPromptImage, ref interactionFill, () => OpenPrompt()); //when the thing is filled
        }
        else if (distance >= playerDetectionRange && requestedInteract)
        {
            interactionPrompt.DisableInteraction();
            requestedInteract = false;
            initializeAction = false;
            if (playerUIToggler.GetSecondaryInventoryShowingBool())
                OpenPrompt();
        }
        else
        {
            interactionPrompt.SetHold(false);
            interactionPrompt.SetRequest(false);
        }
    }

    private void InitializeCampInventory1()
    {
        inventory.inventoryWidth = (int)inventorySize.x;
        inventory.inventoryHeight = (int)inventorySize.y;
        inventory.GenerateInventory(inventory.inventoryWidth, inventory.inventoryHeight, 100, 100);
        inventory.SetInventory(null);
        Invoke("CopyAndSetInventory", 0.1f);
        Invoke("InitializeCampInventory2", 0.1f);
    }

    private void InitializeCampInventory2()
    {
        UpdateInventoryLoot();
        playerUIToggler.SetSecondaryInventory(null);
        Invoke("ClearAndDestroyInventory", 0.1f);
    }


    private void Start()
    {
        //okay i think this is going to crash but ill try anyway
        Inventory[] invs = FindObjectsByType<Inventory>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Inventory inv in invs)
        {
            if (!inv.isPlayerInventory)
            {
                inventory = inv;
                break;
            }
        }

        if (isRandomizingSize)
            RandomizeInventorySize((int)inventorySizeRangeX.x, (int)inventorySizeRangeY.x, (int)inventorySizeRangeX.y, (int)inventorySizeRangeY.y);
        if (isRandomizingLoot)
            Invoke("RandomizeInventoryLoot", 0.1f);
        else
        {
            Invoke("AddInventoryLoot", 0.1f);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        GameObject mainCanvas = FindAnyObjectByType<PlayerUIToggler>().gameObject;
        playerUIToggler = mainCanvas.GetComponent<PlayerUIToggler>();
    }


}
