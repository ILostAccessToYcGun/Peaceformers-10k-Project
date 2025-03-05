using System.Collections.Generic;
using UnityEngine;

public class MapDirector : MonoBehaviour
{
    [SerializeField] List<GameObject> resourceNodeSelection;
    [Space]
    [Header("Node Info")]
    [SerializeField] int nodeAlive;
    [SerializeField] int nodeLimit;

    LayerMask checkMasks;

    private GameObject SelectRandomNode()
    {
        if (resourceNodeSelection.Count < 1) { return null; }
        int choice = Random.Range(0, resourceNodeSelection.Count);
        return resourceNodeSelection[choice];
    }

    private Vector3 CheckForValidSpawn()
    {
        RaycastHit hit;

        bool rayHit = Physics.SphereCast(transform.position, 3f, -transform.up, out hit, 100f, checkMasks);

        Debug.Log(rayHit);

        // Does the ray intersect any objects excluding the player layer
        if (rayHit)
        {
            
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
        ++nodeAlive;
        return true;
    }

    public void GenerateNodes()
    {
        FindAliveNodes();
        if (nodeAlive >= nodeLimit) { return; }
        for (int i = nodeAlive; i < nodeLimit; i++)
        {
            
            SelectRandomLocation();
            if (!SpawnNode())
            {
                i--;
            }
            
        }
    }

    public void FindAliveNodes()
    {
        ResourceNode[] foundNodes = FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        nodeAlive = foundNodes.Length;
    }

    public void DestroyWorldItems()
    {
        WorldItem[] itemsOnTheGround = FindObjectsByType<WorldItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < itemsOnTheGround.Length; i++)
        {
            Destroy(itemsOnTheGround[i].gameObject);
        }
    }

    private void Start()
    {
        checkMasks = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, -transform.up * 100f, Color.cyan);
    }
}
