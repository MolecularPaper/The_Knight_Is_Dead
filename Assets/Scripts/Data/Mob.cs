using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface IMobAction
{
    public void Attack();
    public void Damage(ulong damage);
}

public interface IMobEffect
{
    public void AttackSound();
    public void HitSound();
    public void HitEffect();
}

public interface IMobCTRL
{
    public void AddAbility(Ability ability);
    public void AddItem(Item item);
}

public class MobInfo : MonoBehaviour
{
    public List<Item> items;
    public List<Ability> abilities;
    public long totalDamage;
    public ulong exp;

    public object this[string name] {
        get {
            foreach (var item in items) {
                if (item.itemName == name) {
                    return item;
                }
            }
            foreach (var item in abilities) {
                if (item.abilityName == name) {
                    return item;
                }
            }
            throw new System.ArgumentNullException();
        }
        set {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].itemName == name) {
                    items[i] = (Item)value;
                    return;
                }
            }
            for (int i = 0; i < abilities.Count; i++) {
                if (abilities[i].abilityName == name) {
                    abilities[i] = (Ability)value;
                    return;
                }
            }
            throw new System.ArgumentNullException();
        }
    }
}

public class MobExtension : MobInfo
{
    protected SpriteRenderer spriteRenderer;
    protected Vector3 hpBarScale;
    protected Animator animator;

    [Space(10)]
    [SerializeField] protected Color hitColor;
    [SerializeField] protected AudioClip attakSound;
    [SerializeField] protected AudioClip hitSound;
}

public class MobMethodExtension : MobExtension, IMobEffect, IMobCTRL
{
    public void AddAbility(Ability ability) => abilities.Add(ability);
    public void AddItem(Item item) => items.Add(item);

    public void AttackSound() => SoundManager.sound.PlaySE(attakSound);

    public async void HitEffect()
    {
        spriteRenderer.color = hitColor;

        try {
            await GameManager.gm.Delay(100);
        }
        catch (TaskCanceledException) { 
            return; 
        }

        try { spriteRenderer.color = Color.white; }
        catch { return; }
    }

    public void HitSound() => SoundManager.sound.PlaySE(hitSound);
}