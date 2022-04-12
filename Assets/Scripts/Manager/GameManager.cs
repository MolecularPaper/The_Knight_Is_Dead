using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameInfo : MonoBehaviour
{
    public int highestStageIndex;
    public int stageIndex;
    public float bgmVolume;
    public float seVolume;
    public bool adDeleted { get; set; }

    public GameInfo() { }

    public GameInfo(GameData gameData, NoDeletedData noDeletedData) => SetInfo(gameData, noDeletedData);

    public void SetInfo(GameData gameData, NoDeletedData noDeletedData)
    {
        this.highestStageIndex = gameData.highestStageIndex;
        this.stageIndex = gameData.stageIndex;
        this.bgmVolume = noDeletedData.bgmVolume;
        this.seVolume = noDeletedData.seVolume;
        this.adDeleted = noDeletedData.adDeleted;
    }
}

public class GameInstance : GameInfo
{
    public static GameManager gm;
    protected SpawnManager spawnManager;
    protected PlayerCTRL playerCTRL;
    protected EnemyCTRL enemyCTRL;
    [SerializeField] 
    protected Fade fade;
}

public class GameInfoExtension : GameInstance
{
    public float currentTimeScale;

    public static bool isPause;

    public Vector3 PlayerPosition => playerCTRL.transform.position;

    public static CancellationTokenSource tokenSource;

    public delegate void StageChangedDel(GameInfo gameInfo);
    public StageChangedDel stageChanged;
}

public class GameManager : GameInfoExtension, IPlayerObserver, IEnemyObserver
{
    public void Awake()
    {
        gm = this;
        GameObject player = GameObject.FindWithTag("Player");
        playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);

        spawnManager = GetComponent<SpawnManager>();
        tokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        LoadData();
        GameStart();
    }

    private void OnApplicationQuit() => SaveData();

    public async void GameStart()
    {
        stageChanged.Invoke(this);
        try { await Delay(1000); }
        catch (TaskCanceledException) { return; }
        playerCTRL.IsMove = true;
        SpawnEnemy();
    }

    public void PauseGame()
    {
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        isPause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = currentTimeScale;
        isPause = false;
    }

    public static async Task Delay(int millisecondsDelay)
    {
        await Task.Delay(millisecondsDelay, tokenSource.Token);
        while (isPause) {
            await Task.Delay(1);
        }
    }

    public void AttackEnemy(ulong damage)
    {
        if (enemyCTRL) {
            enemyCTRL.HitSound();
            enemyCTRL.HitEffect();
            enemyCTRL.Damage(damage);
        }
    }

    public void AttackPlayer(ulong damage)
    {
        if (playerCTRL) {
            playerCTRL.HitSound();
            playerCTRL.HitEffect();
            playerCTRL.Damage(damage);
        }
    }

    public async void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        if (playerInfo.IsDead) {
            fade.FadeOut(true);
            while (!fade.isFadeOut) {
                try { await Task.Delay(1, tokenSource.Token); }
                catch (TaskCanceledException) { return; }
            }

            stageIndex = 0;

            SaveData();
            tokenSource.Cancel();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
    }

    public async void EnemyUpdated(EnemyInfo enemyInfo)
    {
        if (enemyInfo.IsDead) {
            UpdateStageIndex();
            playerCTRL.SetCurrentHP();
            playerCTRL.IsAttack = false;
            ((Item)playerCTRL["Soul"]).Count += ((Item)enemyInfo["Soul"]).Count;
            playerCTRL.exp += enemyInfo.exp;
            if (playerCTRL.CanLevelUp) playerCTRL.LevelUp();

            try { await Delay(1500); }
            catch (TaskCanceledException) { return; }

            playerCTRL.CalculateHpBar();
            playerCTRL.IsMove = true;

            try { await Delay(1000); }
            catch (TaskCanceledException) { return; }

            SpawnEnemy();
            return;
        }
        else if (enemyInfo.IsStop) {
            playerCTRL.IsMove = false;

            try { await Delay(100); }
            catch (TaskCanceledException) { return; }

            playerCTRL.IsAttack = true;
            enemyCTRL.IsAttack = true;
        }
    }

    public void SpawnEnemy()
    {
        enemyCTRL = spawnManager.SpawnEnemy(stageIndex);
        enemyCTRL.Subscribe(this);
    }

    public void UpdateStageIndex()
    {
        stageIndex++;
        if (highestStageIndex < stageIndex) {
            highestStageIndex = stageIndex;
        }
        stageChanged.Invoke(this);
    }

    private void LoadData()
    {
        NoDeletedData noDeletedData = null;

        try {
            noDeletedData = GameDataManager.dataManager.LoadNoDeletedData();
            playerCTRL.nickName = noDeletedData.nickName;
        }
        catch (System.IO.DirectoryNotFoundException) { }

        try {
            GameData gameData = GameDataManager.dataManager.LoadGameData();
            GetComponent<AdManager>().SetAdInfos(gameData);
            playerCTRL.SetInfo(gameData);

            if (noDeletedData != null) 
                this.SetInfo(gameData, noDeletedData);
        }
        catch (System.IO.DirectoryNotFoundException) { }
    }

    private void SaveData() => GameDataManager.dataManager.SaveGameData(playerCTRL, this, GetComponent<AdManager>().adCollection);
}
