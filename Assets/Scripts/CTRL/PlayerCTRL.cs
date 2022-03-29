using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class PlayerCTRL : MonoBehaviour
{
    public PlayerData playerData;

    /// <summary>
    /// 메인 캐릭터 애니메이터 컴포넌트
    /// </summary>
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform hpBar;
    [SerializeField] private Color hitColor;

    public bool isMove {
        get => animator.GetBool("IsMove");
        set => animator.SetBool("IsMove", value);
    }

    public bool isAttack {
        get => animator.GetBool("IsAttack");
        set => animator.SetBool("IsAttack", value);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerData.currentHp = playerData.hpPoint;
        Move();
    }

    void Update()
    {
        isAttack = GameManager.gm.canAction;
    }

    public async Task Stop()
    {
        while (true) {
            if (GameManager.gm.isStopDistance) {
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

    public void AttackEnemy()
    {
        if (!GameManager.gm.currentEnemy) return;

        int totalDamage = playerData.atkPoint;

        if (Random.Range(0f, 100f) < playerData.cripPoint) {
            totalDamage += (int)(playerData.atkPoint * playerData.cridPoint);
        }

        GameManager.gm.currentEnemy.Damage(totalDamage);
    }

    public void Damage(int damage)
    {
        _ = HitEffect();
        playerData.currentHp -= (damage - playerData.defPoint);

        if (playerData.currentHp <= 0) {
            Dead();
        }

        CalHpBar();
    }
    public async Task HitEffect()
    {
        Color originColor = spriteRenderer.color;
        spriteRenderer.color = hitColor;

        await Task.Delay(100);

        spriteRenderer.color = originColor;
    }

    private void Dead()
    {
        hpBar.gameObject.SetActive(false);
        animator.SetTrigger("Dead");

        _ = GameManager.gm.PlayerDead();
    }

    public void CalHpBar() => hpBar.localScale = new Vector3((float)playerData.currentHp / playerData.hpPoint, hpBar.localScale.y, hpBar.localScale.z);
}
