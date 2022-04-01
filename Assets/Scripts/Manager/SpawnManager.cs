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
    [SerializeField] private IncreaseData increaseData;

    void Awake()
    {
        sm = this;
    }

    public void Spawn()
    {
        int spawnIndex = GameManager.gm.gameData.stageIndex;
        EnemyCTRL enemy = Instantiate(spawnDatabase.spawnDatas[spawnIndex].entityObject, spawnPoint.position, Quaternion.identity, spawnPoint).GetComponent<EnemyCTRL>();
        enemy.enemyData = new EntityData(spawnDatabase.spawnDatas[spawnIndex].enemyData);

        long hp = (long)Mathf.Pow(increaseData.enemyHPIncrese * (spawnIndex + 1), 2) + 10;
        long atk = (long)Mathf.Pow(increaseData.enemyAtkIncrese * (spawnIndex + 1), 2) + 5;
        long soul = (long)(Mathf.Pow(increaseData.enemySoulIncrese * (spawnIndex + 1), 2)) + 5;

        print($"몬스터 정보[체력: {hp}, 공격력: {atk}, 영혼: {soul}]");

        enemy.enemyData.abilities.Add(AbilityType.HP, new Ability(hp));
        enemy.enemyData.abilities.Add(AbilityType.ATK, new Ability(atk));
        enemy.enemyData.soul = soul;

        enemy.enemyData.currentHp = (long)enemy.enemyData.abilities[AbilityType.HP].point;
        enemy.Reset();

        GameManager.gm.SetEnemy(enemy);
    }
}
