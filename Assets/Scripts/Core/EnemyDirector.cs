using System.Collections.Generic;
using UnityEngine;

public class EnemyDirector : MonoBehaviour
{
    [Header("Enemy Selection")]
    [SerializeField] List<GameObject> randomEnemySelection;
    [SerializeField] List<GameObject> settlementEnemySelection;
    [Space]
    [Header("Enemy Info")]
    [SerializeField] public int enemiesAlive;
    [SerializeField] int enemyLimit;
    [SerializeField] int currentSpawnAttempts;
    LayerMask whiteListMasks;

    private GameObject SelectRandomEnemy()
    {
        if (randomEnemySelection.Count < 1) { return null; }
        int choice = Random.Range(0, randomEnemySelection.Count);
        return randomEnemySelection[choice];
    }

    private Vector3 CheckForValidSpawn()
    {
        RaycastHit hit;
        whiteListMasks = LayerMask.GetMask("Ground");

        bool rayHit = Physics.SphereCast(transform.position, 10f, -transform.up, out hit, 100f, whiteListMasks);

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

    private bool SpawnEnemy()
    {
        GameObject randomNode = SelectRandomEnemy();
        Vector3 location = CheckForValidSpawn();
        if (randomNode == null || location == new Vector3(0, 100, 0)) { return false; }

        GameObject newNode = Instantiate(randomNode, location, Quaternion.identity);

        currentSpawnAttempts = 0;
        return true;
    }

    public void GenerateEnemies()
    {
        if (enemiesAlive >= enemyLimit) { return; }
        currentSpawnAttempts = 0;
        for (int i = enemiesAlive; i < enemyLimit; i++)
        {
            if (currentSpawnAttempts >= 1000) { break; }


            SelectRandomLocation();
            if (!SpawnEnemy())
            {
                i--;
                ++currentSpawnAttempts;
            }

        }
    }

    public void FindAliveEnemies()
    {
        StationaryEnemy[] foundEnemies = FindObjectsByType<StationaryEnemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        enemiesAlive = foundEnemies.Length;
    }

    //public void DestroyWorldItems()
    //{
    //    WorldItem[] itemsOnTheGround = FindObjectsByType<WorldItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

    //    for (int i = 0; i < itemsOnTheGround.Length; i++)
    //    {
    //        Destroy(itemsOnTheGround[i].gameObject);
    //    }
    //}
}
