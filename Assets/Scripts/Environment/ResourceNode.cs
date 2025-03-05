using NUnit.Framework.Internal.Execution;
using TMPro;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{ 
    [SerializeField] Healthbar resourceHealthBar; //resource's health bar, when you shoot it, it goes down, when its 0 the resource dies
    [SerializeField] TextMeshProUGUI resourceNodeTitle; //resource's health bar, when you shoot it, it goes down, when its 0 the resource dies
    [SerializeField] WorldItem resourceType; //this is the resource that the node will spit out
    [SerializeField] int resourceCount; //this is the total number of resources that will be obtained when you fully destroy a node

    [SerializeField] float nodeMaxHealth;
    private float nodeReleaseTimer;
    [SerializeField] float nodeReleaseDelay;
    [SerializeField] int resourceCountReleased;


    public void RandomizeResourceCount(int min, int max)
    {
        resourceCount = Random.Range(min, max + 1);
    }

    public void SetResourceNodeTitle(string newTitle)
    {
        resourceNodeTitle.text = newTitle;
    }


    private void Awake()
    {
        resourceHealthBar.SetMaxHealth(nodeMaxHealth);
        RandomizeResourceCount(7, 14);
        SetResourceNodeTitle(resourceType.inventoryItem.itemName.ToString() + " Node");
    }

    // Update is called once per frame
    void Update()
    {
        //for now the detection code will be here,
        //but maybe make a child of the healthbar script to override the lose health function to update this instead of updating every frame

        if (resourceHealthBar.GetCurrentHealth() == 0)
        {
            if (resourceCountReleased < resourceCount)
            {
                if (nodeReleaseTimer <= 0)
                {
                    GameObject newDroppedItem = Instantiate(resourceType.gameObject, transform.position + new Vector3(0, 2f, 0), transform.rotation);
                    WorldItem newWorldItem = newDroppedItem.GetComponent<WorldItem>();

                    int spawnedStackAmount = Random.Range(1, resourceCount - resourceCountReleased + 1); //can be optimized
                    newWorldItem.InitializeWorldObject(spawnedStackAmount);


                    resourceCountReleased += spawnedStackAmount;
                    nodeReleaseTimer = nodeReleaseDelay;
                }
                else
                {
                    nodeReleaseTimer -= Time.deltaTime;
                }
            }
            else
            {
                Destroy(gameObject);
            }
            

        }

        
    }
}
