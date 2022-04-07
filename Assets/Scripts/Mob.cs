using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface IMobAction
{
    public void Attack();
    public void Damage(ulong damage);
    public void Dead();
}

public interface IMobEffect
{
    public void AttackSound();
    public void HitSound();
    public void HitEffect();
}

public interface IMobCalculate
{
    public void SetCurrentHP();
    public void CalculateHpBar();
}

public interface IMobCTRL
{
    public void AddAbility(Ability ability);
    public Item GetItem(string itemName);
    public void AddItem(Item item);
}

public class MobInfo : MonoBehaviour
{
    [SerializeField] protected List<Item> items;
    [SerializeField] protected List<Ability> abilities;
    protected long currentHp;
}

public class MobExtension : MobInfo
{
    protected SpriteRenderer spriteRenderer;
    protected Vector3 hpBarScale;
    protected Animator animator;

    [Space(10)]
    [SerializeField] protected Transform hpBar;
    [SerializeField] protected Color hitColor;
    [SerializeField] protected AudioClip attakSound;
    [SerializeField] protected AudioClip hitSound;
}

public class MobMethodExtension : MobExtension, IMobEffect, IMobCalculate, IMobCTRL
{
    public Ability this[string abilityName] {
        get {
            foreach (var item in abilities) {
                if (item.abilityName == abilityName) {
                    return item;
                }
            }
            throw new System.ArgumentNullException();
        }
    }
    public void AddAbility(Ability ability) => abilities.Add(ability);

    public Item GetItem(string itemName)
    {
        foreach (var item in items) {
            if (item.itemName == itemName) {
                return item;
            }
        }
        throw new System.ArgumentNullException();
    }
    public void AddItem(Item item) => items.Add(item);

    public void AttackSound() => SoundManager.sound.PlaySE(attakSound);

    public async void HitEffect()
    {
        spriteRenderer.color = hitColor;
        await Task.Delay(100);
        spriteRenderer.color = Color.white;
    }

    public void HitSound() => SoundManager.sound.PlaySE(hitSound);

    public void SetCurrentHP() => currentHp = (long)this["HP"].point;

    public void CalculateHpBar()
    {
        hpBar.localScale =
            new Vector3(
                Mathf.Lerp(0, hpBarScale.x, (float)currentHp / this["HP"].point),
                hpBarScale.y,
                hpBarScale.z);
    }
}