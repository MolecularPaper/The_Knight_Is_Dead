using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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
}

public class GameInfoExtension : GameInstance
{
    protected float currentTimeScale;

    //Triggers
    public bool isStart;
    public bool isPause;
    public bool isFade;

    public static CancellationTokenSource tokenSource;
}

public interface IGameObservable
{
    public void GameUpdated();

    public void Subscribe(IGameObserver obserbver);

    public void Unsubscribe(IGameObserver obserbver);
}

public interface IGameObserver
{
    public void GameUpdated(GameInfoExtension gameInfo);
}

public class GameObservable : GameInfoExtension, IGameObservable
{
    public delegate void GameMangerUpdateDel(GameInfoExtension gameInfo);
    public GameMangerUpdateDel gameMangerUpdateDel;

    public void GameUpdated()
    {
        if (gameMangerUpdateDel != null) gameMangerUpdateDel.Invoke(this);
    }

    public void Subscribe(IGameObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        gameMangerUpdateDel += observer.GameUpdated;
    }

    public void Unsubscribe(IGameObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        gameMangerUpdateDel -= observer.GameUpdated;
    }
}

public class GameManager : GameObservable, IPlayerObserver, IEnemyObserver
{
    public void Awake()
    {
        gm = this;

        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        playerCTRL.Subscribe(this);

        tokenSource = new CancellationTokenSource();

        EnemyCTRL[] enemies = GameObject.FindObjectsOfType<EnemyCTRL>();

        int enemiesLength = enemies.Length;
        for (int i = 0; i < enemiesLength; i++) {
            Destroy(enemies[i]);
        }
    }

    public void Start()
    {
        GameStart();
    }

    public async void GameStart()
    {
        GameUpdated();

        try {
            await Delay(1000); 
        }
        catch (TaskCanceledException) { 
            return;
        }

        isStart = true;
        GameUpdated();
        isStart = false;
    }

    public void PauseGame()
    {
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        isPause = true;
        GameUpdated();
    }

    public void ResumeGame()
    {
        Time.timeScale = currentTimeScale;
        isPause = false;
        GameUpdated();
    }

    public void ResetGame()
    {
        stageIndex = 0;

        tokenSource.Cancel();
        GameDataManager.dataManager.SaveGameData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public async Task Delay(int millisecondsDelay)
    {
        await Task.Delay(millisecondsDelay, tokenSource.Token);
        while (isPause) {
            await Task.Delay(1);
        }
    }

    public async void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        if (playerInfo.IsDead) {
            isFade = true;
            GameUpdated();

            try {
                await Delay(1500);
            }
            catch (TaskCanceledException) { }


            ResetGame();
        }
    }

    public void EnemyUpdated(EnemyObservable enemyCTRL)
    {
        if (enemyCTRL.IsDead) {
            stageIndex++;
            if (highestStageIndex < stageIndex) {
                highestStageIndex = stageIndex;
            }
        }

        GameUpdated();
    }

    public void LoadData(GameData gameData, NoDeletedData noDeletedData)
    {
        adDeleted = noDeletedData.adDeleted;
        if(gameData != null) {
            this.SetInfo(gameData, noDeletedData);
        }
    }
}
