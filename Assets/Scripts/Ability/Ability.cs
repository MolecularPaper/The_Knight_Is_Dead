using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class AbilityInfo
{
    [HideInInspector]
    public string abilityName = "";
    public uint level = 1;
    public long point = 0;
    public long startSoul = 0;
    public bool canLevelUp = false;

    public AbilityInfo() { }

    public AbilityInfo(AbilityInfo abilityInfo) => SetAbility(abilityInfo);

    public void SetAbility(AbilityInfo abilityInfo)
    {
        this.canLevelUp = abilityInfo.canLevelUp;
        this.abilityName = abilityInfo.abilityName;
        this.level = abilityInfo.level;
        this.point = abilityInfo.point;
    }
}

[System.Serializable]
public class AbilityExtension : AbilityInfo
{
    [SerializeField] protected long maxPoint;

    [Space(10)]
    [SerializeField] protected float pointInc;
    [SerializeField] protected float soulInc;
    [SerializeField] protected bool isFixInc;

    [Space(10)]
    public Sprite ablilityIcon;
    public string ablilityTitle;

    [TextArea(5, 50)]
    public string ablilityDescription;

    public long UpPoint {
        get {
            if (isFixInc) {
                return (long)pointInc;
            }
            else {
                return (long)Mathf.Pow(pointInc * level, 2) + 1;
            }
        }
    }

    public long NextPoint => point + UpPoint;

    public long RequestSoul => (long)Mathf.Pow(soulInc * level, 2) + startSoul;
}

public interface AbilityCalculate
{
    public void LevelUp();
}

public interface IAbilityObservable
{
    public void Subscribe(IAbilityObserver observer);

    public void Unsubscribe(IAbilityObserver observer);

    public void AbilityUpdated();
}

public interface IAbilityObserver
{
    public void AbilityUpdated(AbilityExtension abilityInfo);
}

public class AbilityObservable : AbilityExtension, IAbilityObservable
{
    private delegate void AbilityUpdatedDel(AbilityExtension abilityInfo);
    private AbilityUpdatedDel abilityUpdatedDel;

    public void AbilityUpdated()
    {
        if (abilityUpdatedDel != null) abilityUpdatedDel.Invoke(this);
    }

    public void Subscribe(IAbilityObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        abilityUpdatedDel += observer.AbilityUpdated;
    }

    public void Unsubscribe(IAbilityObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        abilityUpdatedDel -= observer.AbilityUpdated;
    }
}

[System.Serializable]
public class Ability : AbilityObservable, AbilityCalculate, IItemObserver
{
    public Ability(string abilityName, long point)
    {
        this.abilityName = abilityName;
        this.point = point;
    }

    public void LevelUp()
    {
        point += UpPoint;
        level++;
    }

    public void ItemUpdate(Item item)
    {
        if (item.itemName == "Soul") {
            canLevelUp = item.Count >= RequestSoul;
            AbilityUpdated();
        }
        else throw new System.ArgumentException();
    }
}