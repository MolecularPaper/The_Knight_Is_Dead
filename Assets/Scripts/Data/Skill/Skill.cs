using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

public interface SkillCalculate
{
    public void LevelUp();
}

[System.Serializable]
public class SkillInfo
{
    [HideInInspector] 
    public bool canLevelUp = false;
    public string skillName = "";
    public uint level = 1;
    public ulong point = 0;
    protected bool enbled;

    public SkillInfo() { }

    public SkillInfo(SkillInfo skillInfo) => SetSkill(skillInfo);

    public void SetSkill(SkillInfo skillInfo)
    {
        this.canLevelUp = skillInfo.canLevelUp;
        this.skillName = skillInfo.skillName;
        this.level = skillInfo.level;
        this.point = skillInfo.point;
        this.enbled = skillInfo.enbled;
    }
}

[System.Serializable]
public class SkillExtension : SkillInfo
{
    [SerializeField] protected int maxLevel;

    [Space(10)]
    [SerializeField] protected ulong pointInc;
    [SerializeField] protected uint skillPointInc;

    [Space(10)]
    [SerializeField] protected int coolTimeSecond;

    [Space(10)]
    [SerializeField] protected SkillObject skillObject;

    public ulong NextPoint => point + pointInc;

    public uint RequestSkillPoint => level * skillPointInc + 3;
}

[System.Serializable]
public class SkillObservable : SkillExtension, ISkillObservable
{
    private delegate void SkillUpdateDel(SkillExtension skillExtension);
    private SkillUpdateDel skillUpdateDel;

    public void SkillUpdated()
    {
        if (skillUpdateDel != null) skillUpdateDel.Invoke(this);
    }

    public void Subscribe(ISkillObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        skillUpdateDel += observer.SkillUpdated;
    }

    public void Unsubscribe(ISkillObserver observer)
    {
        if (observer == null)
            throw new System.NullReferenceException();

        skillUpdateDel -= observer.SkillUpdated;
    }
}

[System.Serializable]
public class Skill : SkillObservable, IPlayerObserver, SkillCalculate
{
    public void LevelUp()
    {
        level++;
        point += pointInc;
    }

    public void PlayerUpdated(PlayerInfoExtension playerInfo)
    {
        canLevelUp = playerInfo.skillPoint >= RequestSkillPoint;
        SkillUpdated();
    }

    public void EnbledSkill()
    {
        enbled = true;
        SkillLoop();
    }

    public void DisableSkill() => enbled = false; 

    public async void SkillLoop()
    {
        while (enbled) {
            skillObject.Execute(this);
            await Task.Delay(coolTimeSecond * 1000);
        }
    }
}
