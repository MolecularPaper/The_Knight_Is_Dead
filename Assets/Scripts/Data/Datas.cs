using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[Serializable]
public class EntityData
{
    [Header("Info")]
    public long currentHp;

    [Header("State")]
    public long hpPoint = 0;
    /// <summary>
    /// 공격력
    /// </summary>
    public long atkPoint = 0;
    /// <summary>
    /// 현재 가지고 있는 영혼 개수
    /// </summary>
    public long soul;

    public EntityData() { }
    public EntityData(EntityData entityData)
    {
        this.hpPoint = entityData.hpPoint;
        this.currentHp = entityData.currentHp;
        this.atkPoint = entityData.atkPoint;
        this.soul = entityData.soul;
    }
}

[Serializable]
public class PlayerData : EntityData
{
    //능력치
    /// <summary>
    /// 방어력
    /// </summary>
    public long defPoint = 0;
    /// <summary>
    /// 운 - 골드 획득량 증가
    /// </summary>
    public float lukPoint = 0;
    /// <summary>
    /// 크리티컬 데미지 (퍼센트)
    /// </summary>
    public float cridPoint = 0;
    /// <summary>
    /// 크리티컬 확률 (퍼센트)
    /// </summary>
    public float cripPoint = 0;

    //능력치별 레벨
    /// <summary>
    /// 공격력 레벨
    /// </summary>
    public int atkLevel = 1;
    /// <summary>
    /// 방어력 레벨
    /// </summary>
    public int defLevel = 1;
    /// <summary>
    /// 운 레벨
    /// </summary>
    public int lukLevel = 1;
    /// <summary>
    /// 치명 데미지 레벨
    /// </summary>
    public int cridLevel = 1;
    /// <summary>
    /// 치명 확률 레벨 
    /// </summary>
    public int cripLevel = 1;

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

    public long RequestSoul(float a, int level) => (long)Mathf.Pow(a * level, 2);

    public float IncreasePoint(float a, int level) => (long)Mathf.Pow(a * level, 2) + 1;
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
    public TextMeshProUGUI description;
    public TextMeshProUGUI soul;
    public Button levelUpButton;
}