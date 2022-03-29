using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyCTRL : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [HideInInspector] public EntityData enemyData;
    [SerializeField] private Transform hpBar;
    [SerializeField] private Color hitColor;

    private float hpBarScale;


    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        hpBarScale = hpBar.localScale.x;
    }

    void Update()
    {
        if (GameManager.gm.player) return;
        animator.SetBool("IsAttack", GameManager.gm.canAction);
    }

    public void AttackPlayer()
    {
        if (GameManager.gm.player) return;
        GameManager.gm.player.Damage(enemyData.atkPoint);
    }

    public void Damage(int damage)
    {
        _ = HitEffect();
        enemyData.currentHp -= damage;

        if (enemyData.currentHp <= 0) {
            Dead();
        }

        hpBar.localScale = new Vector3(Mathf.Lerp(0, hpBarScale, (float)enemyData.currentHp / enemyData.hpPoint), hpBar.localScale.y, hpBar.localScale.z);
    }

    public async Task HitEffect()
    {
        Color originColor = spriteRenderer.color;
        spriteRenderer.color = hitColor;

        await Task.Delay(100);

        spriteRenderer.color = originColor;
    }

    public void Dead()
    {
        hpBar.gameObject.SetActive(false);
        animator.SetTrigger("Dead");

        _ = GameManager.gm.EnemyDead();
        GameManager.gm.AddSoul(enemyData.soul);
    }

    public void Destroy() => Destroy(this.gameObject);
}
