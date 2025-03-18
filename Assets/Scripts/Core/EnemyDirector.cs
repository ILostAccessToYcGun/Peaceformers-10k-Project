using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class EnemyDirector : MonoBehaviour
{
    [Header("Enemy Modifiers")]
    [SerializeField] public float healthMultiplier;
    [SerializeField] public float damageMultiplier;
    [SerializeField] private float healthScaling;
    [SerializeField] private float damageScaling;
    [Space]
    [Header("Enemy Selection")]
    [SerializeField] public List<Transform> targetList;
    [SerializeField] List<GameObject> randomEnemySelection;
    [SerializeField] public GameObject settlementEnemy;
    [Space]
    [Header("Enemy Info")]
    [SerializeField] public int enemiesAlive;
    [SerializeField] int enemyRespawnCount;
    [SerializeField] int enemyLimit;
    [SerializeField] int currentSpawnAttempts;
    [SerializeField] List<int> queuedEnemyRespawn  = new List<int> { 7, 0, 0 };
    [Space]
    [SerializeField] Transform enemyParent;

    //the next thing I need to do is to have dedicated spawns for the enemy camps, maybe...
    LayerMask whiteListMasks;

    #region _Difficulty_

    public void IncreaseDifficulty()
    {
        healthMultiplier += healthScaling;
        damageMultiplier += damageScaling;
    }

    public void ResetDifficulty()
    {
        healthMultiplier = 1;
        damageMultiplier = 1;
    }

    #endregion

    #region _Location_

    private Vector3 CheckForValidSpawn()
    {
        RaycastHit hit;
        whiteListMasks = LayerMask.GetMask("Ground");
        bool rayHit = Physics.SphereCast(transform.position, 10f, -transform.up, out hit, 100f, whiteListMasks);
        // Does the ray intersect any objects excluding the player layer
        if (rayHit)
            return hit.point;
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

    #endregion

    #region _Spawning_Enemies_

    private GameObject SelectRandomEnemy()
    {
        if (randomEnemySelection.Count < 1) { return null; }
        int choice = Random.Range(0, randomEnemySelection.Count);
        return randomEnemySelection[choice];
    }

    private bool SpawnEnemy()
    {
        GameObject randomEnemy = SelectRandomEnemy();
        Vector3 location = CheckForValidSpawn();
        if (randomEnemy == null || location == new Vector3(0, 100, 0)) { return false; }

        GameObject newEnemy = Instantiate(randomEnemy, location, Quaternion.identity, enemyParent);
        currentSpawnAttempts = 0;
        return true;
    }

    public void GenerateEnemies()
    {
        //if (enemiesAlive >= enemyLimit) { return; }
        currentSpawnAttempts = 0;
        GetAndRemoveTopEnemyCountHistoryEntry();
        for (int i = 0; i < enemyRespawnCount; i++)
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

    public int SpawnSettlementEnemeies(GameObject homeSettlement, GameObject targetSettlement)
    {
        Vector3 randVec = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
        GameObject newEnemy = Instantiate(settlementEnemy, homeSettlement.transform.position + randVec, Quaternion.identity, enemyParent);
        TankEnemy setEnemyAI = newEnemy.GetComponentInChildren<TankEnemy>();
        setEnemyAI.SetParentSettlement(homeSettlement);
        setEnemyAI.SetDestination(targetSettlement.transform.position);

        return 1;
    }

    public void FindAliveEnemies()
    {
        StationaryEnemy[] foundEnemies = FindObjectsByType<StationaryEnemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        enemiesAlive = foundEnemies.Length;
    }

    #endregion

    #region _Enemy_Count_History_

    public void AddEnemyCountEntry()
    {
        queuedEnemyRespawn.Add(enemyLimit - enemiesAlive);
        enemyLimit = enemyLimit - (enemyLimit - enemiesAlive);
    }

    public void GetAndRemoveTopEnemyCountHistoryEntry()
    {
        int topRegen = queuedEnemyRespawn[0];
        enemyRespawnCount = topRegen;
        enemyLimit += topRegen;
        queuedEnemyRespawn.RemoveAt(0);
    }

    #endregion
}
