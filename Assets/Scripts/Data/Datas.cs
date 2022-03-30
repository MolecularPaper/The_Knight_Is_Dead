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
    /// ���ݷ�
    /// </summary>
    public long atkPoint = 0;
    /// <summary>
    /// ���� ������ �ִ� ��ȥ ����
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
    //�ɷ�ġ
    /// <summary>
    /// ����
    /// </summary>
    public long defPoint = 0;
    /// <summary>
    /// �� - ��� ȹ�淮 ����
    /// </summary>
    public float lukPoint = 0;
    /// <summary>
    /// ũ��Ƽ�� ������ (�ۼ�Ʈ)
    /// </summary>
    public float cridPoint = 0;
    /// <summary>
    /// ũ��Ƽ�� Ȯ�� (�ۼ�Ʈ)
    /// </summary>
    public float cripPoint = 0;

    //�ɷ�ġ�� ����
    /// <summary>
    /// ���ݷ� ����
    /// </summary>
    public int atkLevel = 1;
    /// <summary>
    /// ���� ����
    /// </summary>
    public int defLevel = 1;
    /// <summary>
    /// �� ����
    /// </summary>
    public int lukLevel = 1;
    /// <summary>
    /// ġ�� ������ ����
    /// </summary>
    public int cridLevel = 1;
    /// <summary>
    /// ġ�� Ȯ�� ���� 
    /// </summary>
    public int cripLevel = 1;

    //��ȭ
    /// <summary>
    /// �÷��̾ ������ �ִ� ���̾Ƹ�� ����
    /// </summary>
    public int diamond = 0;
    /// <summary>
    ///  �÷��̾ ������ �ִ� ũ����Ż ����
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