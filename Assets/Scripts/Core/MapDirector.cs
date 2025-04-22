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
        //LayerMask whiteListMasks = LayerMask.GetMask("Ground", "uhhhh"); //i think this is a white list 
        LayerMask whiteListMasks = LayerMask.GetMask("Ground", "SpawnBlackList", "Default"); //i think this is a white list

        bool rayHit = Physics.SphereCast(transform.position, radius, -transform.up, out hit, 400f, whiteListMasks);
        if (hit.transform != null)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("SpawnBlackList") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Default")) 
            { 
                return new Vector3(0, -1, 0);
            }
        }
        else
        {
            return new Vector3(0, -1, 0);
        }

        if (rayHit)
            return hit.point;

        return new Vector3(0, -1, 0);
    }

    private void SelectRandomLocation(float radius = 2f)
    {
        bool isValidLocation = false;

        while (!isValidLocation)
        {
            int posX = Random.Range(-600, 601);
            int posZ = Random.Range(-600, 601);

            transform.position = new Vector3(posX, 200, posZ);

            if (CheckForValidSpawn(radius) != new Vector3(0, -1, 0))
                isValidLocation = true; break;
        }
    }

    private bool SpawnNode()
    {
        GameObject randomNode = SelectRandomNode();
        Vector3 location = CheckForValidSpawn();
        if (randomNode == null || location == new Vector3(0, -1, 0)) { return false; }

        float rot = Random.Range(0f, 359.9f);
        GameObject newNode = Instantiate(randomNode, location, Quaternion.AngleAxis(rot, transform.up), nodeParent);

        currentSpawnAttempts = 0;
        return true;
    }

    public bool SpawnNode(int node, Vector3 location)
    {
        GameObject randomNode = resourceNodeSelection[node];
        if (randomNode == null || location == new Vector3(0, -1, 0)) { return false; }

        float rot = Random.Range(0f, 359.9f);
        GameObject newNode = Instantiate(randomNode, location, Quaternion.AngleAxis(rot, transform.up), nodeParent);

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
        if (randomCamp == null || location == new Vector3(0, -1, 0)) { return false; }

        float rot = Random.Range(0f, 359.9f);
        GameObject newCamp = Instantiate(randomCamp, location, Quaternion.AngleAxis(rot, transform.up));

        currentSpawnAttempts = 0;
        return true;
    }

    public void GenerateCamps()
    {
        //Debug.Log("generate enemy camps");
        currentSpawnAttempts = 0;
        for (int i = 0; i < enemyCampLimit; i++)
        {
            if (currentSpawnAttempts >= 1000) { break; }
            //Debug.Log("try");
            SelectRandomLocation(40f);
            if (!SpawnCamp())
            {
                i--;
                ++currentSpawnAttempts;
            }
        }
    }

    //private void Update()
    //{
        
    //    if (Input.GetKey(KeyCode.M))
    //    {

    //        RaycastHit hit;
    //        LayerMask whiteListMasks = LayerMask.GetMask("Ground", "SpawnBlackList", "Default"); //i think this is a white list
    //        bool isValid = false;
    //        bool rayHit = Physics.SphereCast(transform.position, 25f, -transform.up, out hit, 100f, whiteListMasks);
    //        Debug.Log(hit);
    //        if (rayHit)
    //            isValid = true;
    //        else
    //            isValid = false;


    //        if (hit.transform != null)
    //        {
    //            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("SpawnBlackList") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Default"))
    //            {
    //                isValid = false;
    //            }
    //        }
    //        else
    //            isValid = false;

            
    //        Debug.Log("isValid = " + isValid);
    //    }
    //}
}
