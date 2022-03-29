using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm { get; set; }
    public EnemyCTRL currentEnemy { get; private set; }
    public PlayerCTRL player { get; private set; }

    [SerializeField] private float canActionDistance;
    [SerializeField] private float stopDistance;

    public bool canAction {
        get => currentEnemy && player && Vector3.Distance(player.transform.position, currentEnemy.transform.position) <= canActionDistance;
    }
    public bool isStopDistance {
        get => currentEnemy && player && Vector3.Distance(player.transform.position, currentEnemy.transform.position) <= stopDistance;
    }

    void Awake()
    {
        gm = this;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerCTRL>();
    }

    void Update()
    {

    }

    public void SetEnemy(EnemyCTRL enemy)
    {
        currentEnemy = enemy;
        _ = player.Stop();
    }

    public async Task EnemyDead()
    {
        currentEnemy = null;

        await Task.Delay(1500);
        player.Move();

        await Task.Delay(2000);
        SpawnManager.sm.canSpawn = true;
    }

    public async Task PlayerDead()
    {
        player = null;
        currentEnemy = null;
    }

    public void AddSoul(int count)
    {
        player.playerData.soul += count;
        UIManager.ui.UpdateItemUI();
    }
}
