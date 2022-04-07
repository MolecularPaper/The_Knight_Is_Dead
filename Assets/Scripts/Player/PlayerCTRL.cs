using System.Threading.Tasks;
using UnityEngine;

public interface IPlayerCalculate
{
    public void LevelUpAbility(string abilityName);
}

public interface IPlayerObservable
{
    public void Subscribe(IPlayerObserver observer);

    public void Unsubscribe(IPlayerObserver observer);

    public void PlayerUpdated();
}

public interface IPlayerObserver
{
    public void PlayerUpdated(PlayerInfo playerInfo);
}

public abstract class PlayerInfo : MobMethodExtension
{
    [SerializeField] 
    protected float defConst;

    protected bool isDead;
    public abstract bool IsDead { get; set; }
    public abstract bool IsAttack { get; set; }
    public abstract bool IsMove { get; set; }
}

public class PlayerObservable : PlayerInfo, IPlayerObservable
{
    private delegate void PlayerUpdatedDel(PlayerInfo playerInfo);
    private PlayerUpdatedDel playerUpdatedDel;
    
    public override bool IsDead {
        get => isDead;
        set {
            if (isDead) return;
            isDead = value;
            if (IsDead) animator.SetTrigger("Dead");
            PlayerUpdated();
        }
    }

    public override bool IsAttack {
        get => animator.GetBool("IsAttack");
        set {
            animator.SetBool("IsAttack", value);
        }
    }

    public override bool IsMove {
        get => animator.GetBool("IsMove");
        set {
            animator.SetBool("IsMove", value);
            PlayerUpdated();
        }
    }

    public void Subscribe(IPlayerObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        playerUpdatedDel += observer.PlayerUpdated;
    }

    public void Unsubscribe(IPlayerObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        playerUpdatedDel -= observer.PlayerUpdated;
    }

    public void PlayerUpdated()
    {
        if(playerUpdatedDel != null) playerUpdatedDel.Invoke(this);
    }
}

public class PlayerCTRL : PlayerObservable, IPlayerCalculate, IMobAction
{
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetCurrentHP();
        hpBarScale = hpBar.localScale;

        Item item = GetItem("Soul");
        foreach (var ability in abilities) {
            item.Subscribe(ability);
        }

        foreach (var _item in items) {
            _item.ItemUpdate();
        }

        PlayerUpdated();
    }

    public void Attack()
    {
        ulong damage = this["ATK"].point;

        if (Random.Range(0f, 10000f) < this["CRIP"].point) {
            damage += (ulong)(damage * (this["CRID"].point / 100f));
        }

        AttackSound();
        GameManager.gm.AttackEnemy(damage);
    }

    public void Damage(ulong damage)
    {
        if (IsDead) return;

        float def = this["DEF"].point;
        currentHp -= (long)((1 - def / (def + defConst)) * damage);

        if (currentHp <= 0) {
            IsDead = true;
        }
    }

    public void Dead() => IsDead = true;

    public bool isHoldButton { get; set; }
    public async void LevelUpAbility(string abilityName)
    {
        Ability ability = this[abilityName];
        Item item = GetItem("Soul");

        isHoldButton = true;
        while (ability.canLevelUp && isHoldButton) {
            item.Count -= ability.RequestSoul;
            ability.LevelUp();
            item.ItemUpdate();
            await Task.Delay(100);
        }

        PlayerUpdated();
    }
}
