using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillInfo
{
    public string skillName;
    public uint level;

    [Space(10)]
    public bool isLock;
    public bool isEnabled;
    public bool canLevelUp;
    public int slotIndex;

    public SkillInfo() { }

    public SkillInfo(SkillInfo skillInfo) => SetSkill(skillInfo); 

    public void SetSkill(SkillInfo skillInfo)
    {
        this.skillName = skillInfo.skillName;
        this.level = skillInfo.level;
        this.isLock = skillInfo.isLock;
        this.isEnabled = skillInfo.isEnabled;
        this.slotIndex = skillInfo.slotIndex;
    }
}

[System.Serializable]
public class SkillExtension : SkillInfo
{
    [Space(10)]
    public ulong pointInc;
    public uint skillPointInc;
    public uint unlockLevel;
    public float coolTime;
    public Sprite icon;

    public ulong Point {
        get {
            return pointInc * (ulong)level;
        }
    }

    public uint RequestSkillPoint {
        get {
            return skillPointInc * level;
        }
    }

    public GameObject skillEffect;
}

public interface ISkillObservable
{
    public void Subscribe(ISkillObserver observer);

    public void Unsubscribe(ISkillObserver observer);

    public void SkillUpdated();
}

public interface ISkillObserver
{
    public void SkillUpdated(SkillExtension skillExtension);
}

public interface ISkill
{
    public void Execute(PlayerInfo playerInfo, EnemyCTRL enemyCTRL);

    public void Unlock();

    public void LevelUp();
}

[System.Serializable]
public class SkillObservable : SkillExtension, ISkillObservable, IPlayerObserver
{
    private delegate void SkillUpdateDel(SkillExtension skillExtension);
    private SkillUpdateDel skillUpdatedDel;

    public void Subscribe(ISkillObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        skillUpdatedDel += observer.SkillUpdated;
    }

    public void Unsubscribe(ISkillObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        skillUpdatedDel -= observer.SkillUpdated;
    }

    public void SkillUpdated()
    {
        if (skillUpdatedDel != null) skillUpdatedDel.Invoke(this);
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        isLock = playerInfo.level < unlockLevel;
        canLevelUp = RequestSkillPoint <= playerInfo.skillPoint;
        SkillUpdated();
    }
}

[System.Serializable]
public class Skill : SkillObservable, ISkill
{
    public void Execute(PlayerInfo playerInfo, EnemyCTRL enemyCTRL)
    {
        GameObject gameObject = GameObject.Instantiate(this.skillEffect, enemyCTRL.transform.position, Quaternion.identity, enemyCTRL.transform);

        SkillEffect skillEffect = gameObject.GetComponent<SkillEffect>();
        skillEffect.skillDamageDel += () => {
            if (enemyCTRL != null) {
                Ability atk = (Ability)playerInfo["ATK"];
                enemyCTRL.Damage((ulong)(atk.point * (Point / 10000f)));
            }
        };
    }

    public void Unlock()
    {
        isLock = false;
        level = 1;
        SkillUpdated();
    }

    public void LevelUp()
    {
        if (!isLock) {
            level++;
        }
    }
}
