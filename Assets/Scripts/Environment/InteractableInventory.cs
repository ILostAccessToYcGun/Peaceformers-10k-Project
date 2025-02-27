using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] public List<GameObject> itemInventory;
    [SerializeField] int TotalItemValue;
    [SerializeField] int currentValue;


    [SerializeField] Inventory inventory;
    [SerializeField] RectTransform inventoryPanel;

    protected override void OpenPrompt()
    {
        Debug.Log("Open secondary inventory and player inventory");

        //if (playerUIToggler.GetPlayerInventoryShowingBool() && playerUIToggler.GetSecondaryInventoryShowingBool() ||
        //    !playerUIToggler.GetPlayerInventoryShowingBool() && !playerUIToggler.GetSecondaryInventoryShowingBool())
        //{
        //    playerUIToggler.ToggleSecondaryInventoryUI();
        //    playerUIToggler.ToggleInventoryUI();
        //}
        //else if(playerUIToggler.GetPlayerInventoryShowingBool() && !playerUIToggler.GetSecondaryInventoryShowingBool())
        //{
        //    playerUIToggler.ToggleSecondaryInventoryUI();
        //}

        playerUIToggler.ToggleInventoryUIs();
        //if (playerUIToggler.GetSecondaryInventoryShowingBool()) //this wasnt working before, kept instantly closing the UI
        //{
            //inventory.CopyInventory(itemInventory);
        //}
        //else
        //{
            //inventory.ClearInventory();
        //}
    }

    public void RandomizeInventoryLoot()
    {
        //FOR NOW this will add a random amount of random items into the inventory

        while (currentValue < TotalItemValue)
        {
            int itemSelect = Random.Range(0, itemSelection.Count); //choose a random item from our selection to add
            Item addingItem = itemSelection[itemSelect].GetComponent<Item>();
            int itemAmount = Random.Range(0, TotalItemValue - currentValue + 1);

            currentValue += itemAmount;

            if (addingItem.itemName == Item.Name.AmmoCrate)
            {
                itemAmount *= 4;
            }
            inventory.AddItemToInventory(addingItem, itemAmount); //randomize grid location later
            itemInventory.Add(itemSelection[itemSelect]);

            //i += itemValue;
        }
        Debug.Log("Inventory is set up and should have items");
    }

    private void Start()
    {
        //if (inventory.inventory == null)

        //else
        //{
        //    SetUpInventory();
        //}
        Invoke("RandomizeInventoryLoot", 0.1f);
    }
}
