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
    public int currentHp;

    [Header("State")]
    public int hpPoint = 0;
    /// <summary>
    /// 공격력
    /// </summary>
    public int atkPoint = 0;
    /// <summary>
    /// 현재 가지고 있는 영혼 개수
    /// </summary>
    public int soul;

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
    /// <summary>
    /// 방어력
    /// </summary>
    public int defPoint = 0;
    /// <summary>
    /// 운 - 골드 획득량 증가
    /// </summary>
    public int lukPoint = 0;
    /// <summary>
    /// 크리티컬 데미지 (퍼센트)
    /// </summary>
    public float cridPoint = 0;
    /// <summary>
    /// 크리티컬 확률 (퍼센트)
    /// </summary>
    public float cripPoint = 0;
    /// <summary>
    /// 플레이어가 가지고 있는 다이아몬드 개수
    /// </summary>
    public int diamond = 0;
    /// <summary>
    ///  플레이어가 가지고 있는 크리스탈 개수
    /// </summary>
    public int crystal = 0;
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