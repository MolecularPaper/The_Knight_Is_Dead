using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerCTRL _player;
    [SerializeField] private BackPanelCTRL backPanelCTRL;
    [SerializeField] private float canActionDistance;

    private CancellationTokenSource tokenSource = new CancellationTokenSource();
    public static GameManager gm { get; set; }
    public GameData gameData { get; set; }
    public EnemyCTRL currentEnemy { get; private set; }
    public PlayerCTRL player { get => _player; }
    private GameDataManager gameDataManager;

    public bool canAction {
        get => currentEnemy && player && Vector3.Distance(player.transform.position, currentEnemy.transform.position) <= canActionDistance;
    }

    void Awake()
    {
        gm = this;
        gameDataManager = GetComponent<GameDataManager>();

        try {
            GameSaveData saveData = gameDataManager.LoadData();
            gameData = new GameData(saveData);
            player.playerData = new PlayerData(saveData);
        }
        catch (DirectoryNotFoundException) {
            gameData = new GameData();
            player.ResetAbility();
        }
    }

    void Start()
    {
        StartGame();
    }

    void OnApplicationQuit()
    {
        _ = UIManager.ui.FadeOut(true);
        gameDataManager.SaveData(player.playerData, gameData);
        CancelToken();
    }

    public void CancelToken()
    {
        if (!tokenSource.IsCancellationRequested) {
            tokenSource.Cancel();
        }
    }

    public async void StartGame()
    {
        player.Reset();
        UIManager.ui.UpdateStage(gameData.stageIndex, gameData.highestStageIndex);
        await UIManager.ui.FadeOut(false);
        SpawnManager.sm.Spawn();
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

        try { await Task.Delay(1500, tokenSource.Token); }
        catch { return; }

        player.Move();

        try { await Task.Delay(2000, tokenSource.Token); }
        catch { return; }

        SpawnManager.sm.Spawn();
        gameData.SetHighestStage();
        gameData.stageIndex++;

        UIManager.ui.UpdateStage(gameData.stageIndex, gameData.highestStageIndex);
    }

    public async void PlayerDead()
    {
        tokenSource.Cancel();

        currentEnemy.Destroy();
        await UIManager.ui.FadeOut(true);

        gameData.ReturnStage();
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
            if (!canIncreaseAbillity || ability.requestSoul > player.playerData.soul || ability.point >= ability.maxPoint) return;
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
