using System.Threading.Tasks;
using UnityEngine;

public interface IEnemyObservable
{
    public void Subscribe(IEnemyObserver observer);

    public void Unsubscribe(IEnemyObserver observer);

    public void EnemyUpdated();
}

public interface IEnemyObserver
{
    public void EnemyUpdated(EnemyObservable enemyCTRL);
}

public interface IEnemyAction : IMobAction
{
    public void Move();
}

public abstract class EnemyInfo : MobMethodExtension
{
    [SerializeField] protected float stopDistance;
    protected float moveSpeed = 4.0f;

    protected bool isStop;
    protected bool isDead;

    public abstract bool IsStop { get; set; }
    public abstract bool IsAttack { get; set; }
    public abstract bool IsDead { get; set; }
}

public class EnemyObservable : EnemyInfo, IEnemyObservable
{
    private delegate void EnemyUpdatedDel(EnemyObservable enemyCTRL);
    private EnemyUpdatedDel enemyUpdatedDel;
    public override bool IsStop {
        get => isStop;
        set {
            isStop = value;
            EnemyUpdated();
        }
    }
    public override bool IsAttack {
        get => animator.GetBool("IsAttack");
        set {
            animator.SetBool("IsAttack", value);
        }
    }
    public override bool IsDead {
        get => isDead;
        set {
            if(isDead) return;
            isDead = value;
            if(IsDead) animator.SetTrigger("Dead");
            EnemyUpdated();
        }
    }

    public void Subscribe(IEnemyObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();
        enemyUpdatedDel += observer.EnemyUpdated;
    }

    public void Unsubscribe(IEnemyObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        enemyUpdatedDel -= observer.EnemyUpdated;
    }

    public void EnemyUpdated()
    {
        if (enemyUpdatedDel != null) enemyUpdatedDel.Invoke(this);
    }
}

public class EnemyCTRL : EnemyObservable, IPlayerObserver, IEnemyAction
{
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Subscribe(GameManager.gm);

        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        playerCTRL.Subscribe(this);
        Subscribe(playerCTRL);

        StageUI stageUI = FindObjectOfType<StageUI>();
        Subscribe(stageUI);

        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        Subscribe(spawnManager);
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        if (playerInfo.IsDead) {
            try {
                IsAttack = false;
            }
            catch (MissingReferenceException) {
                return;
            }
        }
    }

    public void Attack()
    {
        AttackSound();

        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        if (playerCTRL != null) {
            playerCTRL.Damage(((Ability)this["HP"]).point);
        }
    }

    public void Damage(ulong damage)
    {
        if (IsDead) return;

        totalDamage += (long)damage;

        HitEffect();
        HitSound();

        if (totalDamage >= (long)((Ability)this["HP"]).point) {
            IsDead = true;
            return;
        }

        EnemyUpdated();
    }

    public async void Move()
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        Vector3 playerPostion = playerCTRL.transform.position;

        while (Vector3.Distance(playerPostion, transform.position) > stopDistance) {
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.left);

            try { 
                await Task.Delay(1, GameManager.tokenSource.Token); 
            }
            catch (TaskCanceledException) {
                return; 
            }
        }

        IsStop = true;
        IsAttack = true;
    }

    public async void Dead()
    {
        try { 
            await GameManager.gm.Delay((int)(500 / Time.timeScale));
        }
        catch (TaskCanceledException) {
            return;
        }

        Destroy(this.gameObject);
    }
}
