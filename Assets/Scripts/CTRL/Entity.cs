using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// 메인 캐릭터 애니메이터 컴포넌트
    /// </summary>
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    [SerializeField] protected Transform hpBar;
    [SerializeField] protected Color hitColor;

    [Space(10)]
    [SerializeField] protected AudioClip[] hitSounds;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(!animator) animator = GetComponentInChildren<Animator>();
        if(!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public abstract void Attack();

    public virtual void Damage(long damage)
    {
        HitEffect();
        HitSound();
        CalHpBar();
    }

    protected void HitSound()
    {
        if (hitSounds == null || hitSounds.Length == 0) return;

        SoundManager.sound.PlaySE(hitSounds[Random.Range(0, hitSounds.Length - 1)]);
    }

    protected async void HitEffect()
    {
        Color originColor = spriteRenderer.color;
        spriteRenderer.color = hitColor;

        await Task.Delay(100);

        spriteRenderer.color = originColor;
    }

    protected virtual void Dead()
    {
        hpBar.gameObject.SetActive(false);
        animator.SetTrigger("Dead");
    }

    protected abstract void CalHpBar();

    public void Destroy() => Destroy(this.gameObject);
}
