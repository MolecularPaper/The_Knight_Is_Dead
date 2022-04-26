using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IGameObserver, IEnemyObserver
{
    [SerializeField] private SpawnList spawnList;
    [SerializeField] private Transform spawnPoint;

    private int currentStage;

    public void Awake()
    {
        GameManager.gm.Subscribe(this);
    }

    public void GameUpdated(GameInfoExtension gameInfo)
    {
        this.currentStage = gameInfo.stageIndex;

        if (gameInfo.isStart) {
            SpawnEnemy(currentStage);
        }
    }

    public async void EnemyUpdated(EnemyObservable enemyCTRL)
    {
        if (enemyCTRL.IsDead) {
            enemyCTRL.Unsubscribe(this);
            try {
                await GameManager.gm.Delay(1000);
            }
            catch (TaskCanceledException) { }
            SpawnEnemy(currentStage);
        }
    }

    public EnemyCTRL SpawnEnemy(int index)
    {
        if (index >= spawnList.enemyPrefabs.Count)
            index = spawnList.enemyPrefabs.Count - 1;

        GameObject enemy;
        enemy = Instantiate(spawnList.enemyPrefabs[index], spawnPoint.position, spawnPoint.rotation, spawnPoint);

        EnemyCTRL enemyCTRL = enemy.GetComponent<EnemyCTRL>();

        long hp = (long)Mathf.Pow(spawnList.hpIncrease * (index + 1), 2) + 10;
        long atk = (long)Mathf.Pow(spawnList.atkIncrease * (index + 1), 2) + 5;
        long soul = (long)(Mathf.Pow(spawnList.soulIncrease * (index + 1), 2)) + 5;
        long exp = (long)(Mathf.Pow(spawnList.expIncrease * (index + 1), 2)) + 1;

        enemyCTRL.AddAbility(new Ability("HP", hp));
        enemyCTRL.AddAbility(new Ability("ATK", atk));
        enemyCTRL.AddItem(new Item("Soul", soul));
        enemyCTRL.exp = exp;
        enemyCTRL.Move();

        return enemyCTRL;
    }
}
