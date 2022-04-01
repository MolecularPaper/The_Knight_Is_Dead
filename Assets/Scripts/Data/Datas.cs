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
    public Ability(long point, long maxPoint = 0, float pwidth = 0, float swidth = 0, string sign = "", bool fixedIncrease = false)
    {
        this.point = point;
        this.pwidth = pwidth;
        this.swidth = swidth;
        this.maxPoint = maxPoint;
        this.sign = sign;
        this.fixedIncrease = fixedIncrease;
        level = 1;
    }

    public long point;
    public int level;
    public long maxPoint;
    
    public string sign { get; }
    private float pwidth;
    private float swidth;
    private bool fixedIncrease;

    public long upPoint {
        get {
            if (fixedIncrease) {
                return (long)(pwidth * 100);
            }
            else {
                return (long)Mathf.Pow(pwidth * level, 2) + 1;
            }
        }
    }
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
        abilities = new Dictionary<AbilityType, Ability>();
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

    public PlayerData(GameSaveData playerSaveData)
    {
        soul = playerSaveData.soul;
        for (int i = 0; i < playerSaveData.abilityTypes.Count; i++) {
            abilities.Add(playerSaveData.abilityTypes[i], playerSaveData.abilities[i]);
        }
    }
}

[Serializable]
public class GameData
{
    public int stageIndex = 0;
    public int highestStageIndex = 0;

    public GameData() { }
    public GameData(GameSaveData gameSaveData)
    {
        stageIndex = gameSaveData.stageIndex;
        highestStageIndex = gameSaveData.highestStageIndex;
    }

    public void ReturnStage()
    {
        stageIndex = 0;
        stageIndex = Mathf.Clamp(stageIndex, 0, int.MaxValue);
    }

    public void SetHighestStage()
    {
        if (stageIndex > highestStageIndex)
            highestStageIndex = stageIndex;
    }
}

[Serializable]
public class GameSaveData
{
    //게임 데이터
    public int stageIndex = 0;
    public int highestStageIndex = 0;

    //플레이어 데이터
    public long soul;
    public int diamond;
    public int crystal;
    public List<AbilityType> abilityTypes = new List<AbilityType>();
    public List<Ability> abilities = new List<Ability>();

    public GameSaveData(PlayerData playerData, GameData gameData)
    {
        stageIndex = gameData.stageIndex;
        highestStageIndex = gameData.highestStageIndex;

        soul = playerData.soul;
        diamond = playerData.diamond;
        crystal = playerData.crystal;
        foreach (var item in playerData.abilities) {
            abilityTypes.Add(item.Key);
            abilities.Add(item.Value);
        }
    }
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
public class AbillityUI
{
    public string title;
    public TextMeshProUGUI level;
    public TextMeshProUGUI description;
    public Button levelUpButton;
    public TextMeshProUGUI soul;
}