using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponInfo : ItemInfo
{
    public uint level;
    public bool canLevelUp;

    public WeaponInfo() { }

    public WeaponInfo(WeaponInfo weaponInfo) => SetInfo(weaponInfo);

    public void SetInfo(WeaponInfo weaponInfo)
    {
        this.itemName = weaponInfo.itemName;
        this.count = weaponInfo.count;
        this.level = weaponInfo.level;
        this.canLevelUp = weaponInfo.canLevelUp;
    }
}

[System.Serializable]
public class WeaponExtension : WeaponInfo, IItemObserver
{
    [Space(10)]
    public string weaponTitle;
    
    [TextArea(5, 50)]
    public string weaponDescription;

    [Space(10)]
    [SerializeField] private ulong startPoint;
    [SerializeField] private ulong startSoul;
    [SerializeField] private uint startCount;

    [Space(10)]
    [SerializeField] private float pointInc;
    [SerializeField] private float soulInc;
    [SerializeField] private float countInc;

    [Space(10)]
    public Sprite weaponIcon;

    public ulong Point => (ulong)Mathf.Pow(pointInc * level, 2) + startPoint;

    public ulong RequestSoul => (ulong)Mathf.Pow(soulInc * level, 2) + startSoul;

    public uint RequestCount => (uint)Mathf.Pow(countInc * level, 2) + startCount;

    public void ItemUpdate(Item item)
    {
        if (item.itemName == "Soul") {
            canLevelUp = item.Count >= RequestSoul && this.count >= RequestCount;
        }
        else throw new System.ArgumentException();
    }
}

[System.Serializable]
public class Weapon : WeaponExtension
{
    public void LevelUp()
    {
        if (canLevelUp) {
            PlayerCTRL playerCTRL = GameObject.FindObjectOfType<PlayerCTRL>();
            ((Item)playerCTRL["Soul"]).Count -= RequestSoul;
            count -= RequestCount;
            level++;
        }
    }
}