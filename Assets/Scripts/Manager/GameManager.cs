using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerCTRL _player;
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

    void Update()
    {

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
        currentEnemy = null;
        await Task.Delay(1);
    }

    public void AddSoul(long count)
    {
        player.playerData.soul += count + (long)(count * (player.playerData.abilities[AbilityType.LUK].point / 100));
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
