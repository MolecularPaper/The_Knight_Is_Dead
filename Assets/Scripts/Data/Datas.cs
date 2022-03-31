using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[Serializable]
public enum AbilityType
{
    HP = 0,
    ATK = 1,
    DEF = 2,
    LUK = 3,
    CRID = 4,
    CRIP = 5,
}

[Serializable]
public struct Ability
{
    public Ability(long point, int maxLevel = 0, float pwidth = 0, float swidth = 0, string sign = "")
    {
        this.point = point;
        this.pwidth = pwidth;
        this.swidth = swidth;
        this.maxLevel = maxLevel;
        this.sign = sign;
        this.level = 1;
    }

    public long point;
    public int level;
    public int maxLevel;
    
    public string sign { get; }

    private float pwidth;
    private float swidth;

    public long upPoint { get => (long)Mathf.Pow(pwidth * level, 2) + 1; }
    public long requestSoul { get => (long)Mathf.Pow(swidth * level, 2); }

    public long nextPoint { get => point + upPoint; }
}

[Serializable]
public class EntityData
{
    [Header("Info")]
    public long currentHp;

    [Header("State")]
    public Dictionary<AbilityType, Ability> abilities = new Dictionary<AbilityType, Ability>();
    /// <summary>
    /// 현재 가지고 있는 영혼 개수
    /// </summary>
    public long soul = 0;

    public EntityData() { }
    public EntityData(EntityData entityData)
    {
        this.abilities = entityData.abilities;
        this.soul = entityData.soul;
    }
}


[Serializable]
public class PlayerData : EntityData
{
    //재화
    /// <summary>
    /// 플레이어가 가지고 있는 다이아몬드 개수
    /// </summary>
    public int diamond = 0;
    /// <summary>
    ///  플레이어가 가지고 있는 크리스탈 개수
    /// </summary>
    public int crystal = 0;

    public bool CanLevelUp(long requestSoul) => soul >= requestSoul;
}

[Serializable]
public enum ItemType
{
    Soul,
    Diamond,
    Crystal,
}

[Serializable]
public class ItemInfo
{
    public ItemType itemType;
    public CanvasGroup group;
    public TextMeshProUGUI count;
    public bool isActive;
}

[Serializable]
public class InceaseAbillity
{
    public string title;
    public TextMeshProUGUI level;
    public TextMeshProUGUI description;
    public Button levelUpButton;
    public TextMeshProUGUI soul;
}