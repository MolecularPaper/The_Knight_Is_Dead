using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class PlayerCTRL : Entity
{
    public PlayerData playerData;

    public bool isMove {
        get => animator.GetBool("IsMove");
        set => animator.SetBool("IsMove", value);
    }

    public bool isAttack {
        get => animator.GetBool("IsAttack");
        set => animator.SetBool("IsAttack", value);
    }

    protected override void Awake()
    {
        base.Awake();
        playerData.currentHp = playerData.hpPoint;
        Move();
    }

    void Update()
    {
        isAttack = GameManager.gm.canAction;
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
        playerData.currentHp = playerData.hpPoint;
        CalHpBar();
    }

    public override void Attack()
    {
        if (!GameManager.gm.currentEnemy) return;

        long totalDamage = playerData.atkPoint;

        if (Random.Range(0f, 100f) < playerData.cripPoint) {
            totalDamage += (int)(playerData.atkPoint * playerData.cridPoint);
        }

        GameManager.gm.currentEnemy.Damage(totalDamage);
    }

    public override void Damage(long damage)
    {
        playerData.currentHp -= (damage - playerData.defPoint) < 0 ? 0 : (damage - playerData.defPoint);
        base.Damage(damage);

        if (playerData.currentHp <= 0) {
            Dead();
        }
    }

    protected override void Dead()
    {
        base.Dead();
        GameManager.gm.PlayerDead();
    }

    protected override void CalHpBar() => hpBar.localScale = new Vector3((float) playerData.currentHp / playerData.hpPoint, hpBar.localScale.y, hpBar.localScale.z);
}
