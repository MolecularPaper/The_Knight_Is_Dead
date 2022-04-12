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
    public void EnemyUpdated(EnemyInfo enemyInfo);
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
    private delegate void EnemyUpdatedDel(EnemyInfo enemyInfo);
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

public class EnemyCTRL : EnemyObservable, IEnemyAction
{
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        hpBarScale = hpBar.localScale;
    }

    private void OnApplicationQuit()
    {
        try { Destroy(this.gameObject); }
        catch { return; }
    }

    public void Attack()
    {
        AttackSound();
        GameManager.gm.AttackPlayer(((Ability)this["ATK"]).point);
    }

    public void Damage(ulong damage)
    {
        if (IsDead) return;

        currentHp -= (long)damage;

        if (currentHp <= 0) {
            IsDead = true;
        }

        CalculateHpBar();
    }

    public async void Move()
    {
        Vector3 playerPostion = GameManager.gm.PlayerPosition;
        while (Vector3.Distance(playerPostion, transform.position) > stopDistance) {
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.right);
            try { await Task.Delay(1, GameManager.tokenSource.Token); }
            catch (TaskCanceledException) { return; }
        }

        IsStop = true;
    }

    public async void Dead()
    {
        try { await GameManager.Delay(500); }
        catch (TaskCanceledException) { return; }
        Destroy(this.gameObject);
    }
}
