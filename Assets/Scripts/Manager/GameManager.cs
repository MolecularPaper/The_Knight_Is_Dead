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

    public GameInfo(GameData gameData) => SetInfo(gameData);

    public void SetInfo(GameData gameData)
    {
        this.highestStageIndex = gameData.highestStageIndex;
        this.stageIndex = gameData.stageIndex;
        this.bgmVolume = gameData.bgmVolume;
        this.seVolume = gameData.seVolume;
        this.adDeleted = gameData.adDeleted;
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
    public Vector3 PlayerPosition => playerCTRL.transform.position;

    public CancellationTokenSource tokenSource;

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

        Time.timeScale = 4f;
    }

    private void OnApplicationQuit() => SaveData();

    public async void GameStart()
    {
        stageChanged.Invoke(this);
        try { await Task.Delay(1000, tokenSource.Token); }
        catch (TaskCanceledException) { return; }
        playerCTRL.IsMove = true;
        SpawnEnemy();
    }

    public void PauseGame()
    {
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    public void ResumeGame() => Time.timeScale = currentTimeScale;

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

    public async void PlayerUpdated(PlayerInfo playerInfo)
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
            playerCTRL.GetItem("Soul").Count += enemyInfo.GetItem("Soul").Count;

            try { await Task.Delay(1000, tokenSource.Token); }
            catch (TaskCanceledException) { return; }

            playerCTRL.CalculateHpBar();
            playerCTRL.IsMove = true;

            try { await Task.Delay(2000, tokenSource.Token); }
            catch (TaskCanceledException) { return; }

            SpawnEnemy();
            return;
        }
        else if (enemyInfo.IsStop) {
            playerCTRL.IsMove = false;

            try { await Task.Delay(100, tokenSource.Token); }
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
        try {
            GameData gameData = GameDataManager.dataManager.LoadGameData();
            playerCTRL.SetInfo(gameData);
            GetComponent<AdManager>().SetAdInfos(gameData.adInfos);
            this.SetInfo(gameData);
        }
        catch (System.IO.DirectoryNotFoundException) { }
    }

    private void SaveData() => GameDataManager.dataManager.SaveGameData(playerCTRL, this, GetComponent<AdManager>().adCollection);
}
