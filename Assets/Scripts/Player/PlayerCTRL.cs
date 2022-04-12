using System.Threading.Tasks;
using UnityEngine;

public interface IPlayerCalculate
{
    public void LevelUp();

    public void LevelUpSkill(string skillName);
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
    public void PlayerUpdated(PlayerInfoExtension playerInfo);
}

public class PlayerInfo : MobMethodExtension
{
    [Space(10)]
    public string nickName;
    public uint level;
    public uint skillPoint;

    public void SetInfo(GameData gameData)
    {
        this.level = gameData.playerLevel;
        this.skillPoint = gameData.playerSkillPoint;

        foreach (var item in gameData.abilityInfos) {
            ((Ability)this[item.abilityName]).SetAbility(item);
        }

        foreach (var item in gameData.itemInfos) {
            this[item.itemName] = new Item(item);
        }

        foreach (var item in gameData.skillInfos) {
            ((Skill)this[item.skillName]).SetSkill(item);
        }
    }
}

public abstract class PlayerInfoExtension : PlayerInfo
{
    [Space(10)]
    [SerializeField] protected float defConst;
    [SerializeField] protected float expInc;

    protected Skill enbledSkill;

    public ulong RequestExp => (ulong)Mathf.Pow(expInc * level, 2);

    public bool CanLevelUp => exp >= RequestExp;

    protected bool isDead;
    public abstract bool IsDead { get; set; }
    public abstract bool IsAttack { get; set; }
    public abstract bool IsMove { get; set; }
}

public class PlayerObservable : PlayerInfoExtension, IPlayerObservable
{
    private delegate void PlayerUpdatedDel(PlayerInfoExtension playerInfo);
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
        
        PlayerUpdated();
    }

    public void Start()
    {
        Item item = (Item)this["Soul"];
        foreach (var ability in abilities) {
            item.Subscribe(ability);
        }

        foreach (var _item in items) {
            _item.ItemUpdate();
        }

        foreach (var skill in skills) {
            skill.SkillUpdated();
            Subscribe(skill);
        }
    }

    public void Attack()
    {
        ulong damage = ((Ability)this["ATK"]).point;

        if (Random.Range(0f, 10000f) < ((Ability)this["CRIP"]).point) {
            damage += (ulong)(damage * (((Ability)this["CRID"]).point / 10000f));
        }

        AttackSound();
        GameManager.gm.AttackEnemy(damage);
    }

    public void Damage(ulong damage)
    {
        if (IsDead) return;

        float def = ((Ability)this["DEF"]).point;
        currentHp -= (long)((1 - def / (def + defConst)) * damage);

        if (currentHp <= 0) {
            IsDead = true;
        }

        CalculateHpBar();
    }

    public void Dead() => IsDead = true;

    public void LevelUp()
    {
        level++;
        skillPoint += 3;
        PlayerUpdated();
    }

    public void EnbledSkill(string skillName)
    {
        if (enbledSkill != null) {
            enbledSkill.DisableSkill();
            if (enbledSkill.skillName != skillName) return;
        }

        enbledSkill = (Skill)this[skillName];
        enbledSkill.EnbledSkill();
    }

    public async void LevelUpSkill(string skillName)
    {
        Skill skill = (Skill)this[skillName];

        isHoldButton = true;
        while (skill.canLevelUp && isHoldButton) {
            skillPoint -= skill.RequestSkillPoint;
            skill.LevelUp();
            PlayerUpdated();
            try { await GameManager.Delay(100); }
            catch (TaskCanceledException) { return; }
        }
    }

    public bool isHoldButton { get; set; }
    public async void LevelUpAbility(string abilityName)
    {
        Ability ability = (Ability)this[abilityName];
        Item item = (Item)this["Soul"];

        isHoldButton = true;
        while (ability.canLevelUp && isHoldButton) {
            item.Count -= ability.RequestSoul;
            ability.LevelUp();
            item.ItemUpdate();
            try { await GameManager.Delay(100); }
            catch (TaskCanceledException) { return; }
        }

        PlayerUpdated();
    }
}
