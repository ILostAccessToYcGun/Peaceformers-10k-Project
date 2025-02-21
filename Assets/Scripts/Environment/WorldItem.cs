using TMPro;
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

    [SerializeField] public Item inventoryItem;
    [SerializeField] private Canvas interactionCanvas;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private TextMeshProUGUI worldItemStackCount;
    [SerializeField] private Rigidbody rb;


    protected override void OpenPrompt()
    {
        //print("Open Item prompt");
        if (playerInventory != null)
        {
            playerInventory.AddItemToInventory(inventoryItem, inventoryItem.stackAmount);
            Debug.Log("inventoryItem.stackAmount = " + inventoryItem.stackAmount);
            Destroy(this.gameObject);
        }
        else
            Debug.Log("player's inventory not found");
    }

    public void AddPopUpForce(float minForce = 500f, float maxForce = 700f)
    {
        //float xForce = Random.Range(-100f, 100f);
        //float yForce = Random.Range(400f, 500f);
        //float zForce = Random.Range(-100f, 100f);
        float explosionForce = 0;
        if (minForce != 0 && maxForce != 0)
            explosionForce = Random.Range(minForce * rb.mass, maxForce * rb.mass);


        float xPos = Random.Range(-1f, 1f);
        float zPos = Random.Range(-1f, 1f);

        //rb.AddForce(new Vector3(xForce, yForce, zForce));
        rb.AddExplosionForce(explosionForce, new Vector3(transform.position.x + xPos, transform.position.y - 5f, transform.position.z + zPos), 10f);
    }

    protected void UpdateStackCount()
    {
        worldItemStackCount.text = inventoryItem.stackAmount.ToString();
    }

    public void InitializeWorldObject(int worldStackAmount, float minForce, float maxForce)
    {
        inventoryItem.SetStackAmount(worldStackAmount);
        UpdateStackCount();
        AddPopUpForce(minForce, maxForce);
    }

    protected override void Awake()
    {
        base.Awake();
        interactionCanvas = GetComponentInChildren<Canvas>();
        interactionCanvas.worldCamera = Camera.main;
        playerInventory = FindAnyObjectByType<Inventory>(); //chnage later
        this.name = "WorldItem (" + inventoryItem.name + ")";
        UpdateStackCount();
    }

    protected override void Update()
    {
        base.Update();
        interactionCanvas.gameObject.transform.rotation = Quaternion.identity;
        interactionCanvas.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z);
    }
}
