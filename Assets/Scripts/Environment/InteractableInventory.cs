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

    [SerializeField] public List<Item> ItemSelection;
    [SerializeField] int baseNumberOfItems;


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
    }

    public void SetUpInventory()
    {
        //FOR NOW this will add a random amount of random items into the inventory

        for (int i = 0; i < baseNumberOfItems;)
        {
            int itemSelect = Random.Range(0, ItemSelection.Count); //choose a random item from our selection to add
            Item addingItem = ItemSelection[itemSelect];
            int itemStackAmount = Random.Range(0, baseNumberOfItems - i);
            int itemValue = itemStackAmount;

            if (addingItem.itemName == Item.Name.AmmoCrate)
            {
                itemValue /= 4; //basically if we're adding bullets they have 0.25 value instead of 1 value, you get 4 bullets per 1 resource
            }
            inventory.AddItemToInventory(addingItem, itemStackAmount);



            i += itemValue;
        }
    }

    private void Start()
    {
        SetUpInventory();
    }
}
