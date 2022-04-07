using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstance : MonoBehaviour
{
    public static GameManager gm;
    protected SpawnManager spawnManager;
    protected PlayerCTRL playerCTRL;
    protected EnemyCTRL enemyCTRL;
    [SerializeField] protected Fade fade;
}

public class GameInfo : GameInstance
{
    public int highestStageIndex;

    public int stageIndex;
}

public class GameInfoExtension : GameInfo
{
    public Vector3 PlayerPosition => playerCTRL.transform.position;
}

public class GameManager : GameInfoExtension, IPlayerObserver, IEnemyObserver
{
    public delegate void StageChangedDel(GameInfo gameInfo);
    public StageChangedDel stageChanged;

    public void Awake()
    {
        gm = this;
        GameObject player = GameObject.FindWithTag("Player");
        playerCTRL = player.GetComponent<PlayerCTRL>();
        playerCTRL.Subscribe(this);
        spawnManager = GetComponent<SpawnManager>();
    }

    public void Start() => GameStart();

    public async void GameStart()
    {
        stageChanged.Invoke(this);
        await Task.Delay(1000);
        playerCTRL.IsMove = true;
        SpawnEnemy();
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

    public async void PlayerUpdated(PlayerInfo playerInfo)
    {
        if (playerInfo.IsDead) {
            fade.FadeOut(true);
            while (!fade.isFadeOut) await Task.Delay(1);

            stageIndex -= 10;
            stageIndex = Mathf.Clamp(stageIndex, 0, int.MaxValue);

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
            await Task.Delay(1000);
            playerCTRL.IsMove = true;
            await Task.Delay(2000);
            SpawnEnemy();
            return;
        }

        if (enemyInfo.IsStop) {
            playerCTRL.IsMove = false;
            await Task.Delay(100);
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
}
