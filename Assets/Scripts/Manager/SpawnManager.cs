using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private SpawnList spawnList;
    [SerializeField] private Transform spawnPoint;

    public EnemyCTRL SpawnEnemy(int index)
    {
        if (index >= spawnList.enemyPrefabs.Count)
            index = spawnList.enemyPrefabs.Count - 1;

        GameObject enemy;
        try { enemy = Instantiate(spawnList.enemyPrefabs[index], spawnPoint.position, spawnPoint.rotation, spawnPoint); }
        catch { return null; }

        EnemyCTRL enemyCTRL = enemy.GetComponent<EnemyCTRL>();

        ulong hp = (ulong)Mathf.Pow(spawnList.hpIncrease * (index + 1), 2) + 10;
        ulong atk = (ulong)Mathf.Pow(spawnList.atkIncrease * (index + 1), 2) + 5;
        ulong soul = (ulong)(Mathf.Pow(spawnList.soulIncrease * (index + 1), 2)) + 5;

        enemyCTRL.AddAbility(new Ability("HP", hp));
        enemyCTRL.AddAbility(new Ability("ATK", atk));
        enemyCTRL.AddItem(new Item("Soul", soul));
        enemyCTRL.SetCurrentHP();
        enemyCTRL.Move();

        return enemyCTRL;
    }
}
