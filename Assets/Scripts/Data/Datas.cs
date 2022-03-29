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
    /// ���ݷ�
    /// </summary>
    public int atkPoint = 0;
    /// <summary>
    /// ���� ������ �ִ� ��ȥ ����
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
    /// ����
    /// </summary>
    public int defPoint = 0;
    /// <summary>
    /// �� - ��� ȹ�淮 ����
    /// </summary>
    public int lukPoint = 0;
    /// <summary>
    /// ũ��Ƽ�� ������ (�ۼ�Ʈ)
    /// </summary>
    public float cridPoint = 0;
    /// <summary>
    /// ũ��Ƽ�� Ȯ�� (�ۼ�Ʈ)
    /// </summary>
    public float cripPoint = 0;
    /// <summary>
    /// �÷��̾ ������ �ִ� ���̾Ƹ�� ����
    /// </summary>
    public int diamond = 0;
    /// <summary>
    ///  �÷��̾ ������ �ִ� ũ����Ż ����
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