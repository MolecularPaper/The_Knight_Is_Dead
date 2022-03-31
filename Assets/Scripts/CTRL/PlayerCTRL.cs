using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class PlayerCTRL : Entity
{
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private IncreaseData increaseData;

    public PlayerData playerData;

    public bool isMove {
        get => animator.GetBool("IsMove");
        set => animator.SetBool("IsMove", value);
    }

    public bool isAttack {
        get => animator.GetBool("IsAttack");
        set => animator.SetBool("IsAttack", value);
    }

    public bool isDead = true;

    public void Reset()
    {
        animator.Rebind();
        isDead = false;
        playerData.currentHp = playerData.abilities[AbilityType.HP].point;
        hpBar.gameObject.SetActive(true);
        CalHpBar();
    }

    protected override void Awake()
    {
        base.Awake();
        ResetAbility();
    }

    void Update()
    {
        isAttack = GameManager.gm.canAction;
    }

    private void ResetAbility()
    {
        playerData.abilities = new Dictionary<AbilityType, Ability>();
        playerData.abilities.Add(AbilityType.HP, new Ability(100, 12000, increaseData.hpIncreseWidth, increaseData.hpSoulIncreseWidth));
        playerData.abilities.Add(AbilityType.ATK, new Ability(1, 12000, increaseData.atkIncreseWidth, increaseData.atkSoulIncreseWidth));
        playerData.abilities.Add(AbilityType.DEF, new Ability(1, 12000, increaseData.defIncreseWidth, increaseData.defSoulIncreseWidth));
        playerData.abilities.Add(AbilityType.LUK, new Ability(0, 12000, increaseData.lukIncreseWidth, increaseData.lukSoulIncreseWidth, "%"));
        playerData.abilities.Add(AbilityType.CRIP, new Ability(0, 12000, increaseData.cripIncreseWidth, increaseData.cripSoulIncreseWidth, "%"));
        playerData.abilities.Add(AbilityType.CRID, new Ability(50, 12000, increaseData.cridIncreseWidth, increaseData.cridSoulIncreseWidth, "%"));
    }

    public async void Stop()
    {
        while (true) {
            if (GameManager.gm.canAction) {
                isMove = false;
                return;
            }
            await Task.Delay(1);
        }
    }

    public void Move()
    {
        isMove = true;
        isAttack = false;
        playerData.currentHp = (long)playerData.abilities[AbilityType.HP].point;
        CalHpBar();
    }

    public override void Attack()
    {
        if (!GameManager.gm.currentEnemy) return;

        long totalDamage = playerData.abilities[AbilityType.ATK].point;

        if (Random.Range(0f, 10000f) < playerData.abilities[AbilityType.CRIP].point) {
            totalDamage += (int)(totalDamage * playerData.abilities[AbilityType.CRID].point);
        }

        audioSource.PlayOneShot(attackSound);
        GameManager.gm.currentEnemy.Damage(totalDamage);
    }

    public override void Damage(long damage)
    {
        if (isDead) return;

        float def = playerData.abilities[AbilityType.DEF].point;

        print("데미지받음: " + (long)((1 - def / (def + increaseData.defConst)) * damage));
        playerData.currentHp -= (long)((1 - def / (def + increaseData.defConst)) * damage);
        base.Damage(damage);

        if (playerData.currentHp <= 0) {
            base.Dead();
            isDead = true;
        }
    }

    public void DeadEvent() => GameManager.gm.PlayerDead();

    protected override void CalHpBar() => hpBar.localScale = new Vector3((float)playerData.currentHp / playerData.abilities[AbilityType.HP].point, hpBar.localScale.y, hpBar.localScale.z);
}
