using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager sm;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SpawnDatabase spawnDatabase;
    [HideInInspector] public bool canSpawn;
    private int spawnIndex;

    void Awake()
    {
        sm = this;

        canSpawn = true;
        spawnIndex = 0;
    }

    void Update()
    {
        if (canSpawn) {
            Spawn();
        }
    }

    private void Spawn()
    {
        EnemyCTRL enemy = Instantiate(spawnDatabase.spawnDatas[spawnIndex].entityObject, spawnPoint.position, Quaternion.identity, spawnPoint).GetComponent<EnemyCTRL>();
        enemy.enemyData = new EntityData(spawnDatabase.spawnDatas[spawnIndex].enemyData);
        enemy.enemyData.currentHp = enemy.enemyData.hpPoint;
        spawnIndex++;

        GameManager.gm.SetEnemy(enemy);
        canSpawn = false;
    }
}
