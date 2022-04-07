using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public abstract class AbilityInfo
{
    [HideInInspector]
    public bool canLevelUp;
    public string abilityName;
    public string sign;
    public uint level;
    public ulong point;

    public abstract ulong UpPoint { get; }

    public abstract ulong RequestSoul { get; }

    public abstract ulong NextPoint { get; }
}

public class AbilityExtension : AbilityInfo
{
    [SerializeField] protected ulong maxPoint;

    [Space(10)]
    [SerializeField] protected float pointInc;
    [SerializeField] protected float soulInc;
    [SerializeField] protected bool isFixInc;

    public override ulong UpPoint {
        get {
            if (isFixInc) {
                return (ulong)pointInc;
            }
            else {
                return (ulong)Mathf.Pow(pointInc * level, 2) + 1;
            }
        }
    }

    public override ulong NextPoint => point + UpPoint;

    public override ulong RequestSoul => (ulong)Mathf.Pow(soulInc * level, 2);
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
    public void AbilityUpdated(AbilityInfo abilityInfo);
}

[System.Serializable]
public class Ability : AbilityExtension, AbilityCalculate, IAbilityObservable, IItemObserver
{
    public Ability(string abilityName, ulong point)
    {
        this.abilityName = abilityName;
        this.point = point;
    }

    private delegate void AbilityUpdatedDel(AbilityInfo abilityInfo);
    private AbilityUpdatedDel abilityUpdatedDel;

    public void LevelUp()
    {
        level++;
        point += UpPoint;
    }

    public void AbilityUpdated()
    {
        abilityUpdatedDel.Invoke(this);
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
            Debug.Log($"{abilityName}, {canLevelUp}");
            AbilityUpdated();
        }
        else throw new System.ArgumentException();
    }
}