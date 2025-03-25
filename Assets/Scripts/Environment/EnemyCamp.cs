using System.Collections.Generic;
using UnityEngine;

public class EnemyCamp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //the next thing I need to do is to have dedicated spawns for the enemy camps
    //what im thinking is that for however many camps we have, they will spawn 1-2 enemies on the camp
    //when that camp is _cleared_ a day timer is started and it will regen in 3 days
    //but these enemies are on a separate counter to the regular world spawns, so they dont get counted towards those variables

    [SerializeField] EnemyDirector ed;
    [SerializeField] public int campEnemiesAlive;
    [SerializeField] Vector2 EnemySpawnAmountRange;
    [SerializeField] int respawnCooldown;
    [SerializeField] int respawnTimer;
    [SerializeField] List<InteractableInventory> barrels;


    private void Awake()
    {
        ed = FindAnyObjectByType<EnemyDirector>();
    }
    public void UpdateCamp()
    {
        if (campEnemiesAlive < 1)
        {
            //respawn the enemies
            if (respawnTimer < 1)
            {
                int size = (int)Random.Range(EnemySpawnAmountRange.x, EnemySpawnAmountRange.y + 1);
                int spawnAttemps = 0;
                for (int i = 0; i < size; )
                {
                    float varX = Random.Range(-10, 10);
                    float varZ = Random.Range(-10, 10);
                    ed.SelectLocation(transform.position + new Vector3(varX, 0, varZ));

                    if (spawnAttemps > 10)
                    {
                        ++i;
                        ed.SpawnEnemy(this);
                        spawnAttemps = 0;
                    }
                    else
                    {
                        if (ed.CheckForValidSpawn(2f) == new Vector3(0, 100, 0))
                            ++spawnAttemps;
                        else
                        {
                            ++i;
                            ed.SpawnEnemy(this);
                            spawnAttemps = 0;
                        }
                    }
                }
                respawnTimer = respawnCooldown;

                //refill the barrels
                foreach (InteractableInventory barrel in barrels)
                {
                    barrel.RandomizeInventoryLoot();
                }
            }
            else
                --respawnTimer;
        }
    }
}
