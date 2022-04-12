using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillObject : MonoBehaviour
{
    protected SkillInfo skillInfo;
    protected MobInfo playerInfo;

    public void Execute(SkillInfo skillInfo)
    {
        this.skillInfo = skillInfo;
        gameObject.SetActive(true);
    }

    public abstract void Result();
}


