using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager sm;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SpawnDatabase spawnDatabase;
    [HideInInspector] public bool canSpawn;
    private int spawnIndex = 0;

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

        long hp = (long)Mathf.Pow(1.4f * (spawnIndex + 1), 2) + 10;
        long atk = (long)Mathf.Pow(2.0f * (spawnIndex + 1), 2) + 50;
        long soul = (long)(Mathf.Pow(1.2f * (spawnIndex + 1), 2)) + 5;

        print($"몬스터 정보[체력: {hp}, 공격력: {atk}, 영혼: {soul}]");

        enemy.enemyData.abilities.Add(AbilityType.HP, new Ability(hp));
        enemy.enemyData.abilities.Add(AbilityType.ATK, new Ability(atk));
        enemy.enemyData.soul = soul;

        enemy.enemyData.currentHp = (long)enemy.enemyData.abilities[AbilityType.HP].point;
        spawnIndex++;

        enemy.Reset();

        UIManager.ui.UpdateStage(spawnIndex);

        GameManager.gm.SetEnemy(enemy);
        canSpawn = false;
    }
}
