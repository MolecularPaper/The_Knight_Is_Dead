using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyCTRL : Entity
{
    [HideInInspector] public EntityData enemyData;

    private float hpBarScale;


    protected override void Awake()
    {
        base.Awake();
        
    }

    public void Reset()
    {
        enemyData.currentHp = (long)enemyData.abilities[AbilityType.HP].point;
        hpBarScale = hpBar.localScale.x;
        Move();
    }

    void Update()
    {
        if (!GameManager.gm.player) return;
        animator.SetBool("IsAttack", GameManager.gm.canAction);
    }

    private async void Move()
    {
        while (true) {
            if (!GameManager.gm.canAction) {
                transform.Translate(Vector3.right * 5 * Time.deltaTime);
                await Task.Delay(1);
            }
            else {
                return;
            }
        }
    }

    public override void Attack()
    {
        if (!GameManager.gm.player) return;
        GameManager.gm.player.Damage((long)enemyData.abilities[AbilityType.ATK].point);
    }

    public override void Damage(long damage)
    {
        enemyData.currentHp -= damage;
        base.Damage(damage);

        if (enemyData.currentHp <= 0) {
            Dead();
        }
    }

    protected override void Dead()
    {
        base.Dead();

        GameManager.gm.EnemyDead();
        GameManager.gm.AddSoul(enemyData.soul);
    }

    protected override void CalHpBar() => hpBar.localScale = new Vector3(Mathf.Lerp(0, hpBarScale, (float)enemyData.currentHp / enemyData.abilities[AbilityType.HP].point), hpBar.localScale.y, hpBar.localScale.z);
}
