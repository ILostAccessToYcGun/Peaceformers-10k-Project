using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEditor.Progress;

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
    [SerializeField] Vector2 inventorySize;
    [SerializeField] RectTransform inventoryPanel;

    protected override void OpenPrompt()
    {
        Debug.Log("Open secondary inventory and player inventory");

        playerUIToggler.ToggleInventoryUIs();

        //if you open a crate and then close it using another UI, it will write the data into the box you used to close

        //if (playerUIToggler.GetSecondaryInventory() == null)
        //{
        //    if (playerUIToggler.GetSecondaryInventoryShowingBool())
        //    {
        //        inventory.CopyInventory(itemInventory, itemDet);
        //        playerUIToggler.SetSecondaryInventory(this);
        //    }
        //}
        
        
        if (playerUIToggler.GetSecondaryInventoryShowingBool())
        {
            //inventory.inventoryWidth = (int)inventorySize.x;
            //inventory.inventoryHeight = (int)inventorySize.y;
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

    public void RandomizeInventoryLoot()
    {
        inventory.DestroyInventory();
        //inventory.inventoryWidth = (int)inventorySize.x;
        //inventory.inventoryHeight = (int)inventorySize.y;
        inventory.GenerateInventory(inventory.inventoryWidth, inventory.inventoryHeight, 100, 100);
        inventory.SetInventory(null);
        //while (currentValue < TotalItemValue)
        //{
        //    int itemSelect = Random.Range(0, itemSelection.Count); //choose a random item from our selection to add
        //    Item addingItem = itemSelection[itemSelect].GetComponent<Item>();
        //    int itemAmount = Random.Range(1, (TotalItemValue - currentValue > addingItem.stackLimit ? addingItem.stackLimit + 1 : TotalItemValue - currentValue + 1));

        //    currentValue += itemAmount;
        //    Debug.Log("currentValue: " + currentValue);

        //    if (addingItem.itemName == Item.Name.AmmoCrate)
        //    {
        //        itemAmount *= 4;
        //    }

        //    int randX = 0;
        //    int randY = 0;

        //    do
        //    {
        //        randX = Random.Range(0, inventory.inventoryWidth);
        //        randY = Random.Range(0, inventory.inventoryHeight);
        //    }
        //    while (inventory.AddItemToInventory(addingItem, itemAmount, randX, randY) == false);
        //    itemInventory.Add(addingItem);
        //    itemDet.Add(new Vector3(itemAmount, randX, randY));

        //    //i += itemValue;
        //}
        //Debug.Log("Inventory is set up and should have items");
        inventory.ClearInventory();
        inventory.DestroyInventory();
    }

    public void UpdateInventoryLoot()
    {
        itemInventory.Clear();
        itemDet.Clear();

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
                        }
                    }
                    else
                    {
                        blackList.Add(currentItem);
                        itemInventory.Add(addingItem);
                        itemDet.Add(new Vector3(currentItem.stackAmount, currentItem.GetCurrentInventorySlot().inventoryPosition.x, currentItem.GetCurrentInventorySlot().inventoryPosition.y));
                    }



                }
            }
        }

        //loop through the inventory and find all the items inside
        //then put them and their amounts into lists
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


    private void Start()
    {
        Invoke("RandomizeInventoryLoot", 0.1f);
    }


}
