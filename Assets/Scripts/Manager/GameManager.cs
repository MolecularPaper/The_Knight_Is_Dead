using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerCTRL _player;
    [SerializeField] private BackPanelCTRL backPanelCTRL;
    [SerializeField] private float canActionDistance;

    public static GameManager gm { get; set; }
    public EnemyCTRL currentEnemy { get; private set; }
    public PlayerCTRL player { get => _player; }

    public bool canAction {
        get => currentEnemy && player && Vector3.Distance(player.transform.position, currentEnemy.transform.position) <= canActionDistance;
    }

    void Awake()
    {
        gm = this;
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {

    }

    public async void StartGame()
    {
        player.Reset();
        UIManager.ui.UpdateStage(SpawnManager.sm.spawnIndex + 1);
        await UIManager.ui.FadeOut(false);
        SpawnManager.sm.canSpawn = true;
        player.Move();
    }

    public void SetEnemy(EnemyCTRL enemy)
    {
        currentEnemy = enemy;
        player.Stop();
    }

    public async void EnemyDead()
    {
        currentEnemy = null;

        await Task.Delay(1500);
        player.Move();

        await Task.Delay(2000);
        SpawnManager.sm.canSpawn = true;
    }

    public async void PlayerDead()
    {
        currentEnemy.Destroy();
        await UIManager.ui.FadeOut(true);

        SpawnManager.sm.canSpawn = false;
        SpawnManager.sm.ReturnSpawn();
        backPanelCTRL.Reset();

        StartGame();
    }

    public void AddSoul(long count)
    {
        player.playerData.soul += count + count * (player.playerData.abilities[AbilityType.LUK].point / 100);
        UIManager.ui.UpdateItemUI();
        UIManager.ui.UdateAllAbilityUI();
    }

    public bool doIncreaseAbillity { get; set; }
    public async void IncreaseAbility(string typeStr)
    {
        if (!System.Enum.TryParse<AbilityType>(typeStr, out AbilityType type)) throw new System.FormatException();
        PlayerData playerData = player.playerData;
        Ability ability = player.playerData.abilities[type];

        doIncreaseAbillity = true;
        bool canIncreaseAbillity = true;
        while (doIncreaseAbillity) {
            if (!canIncreaseAbillity || ability.requestSoul > player.playerData.soul) return;
            canIncreaseAbillity = false;

            playerData.soul -= ability.requestSoul;
            ability.point += ability.upPoint;
            ability.level++;

            player.playerData.abilities[type] = ability;

            UIManager.ui.UdateAllAbilityUI();
            UIManager.ui.UpdateItemUI();

            await Task.Delay(100);
            canIncreaseAbillity = true;
        }
    }
}
