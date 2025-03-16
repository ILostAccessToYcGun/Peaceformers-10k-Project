using System.Collections.Generic;
using UnityEngine;

public class MapDirector : MonoBehaviour
{
    [SerializeField] List<GameObject> resourceNodeSelection;
    [Space]
    [Header("Node Info")]
    [SerializeField] public int nodesAlive;
    [SerializeField] int nodeLimit;
    [SerializeField] int currentSpawnAttempts;
    LayerMask whiteListMasks;

    private GameObject SelectRandomNode()
    {
        if (resourceNodeSelection.Count < 1) { return null; }
        int choice = Random.Range(0, resourceNodeSelection.Count);
        return resourceNodeSelection[choice];
    }

    private Vector3 CheckForValidSpawn()
    {
        RaycastHit hit;
        whiteListMasks = LayerMask.GetMask("Ground");

        bool rayHit = Physics.SphereCast(transform.position, 2f, -transform.up, out hit, 100f, whiteListMasks);

        //Debug.Log(hit.collider.gameObject.layer);

        // Does the ray intersect any objects excluding the player layer
        if (rayHit)
        {
            //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            return hit.point;
        }
        return new Vector3(0, 100, 0);
    }

    private void SelectRandomLocation()
    {
        bool isValidLocation = false;

        while (!isValidLocation)
        {
            int posX = Random.Range(-600, 601);
            int posZ = Random.Range(-600, 601);

            transform.position = new Vector3(posX, 25, posZ);

            if (CheckForValidSpawn() != new Vector3(0, 100, 0))
                isValidLocation = true; break;
        }
    }

    private bool SpawnNode()
    {
        GameObject randomNode = SelectRandomNode();
        Vector3 location = CheckForValidSpawn();
        if (randomNode == null || location == new Vector3(0, 100, 0)) { return false; }

        GameObject newNode = Instantiate(randomNode, location, Quaternion.identity);

        currentSpawnAttempts = 0;
        return true;
    }

    public void GenerateNodes()
    {
        if (nodesAlive >= nodeLimit) { return; }
        currentSpawnAttempts = 0;
        for (int i = nodesAlive; i < nodeLimit; i++)
        {
            if (currentSpawnAttempts >= 1000) { break; }
            

            SelectRandomLocation();
            if (!SpawnNode())
            {
                i--;
                ++currentSpawnAttempts;
            }
            
        }
    }

    public void FindAliveNodes()
    {
        ResourceNode[] foundNodes = FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        nodesAlive = foundNodes.Length;
    }

    public void DestroyWorldItems()
    {
        WorldItem[] itemsOnTheGround = FindObjectsByType<WorldItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < itemsOnTheGround.Length; i++)
        {
            Destroy(itemsOnTheGround[i].gameObject);
        }
    }
}
