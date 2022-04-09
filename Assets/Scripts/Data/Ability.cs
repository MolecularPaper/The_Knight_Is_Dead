using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class AbilityInfo
{
    [HideInInspector]
    public bool canLevelUp = false;
    public string abilityName = "";
    public string sign = "";
    public uint level = 0;
    public ulong point = 0;

    public void SetAbility(AbilityInfo abilityInfo)
    {
        this.canLevelUp = abilityInfo.canLevelUp;
        this.abilityName = abilityInfo.abilityName;
        this.sign = abilityInfo.sign;
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

    public ulong RequestSoul => (ulong)Mathf.Pow(soulInc * level, 2);
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

[System.Serializable]
public class Ability : AbilityExtension, AbilityCalculate, IAbilityObservable, IItemObserver
{
    public Ability(string abilityName, ulong point)
    {
        this.abilityName = abilityName;
        this.point = point;
    }
    public Ability(AbilityInfo abilityInfo)
    {
        this.canLevelUp = abilityInfo.canLevelUp;
        this.abilityName = abilityInfo.abilityName;
        this.sign = abilityInfo.sign;
        this.level = abilityInfo.level;
        this.point = abilityInfo.point;
    }

    private delegate void AbilityUpdatedDel(AbilityExtension abilityInfo);
    private AbilityUpdatedDel abilityUpdatedDel;

    public void LevelUp()
    {
        level++;
        point += UpPoint;
    }

    public void AbilityUpdated()
    {
        if(abilityUpdatedDel != null) abilityUpdatedDel.Invoke(this);
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

    public void ItemUpdate(Item item)
    {
        if (item.itemName == "Soul") {
            canLevelUp = item.Count >= RequestSoul;
            AbilityUpdated();
        }
        else throw new System.ArgumentException();
    }
}