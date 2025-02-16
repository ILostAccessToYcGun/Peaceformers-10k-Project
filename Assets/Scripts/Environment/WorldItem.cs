using UnityEngine;
using UnityEngine.UI;

public class WorldItem : BaseInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /*
    - has a reference to the inventory version of the item and vice versa
    - when the player is near it they can interact with it
    - when the player drops an item, this is instantiated
    - when the player hits or destorys a resource node, this is instantiated
    - if this item is alive (in the world) when the day ends, this is destroyed
     */

    [SerializeField] Item inventoryItem;
    [SerializeField] private Canvas interactionCanvas;
    [SerializeField] private Inventory playerInventory;

    protected override void OpenPrompt()
    {
        print("Open Item prompt");
        if (playerInventory != null)
        {
            playerInventory.AddItemToInventory(inventoryItem, 1);
            Debug.Log("Item is about to be destroyed");
            Destroy(this.gameObject);
        }
        else
            Debug.Log("player's inventory not found");
    }

    protected override void Awake()
    {
        base.Awake();
        interactionCanvas = GetComponentInChildren<Canvas>();
        interactionCanvas.worldCamera = Camera.main;
        playerInventory = FindAnyObjectByType<Inventory>(); //chnage later
        this.name = "WorldItem (" + inventoryItem.name + ")";
    }

    protected override void Update()
    {
        base.Update();
        interactionCanvas.gameObject.transform.rotation = Quaternion.identity;
        interactionCanvas.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z);
    }
}
