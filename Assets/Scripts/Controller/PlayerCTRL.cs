using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IPlayerCalculate
{
    public void LevelUp();
    public void LevelUpAbility(string abilityName);
    public void LevelUpSkill(string skillName);
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
    public uint level;
    public uint skillPoint;

    [Space(10)]
    public List<Skill> skills;
    public List<Weapon> weapons;

    public override object this[string name] { 
        get {
            foreach (var item in skills) {
                if (item.skillName == name) {
                    return item;
                }
            }
            foreach (var item in weapons) {
                if (item.itemName == name) {
                    return item;
                }
            }
            return base[name];
        }
        set {
            for (int i = 0; i < skills.Count; i++) {
                if (skills[i].skillName == name) {
                    skills[i] = (Skill)value;
                    return;
                }
            }
            for (int i = 0; i < weapons.Count; i++) {
                if (weapons[i].itemName == name) {
                    weapons[i] = (Weapon)value;
                    return;
                }
            }
            base[name] = value;
        }
    }

    public void SetInfo(GameData gameData)
    {
        this.exp = gameData.playerExp;
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

        foreach (var item in gameData.weapons) {
            ((WeaponInfo)this[item.itemName]).SetInfo(item);
        }
    }
}

public abstract class PlayerInfoExtension : PlayerInfo
{
    [Space(10)]
    [SerializeField] protected float defConst;
    [SerializeField] protected float expInc;

    [HideInInspector] public Weapon currentWeapon;

    public long RequestExp => (long)Mathf.Pow(expInc * level, 2);

    public bool CanLevelUp => exp >= RequestExp;

    protected bool isDead;

    public long AttackDamage {
        get {
            long damage = ((Ability)this["ATK"]).point;

            if (currentWeapon != null) {
                damage += (long)(damage * (currentWeapon.Point / 10000f));
            }

            return damage;
        }
    }

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
            isDead = false;
        }
    }

    public override bool IsAttack {
        get => animator.GetBool("IsAttack");
        set {
            animator.SetBool("IsAttack", value);
            PlayerUpdated();
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

public class PlayerCTRL : PlayerObservable, IEnemyObserver, IPlayerCalculate, IMobAction
{
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        Item item = (Item)this["Soul"];
        foreach (var ability in abilities) {
            item.Subscribe(ability);
        }

        foreach (var weapon in weapons) {
            item.Subscribe(weapon);

            if (weapon.isHold) {
                currentWeapon = weapon;
            }
        }

        foreach (var skill in skills) {
            Subscribe(skill);
        }

        foreach (var _item in items) {
            _item.ItemUpdate();
        }

        IsMove = true;
        PlayerUpdated();
    }

    public async void EnemyUpdated(EnemyObservable enemyCTRL)
    {
        if (enemyCTRL.IsDead) {
            enemyCTRL.Unsubscribe(this);

            AddItem("Soul", ((Item)enemyCTRL["Soul"]).Count);

            exp += enemyCTRL.exp;
            if (CanLevelUp) {
                LevelUp();
            }
            
            IsAttack = false;

            try {
                await GameManager.gm.Delay((int)(1200 / Time.timeScale));
            }
            catch (TaskCanceledException) { 
                return; 
            }

            totalDamage = 0;

            IsMove = true;
        }
        else if (enemyCTRL.IsStop && IsMove) {
            IsMove = false;

            try {
                await GameManager.gm.Delay((int)(300 / Time.timeScale)); 
            }
            catch (TaskCanceledException) {
                return; 
            }

            IsAttack = true;
        }
    }

    public void Attack()
    {
        long damage = AttackDamage;

        if (Random.Range(0f, 10000f) < ((Ability)this["CRIP"]).point) {
            damage += (long)(damage * (((Ability)this["CRID"]).point / 10000f));
        }

        AttackSound();

        EnemyCTRL enemyCTRL = FindObjectOfType<EnemyCTRL>();
        if(enemyCTRL != null) {
            enemyCTRL.Damage(damage);
        }
    }

    public void Damage(long damage)
    {
        if (IsDead) return;

        float def = ((Ability)this["DEF"]).point;
        totalDamage += (long)((1 - def / (def + defConst)) * damage);
        
        HitEffect();

        if (totalDamage >= ((Ability)this["HP"]).point) {
            IsDead = true;
            return;
        }

        PlayerUpdated();
    }

    public void LevelUp()
    {
        level++;
        skillPoint += 3;
        PlayerUpdated();
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

            try { 
                await GameManager.gm.Delay(50); 
            }
            catch (TaskCanceledException) { 
                return;
            }
        }

        PlayerUpdated();
        GameDataManager.dataManager.SaveGameData();
    }

    public async void LevelUpSkill(string skillName)
    {
        Skill skill = (Skill)this[skillName];

        isHoldButton = true;
        while (skill.canLevelUp && isHoldButton) {
            skillPoint -= skill.RequestSkillPoint;
            skill.LevelUp();
            PlayerUpdated();

            try {
                await GameManager.gm.Delay(100);
            }
            catch (TaskCanceledException) {
                return;
            }
        }

        GameDataManager.dataManager.SaveGameData();
    }

    public void AddItem(string name, long count)
    {
        object obj = this[name];

        try
        {
            Item item = (Item)obj;
            item.Count += count;
        }
        catch (System.InvalidCastException)
        {
            Weapon weapon = (Weapon)obj;

            if (!weapon.isUnlock)
            {
                weapon.Unlock();
            }

            weapon.count += count;
            weapon.WeaponUpdate();
        }

        GameDataManager.dataManager.SaveGameData();
    }
}
