using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponObserver
{
    public void WeaponUpdate(WeaponObservable weaponExtension);
}

public interface IWeaponObservable
{
    public void Subscribe(IWeaponObserver observer);

    public void Unsubscribe(IWeaponObserver observer);

    public void WeaponUpdate();
}

[System.Serializable]
public enum WeaponRate
{
    S = 0,
    A = 1,
    B = 2,
    C = 3,
    D = 4,
    E = 5,
    F = 6,
}

[System.Serializable]
public class WeaponInfo : ItemInfo
{
    public uint level;
    public bool canLevelUp;
    public bool isUnlock;
    public bool isHold;

    public WeaponInfo() { }

    public WeaponInfo(WeaponInfo weaponInfo) => SetInfo(weaponInfo);

    public void SetInfo(WeaponInfo weaponInfo)
    {
        this.itemName = weaponInfo.itemName;
        this.count = weaponInfo.count;
        this.level = weaponInfo.level;
        this.canLevelUp = weaponInfo.canLevelUp;
        this.isUnlock = weaponInfo.isUnlock;
        this.isHold = weaponInfo.isHold;
    }
}

[System.Serializable]
public class WeaponExtension : WeaponInfo, IItemObserver
{
    [Space(10)]
    public string weaponTitle;
    
    [TextArea(5, 50)]
    public string weaponDescription;

    public WeaponRate weaponRate;

    [Space(10)]
    [SerializeField] private long startPoint;
    [SerializeField] private long startSoul;
    [SerializeField] private uint startCount;

    [Space(10)]
    [SerializeField] private float pointInc;
    [SerializeField] private float soulInc;
    [SerializeField] private float countInc;

    [Space(10)]
    public Sprite weaponIcon;

    public long Point => (long)Mathf.Pow(pointInc * level, 2) + startPoint;

    public long RequestSoul => (long)Mathf.Pow(soulInc * level, 2) + startSoul;

    public uint RequestCount => (uint)Mathf.Pow(countInc * level, 2) + startCount;

    public void ItemUpdate(Item item)
    {
        if (item.itemName == "Soul") {
            canLevelUp = item.Count >= RequestSoul && this.count >= RequestCount;
        }
        else throw new System.ArgumentException();
    }
}

public class WeaponObservable : WeaponExtension, IWeaponObservable
{
    private delegate void WeaponUpdateDel(WeaponObservable weaponExtension);
    private WeaponUpdateDel weaponUpdateDel;

    public void WeaponUpdate()
    {
        if (weaponUpdateDel != null) weaponUpdateDel.Invoke(this);
    }

    public void Subscribe(IWeaponObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        weaponUpdateDel += observer.WeaponUpdate;
    }

    public void Unsubscribe(IWeaponObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        weaponUpdateDel -= observer.WeaponUpdate;
    }
}

[System.Serializable]
public class Weapon : WeaponObservable
{
    public void LevelUp()
    {
        if (canLevelUp) {
            PlayerCTRL playerCTRL = GameObject.FindObjectOfType<PlayerCTRL>();

            Item soul = ((Item)playerCTRL["Soul"]);
            soul.Count -= RequestSoul;
            count -= RequestCount;

            level++;
            canLevelUp = soul.Count >= RequestSoul && this.count >= RequestCount;
        }

        WeaponUpdate();
    }

    public void Unlock()
    {
        isUnlock = true;
        WeaponUpdate();
    }
}