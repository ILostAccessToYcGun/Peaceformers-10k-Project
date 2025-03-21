using System.Collections.Generic;
using UnityEngine;

public class MapDirector : MonoBehaviour
{
    
    [SerializeField] int currentSpawnAttempts;
    [Space]
    [Header("Node Info")]
    [SerializeField] List<GameObject> resourceNodeSelection;
    [SerializeField] public int nodesAlive;
    [SerializeField] int nodeLimit;
    [Space]
    [Header("EnemyCamp Info")]
    [SerializeField] List<GameObject> enemyCampSelection;
    [SerializeField] int enemyCampLimit;
    [Space]
    [Header("Parents")]
    [SerializeField] Transform nodeParent;
    [SerializeField] Transform enemyCampParent;

    private GameObject SelectRandomNode()
    {
        if (resourceNodeSelection.Count < 1) { return null; }
        int choice = Random.Range(0, resourceNodeSelection.Count);
        return resourceNodeSelection[choice];
    }

    private Vector3 CheckForValidSpawn(float radius = 2f)
    {
        RaycastHit hit;
        LayerMask whiteListMasks = LayerMask.GetMask("Ground");

        bool rayHit = Physics.SphereCast(transform.position, radius, -transform.up, out hit, 100f, whiteListMasks);
        if (rayHit)
            return hit.point;

        return new Vector3(0, 100, 0);
    }

    private void SelectRandomLocation(float radius = 2f)
    {
        bool isValidLocation = false;

        while (!isValidLocation)
        {
            int posX = Random.Range(-600, 601);
            int posZ = Random.Range(-600, 601);

            transform.position = new Vector3(posX, 25, posZ);

            if (CheckForValidSpawn(radius) != new Vector3(0, 100, 0))
                isValidLocation = true; break;
        }
    }

    private bool SpawnNode()
    {
        GameObject randomNode = SelectRandomNode();
        Vector3 location = CheckForValidSpawn();
        if (randomNode == null || location == new Vector3(0, 100, 0)) { return false; }

        GameObject newNode = Instantiate(randomNode, location, Quaternion.identity, nodeParent);

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

    private GameObject SelectRandomCamp()
    {
        if (enemyCampSelection.Count < 1) { return null; }
        int choice = Random.Range(0, enemyCampSelection.Count);
        return enemyCampSelection[choice];
    }

    private bool SpawnCamp()
    {
        GameObject randomCamp = SelectRandomCamp();
        Vector3 location = CheckForValidSpawn();
        if (randomCamp == null || location == new Vector3(0, 100, 0)) { return false; }

        GameObject newCamp = Instantiate(randomCamp, location, Quaternion.identity);

        currentSpawnAttempts = 0;
        return true;
    }

    public void GenerateCamps()
    {
        Debug.Log("generate enemy camps");
        currentSpawnAttempts = 0;
        for (int i = 0; i < enemyCampLimit; i++)
        {
            if (currentSpawnAttempts >= 1000) { break; }
            Debug.Log("try");
            SelectRandomLocation(30f);
            if (!SpawnCamp())
            {
                i--;
                ++currentSpawnAttempts;
            }
        }
    }
}
