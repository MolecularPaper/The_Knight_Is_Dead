using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class AbilityInfo
{
    [HideInInspector]
    public bool canLevelUp = false;
    public string abilityName = "";
    public uint level = 1;
    public ulong point = 0;
    public ulong startSoul = 0;

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
    [SerializeField] protected ulong maxPoint;

    [Space(10)]
    [SerializeField] protected float pointInc;
    [SerializeField] protected float soulInc;
    [SerializeField] protected bool isFixInc;

    public ulong UpPoint {
        get {
            if (isFixInc) {
                return (ulong)pointInc;
            }
            else {
                return (ulong)Mathf.Pow(pointInc * level, 2) + 1;
            }
        }
    }

    public ulong NextPoint => point + UpPoint;

    public ulong RequestSoul => (ulong)Mathf.Pow(soulInc * level, 2) + startSoul;
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
    public Ability(string abilityName, ulong point)
    {
        this.abilityName = abilityName;
        this.point = point;
    }

    public void LevelUp()
    {
        level++;
        point += UpPoint;
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